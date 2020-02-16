using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Zipper.Compression.Logic;
using Zipper.Compression.Models;
using Zipper.Terminal;

namespace Zipper.Compression.Abstractions
{
    /// <summary>
    /// базовый класс потока компрессии//декомпрессии
    /// </summary>
    public abstract class ZipStreamBase : IDisposable
    {
        private Thread zipThread;
        private ManualResetEvent busyEvent;
        private ZipQueue inputQueue;

        public ManualResetEvent BusyEvent => busyEvent;

        public abstract event Action<BufferModel> ZipComplated;
        public event Action<ZipStreamBase, Exception> ZipStreamException;

        public ZipStreamBase()
        {
            busyEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// компрессия/декомпрессия данных
        /// </summary>
        /// <param name="model">модель буфера</param>
        protected abstract void Zipping(BufferModel model);

        /// <summary>
        /// запуск потока
        /// </summary>
        /// <param name="inputQueue">очередь входных данных</param>
        /// <param name="token">токен отмены</param>
        public void Start(ZipQueue inputQueue, CancellationToken token)
        {
            this.inputQueue = inputQueue;
            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(Launch);
            zipThread = new Thread(parameterizedThreadStart);
            zipThread.Start(token);
        }

        private void Launch(object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    BufferModel model = inputQueue.Pop();
                    if (model == null)
                    {
                        break;
                    }
                    Zipping(model);
                }
            }
            catch (Exception ex)
            {
                ZipStreamException?.Invoke(this, ex);
            }
            busyEvent.Set();
        }

        #region IDisposable Support
        public bool Disposed { get; private set; } = false;

        protected abstract void Disposing();

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Disposing();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Disposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ZipBase()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
