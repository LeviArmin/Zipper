using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Zipper.Compression.Abstractions;
using Zipper.Compression.Models;

namespace Zipper.Compression.Logic
{

    /// <summary>
    /// класс декомпрессии
    /// </summary>
    public class ZipDecompressor : ZipBase<ZipStreamDecompressor>
    {
        public ZipDecompressor()
            : base()
        {
        }

        /// <summary>
        /// чтение данных из входного файла
        /// </summary>
        /// <param name="obj">токен отмены</param>
        protected override void Read(object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            byte[] tempBuffer = new byte[12];

            try
            {
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    while (!token.IsCancellationRequested && inputStream.Position < inputStream.Length)
                    {
                        inputStream.Read(tempBuffer, 0, tempBuffer.Length);
                        int sizeCompressedBlock = BitConverter.ToInt32(tempBuffer, 4);

                        BufferModel model = new BufferModel();
                        model.Initialize(null, new byte[sizeCompressedBlock]);
                        tempBuffer.CopyTo(model.Data, 0);

                        inputStream.Read(model.Data, 12, sizeCompressedBlock - 12);

                        if (!canceled)
                            inputQueue.Push(model);

                        //сообщить прогресс
                        UpdateProgressReading();
                    }
                    outputQueue.AutoCloseQueueByCapacity(inputQueue.TotalBlocks);
                }
            }
            catch (Exception ex)
            {
                SendZipException(ex);
                Stop();
            }
            inputQueue.End();
            busyReadEvent.Set();
        }

        /// <summary>
        /// завершение выполнения декомпрессии данных
        /// </summary>
        /// <param name="model"></param>
        protected override void ZippingComplated(BufferModel model)
        {
            if (!canceled)
                outputQueue.Push(model);

            //сообщить прогресс
            UpdateProgressZipping();
        }

        /// <summary>
        /// запись декопрессированных данных в выходной файл
        /// </summary>
        /// <param name="obj">токен отмены</param>
        protected override void Write(object obj)
        {
            CancellationToken token = (CancellationToken)obj;

            try
            {
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                {
                    while (!token.IsCancellationRequested)
                    {
                        BufferModel model = outputQueue.Pop();
                        if (model != null)
                        {
                            outputStream.Write(model.Data, 0, model.Data.Length);

                            //сообщить прогресс
                            UpdateProgressWriting();
                        }
                        else if (!outputQueue.Running)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendZipException(ex);
                Stop();
            }
            busyWriteEvent.Set();
        }

        protected override void Disposing()
        {
        }
    }
}
