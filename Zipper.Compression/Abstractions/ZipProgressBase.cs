using System;
using System.Collections.Generic;
using System.Text;
using Zipper.Compression.Models;

namespace Zipper.Compression.Abstractions
{
    /// <summary>
    /// базовый класс обработки прогресса компрессии/декомпрессии
    /// </summary>
    public abstract class ZipProgressBase
    {
        private object locker;
        private ZipProgressModel model;

        public event Action<ZipProgressModel> ProgressReport;

        /// <summary>
        /// количество блоков
        /// </summary>
        public int NumberBlocks
        {
            get => model.NumberBlocks;
            set => model.NumberBlocks = value;
        }
        /// <summary>
        /// икнремента прогресса
        /// </summary>
        public int Invrement { get; set; } = 1;

        public ZipProgressBase()
        {
            locker = new object();
            model = new ZipProgressModel();
        }

        /// <summary>
        /// обновить прогресс чтения
        /// </summary>
        protected void UpdateProgressReading()
        {
            lock (locker)
            {
                model.ReadingProgress += Invrement;
                ProgressReport?.Invoke(model);
            }
        }
        /// <summary>
        /// обновить прогресс копрессии/декопрессии
        /// </summary>
        protected void UpdateProgressZipping()
        {
            lock (locker)
            {
                model.ZippingProgress += Invrement;
                ProgressReport?.Invoke(model);
            }
        }
        /// <summary>
        /// обновить прогресс записи
        /// </summary>
        protected void UpdateProgressWriting()
        {
            lock (locker)
            {
                model.WritingProgress += Invrement;
                ProgressReport?.Invoke(model);
            }
        }
    }
}
