using System;
using System.Collections.Generic;
using System.Text;

namespace Zipper.Terminal
{
    /// <summary>
    /// класс группы прогресс баров
    /// </summary>
    public class TerminalGroupProgress : IDisposable
    {
        private bool printed;
        private List<TerminalProgress> terminalProgresses;
        private object locker;
        private bool updateProgress;
        private bool terminated;

        public bool Terminated
        {
            get => terminated;
            set => terminated = value;
        }

        /// <summary>
        /// получить прогресс бар по индексу
        /// </summary>
        /// <param name="index">индекс</param>
        /// <returns>консольный прогерсс бар</returns>
        public TerminalProgress this[int index]
        {
            get
            {
                return terminalProgresses[index];
            }
        }

        public TerminalGroupProgress()
        {
            updateProgress = false;
            printed = false;
            locker = new object();
            terminalProgresses = new List<TerminalProgress>();
        }

        /// <summary>
        /// добавить новый прогресс бар
        /// </summary>
        /// <param name="text">подпись</param>
        /// <param name="maxProgress">максимальное значение прогресса</param>
        /// <param name="progressWidth">ширина полосы прогресса</param>
        /// <param name="progressColor">цвет полосы прогресса</param>
        public void AddProgress(string text, double maxProgress, int progressWidth, ConsoleColor progressColor)
        {
            TerminalProgress progress = new TerminalProgress
            {
                Text = text,
                MaxProgress = maxProgress,
                ProgressWidth = progressWidth,
                ProgressColor = progressColor,
                Progress = 0
            };

            progress.ProgressUpdated += Progress_ProgressUpdated;

            terminalProgresses.Add(progress);
        }

        public void UpdateProgress(params int[] progress)
        {
            updateProgress = true;
            for (int i = 0; i < progress.Length; ++i)
            {
                terminalProgresses[i].Progress = progress[i];
            }
            PrintAll();
            updateProgress = false;
        }

        private void Progress_ProgressUpdated(TerminalProgress obj)
        {
            if (!updateProgress)
                PrintAll();
        }

        public void PrintAll()
        {
            if (Disposed || terminated)
                return;

            lock (locker)
            {
                Console.CursorLeft = 0;
                if (printed)
                    Console.CursorTop -= terminalProgresses.Count;
                foreach (var progress in terminalProgresses)
                {
                    progress.PrintProgress();
                    Console.WriteLine();
                }
                printed = true;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        public bool Disposed => disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    foreach (var progress in terminalProgresses)
                    {
                        progress.ProgressUpdated -= Progress_ProgressUpdated;
                    }
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~TerminalGroupProgress()
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
