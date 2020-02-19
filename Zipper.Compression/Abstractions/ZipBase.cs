using System;
using System.IO;
using System.Threading;
using Zipper.Compression.Logic;
using Zipper.Compression.Models;

namespace Zipper.Compression.Abstractions
{
    /// <summary>
    /// базовый класс управления компрессией/декомпрессией
    /// </summary>
    public abstract class ZipBase : ZipProgressBase, IDisposable
    {
        protected bool canceled = false;

        protected string inputFile;
        protected string outputFile;

        protected int blockSize;

        protected ZipQueue inputQueue;
        protected ZipQueue outputQueue;

        private Thread reading;
        private Thread writing;

        protected ManualResetEvent busyReadEvent;
        protected ManualResetEvent busyWriteEvent;
        private ManualResetEvent[] manualEvents;

        private CancellationTokenSource cancellationToken;

        protected ZipStreamBase[] zipStreams;

        public event Action<ZipBase, Exception> ZipException;
        public event Action<ZipBase> Done;

        public ZipBase()
        {
            busyReadEvent = new ManualResetEvent(false);
            busyWriteEvent = new ManualResetEvent(false);

            inputQueue = new ZipQueue { FollowSequence = false };
            outputQueue = new ZipQueue { FollowSequence = true };
        }

        /// <summary>
        /// инициализация (обязательна перед началом работы)
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="blockSize"></param>
        public void Initialize(string inputFile, string outputFile, int blockSize)
        {
            this.inputFile = inputFile;
            this.outputFile = outputFile;
            this.blockSize = blockSize;

            FileInfo info = new FileInfo(inputFile);
            NumberBlocks = Convert.ToInt32(Math.Ceiling(info.Length / (double)blockSize));
        }
        
        /// <summary>
        /// запуск процесса компрессии/декопрессии
        /// </summary>
        public void Start()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
                cancellationToken = null;
            }

            cancellationToken = new CancellationTokenSource();

            int idx = 0;
            manualEvents = new ManualResetEvent[zipStreams.Length];

            outputQueue.Begin();

            ParameterizedThreadStart threadStartReading = new ParameterizedThreadStart(Read);
            reading = new Thread(threadStartReading);
            reading.Start(cancellationToken.Token);

            foreach (var zipStream in zipStreams)
            {
                manualEvents[idx] = zipStream.BusyEvent;
                zipStream.Start(inputQueue, cancellationToken.Token);
                idx++;
            }

            ParameterizedThreadStart threadStartWriting = new ParameterizedThreadStart(Write);
            writing = new Thread(threadStartWriting);
            writing.Start(cancellationToken.Token);
        }
        /// <summary>
        /// ожидание завершения компрессии/декомпрессии
        /// </summary>
        public void Waiting()
        {
            busyReadEvent.WaitOne();
            WaitHandle.WaitAll(manualEvents);
            busyWriteEvent.WaitOne();
            Console.WriteLine("Завершение записи файла. Пожалуйста подождите!");
            outputQueue.End();
            Done?.Invoke(this);
        }
        /// <summary>
        /// остановить работу компрессии/декомпрессии
        /// </summary>
        public void Stop()
        {
            canceled = true;
            cancellationToken.Cancel();
            WaitHandle.WaitAll(manualEvents);
            outputQueue.End();
            inputQueue.End();
        }

        //обработка компрессированных/декомпрессированных данных
        protected abstract void ZippingComplated(BufferModel model);
        //обработка чтения входного файла
        protected abstract void Read(object obj);
        //обработка записи выходного файла
        protected abstract void Write(object obj);

        protected void SendZipException(Exception ex)
        {
            ZipException?.Invoke(this, ex);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        /// <summary>
        /// вохращает состояние объекта. true - если объект уничтожен, иначе - false
        /// </summary>
        public bool Disposed => disposedValue;

        protected abstract void Disposing();

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    Stop();

                    busyReadEvent.Dispose();
                    busyWriteEvent.Dispose();
                    outputQueue.Dispose();
                    inputQueue.Dispose();
                    cancellationToken.Dispose();

                    Disposing();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~ZipBase()
        // {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public abstract class ZipBase<TZipStream> : ZipBase
        where TZipStream : ZipStreamBase, new()
    {
        public ZipBase()
            : base()
        {
            zipStreams = new TZipStream[Environment.ProcessorCount];
            for (int i = 0; i < zipStreams.Length; ++i)
            {
                zipStreams[i] = new TZipStream();
                zipStreams[i].ZipComplated += ZippingComplated;
                zipStreams[i].ZipStreamException += ZipStreamException;
            }
        }

        private void ZipStreamException(ZipStreamBase zipStream, Exception ex)
        {
            SendZipException(ex);
            Stop();
        }

        protected override void Disposing()
        {
            for (int i = 0; i < zipStreams.Length; ++i)
            {
                zipStreams[i].ZipComplated -= ZippingComplated;
                zipStreams[i].ZipStreamException -= ZipStreamException;
            }
        }
    }
}
