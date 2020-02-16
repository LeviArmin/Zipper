using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Zipper.Compression.Models;

namespace Zipper.Compression.Logic
{
    /// <summary>
    /// очередь архивации данных
    /// </summary>
    public class ZipQueue : IDisposable
    {
        private bool followSequence;
        private Queue<BufferModel> bufferQueue;
        private object locker;
        private bool running;
        private int blockId;

        public bool FollowSequence
        {
            get
            {
                return followSequence;
            }
            set
            {
                SetProperty(ref followSequence, value);
            }
        }

        public ZipQueue()
        {
            blockId = 0;
            followSequence = false;
            bufferQueue = new Queue<BufferModel>();
            locker = new object();
        }

        public void Begin()
        {
            lock (locker)
            {
                running = true;
                Monitor.PulseAll(locker);
            }
        }
        public void End()
        {
            lock (locker)
            {
                running = false;
                Monitor.PulseAll(locker);
            }
        }

        /// <summary>
        /// добавить данные в очередь
        /// </summary>
        /// <param name="model">модель буфера</param>
        public void Push(BufferModel model)
        {
            ValidateState();

            lock (locker)
            {
                if (running)
                {
                    model.Id = model.Id ?? blockId;

                    while (followSequence && blockId != model.Id && running)
                    {
                        Monitor.Wait(locker);
                    }

                    if (!running)
                        return;
                    
                    blockId++;
                    bufferQueue.Enqueue(model);
                    Monitor.PulseAll(locker);
                }
            }
        }

        /// <summary>
        /// извлечь данные из очереди
        /// </summary>
        /// <returns>модель буфера</returns>
        public BufferModel Pop()
        {
            BufferModel result = null;

            lock (locker)
            {
                ValidateState(false);

                while (bufferQueue.Count == 0 && running)
                {
                    Monitor.Wait(locker);
                }

                if (bufferQueue.Count > 0)
                {
                    result = bufferQueue.Dequeue();
                }

                return result;
            }
        }

        //валидация состояния очереди
        private void ValidateState(bool checkRunning = true)
        {
            if (!running && checkRunning)
            {
                throw new InvalidOperationException("Очередь не может больше принимать объекты.");
            }

            if (Disposed)
            {
                throw new ObjectDisposedException("Объект уже уничтожен!");
            }
        }

        //установить свойство
        private void SetProperty<T>(ref T target, T value)
        {
            if (!running && !target.Equals(value))
            {
                target = value;
            }
        }

        #region IDisposable Support
        public bool Disposed { get; private set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    running = false;
                    lock (locker)
                    {
                        Monitor.PulseAll(locker);
                    }
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                Disposed = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~ZipQueue()
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
}
