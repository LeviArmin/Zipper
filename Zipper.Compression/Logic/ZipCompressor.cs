using System;
using System.IO;
using System.Threading;
using Zipper.Compression.Abstractions;
using Zipper.Compression.Models;
using Zipper.Terminal;

namespace Zipper.Compression.Logic
{
    /// <summary>
    /// класс компрессии
    /// </summary>
    public class ZipCompressor : ZipBase<ZipStreamCompressor>
    {
        public ZipCompressor()
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
            byte[] buffer;
            int length;

            inputQueue.Begin();
            try
            {
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    while (!token.IsCancellationRequested && inputStream.Position < inputStream.Length)
                    {
                        if (inputStream.Length - inputStream.Position < blockSize)
                        {
                            length = Convert.ToInt32(inputStream.Length - inputStream.Position);
                        }
                        else
                        {
                            length = blockSize;
                        }

                        buffer = new byte[length];
                        inputStream.Read(buffer, 0, buffer.Length);
                        BufferModel model = new BufferModel();
                        model.Initialize(null, buffer);

                        if (!canceled)
                            inputQueue.Push(model);

                        //сообщить прогресс
                        UpdateProgressReading();
                    }
                }

            }
            catch (Exception ex)
            {
                SendZipException(ex);
                Stop();
            }
            inputQueue.End();
            outputQueue.AutoCloseQueueByCapacity(inputQueue.TotalBlocks);
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
            byte[] bufferLength;

            try
            {
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                {
                    while (!token.IsCancellationRequested)
                    {
                        BufferModel model = outputQueue.Pop();
                        if (model == null)
                            break;

                        bufferLength = BitConverter.GetBytes(model.Data.Length);
                        bufferLength.CopyTo(model.Data, 4);

                        outputStream.Write(model.Data, 0, model.Data.Length);

                        //сообщить прогресс
                        UpdateProgressWriting();
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
