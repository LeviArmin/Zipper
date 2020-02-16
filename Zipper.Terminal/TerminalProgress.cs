using System;
using System.Collections.Generic;
using System.Text;

namespace Zipper.Terminal
{
    /// <summary>
    /// Консольный прогресс бар
    /// </summary>
    public class TerminalProgress
    {
        private const char fullProgress = '█';
        private const char halfProgress = '▌';
        
        private double progress = 0;
        private double maxProgress = 100;
        private int progressWidth = 30;
        private string text = "";
        private ConsoleColor progressColor = ConsoleColor.Green;

        /// <summary>
        /// событие обновления прогресс бара
        /// </summary>
        public event Action<TerminalProgress> ProgressUpdated;

        /// <summary>
        /// Цвет полосы прогресса
        /// </summary>
        public ConsoleColor ProgressColor
        {
            get
            {
                return progressColor;
            }
            set
            {
                SetProperty(ref progressColor, value);
            }
        }
        /// <summary>
        /// ширина прогрес бара (кол-во полей)
        /// </summary>
        public int ProgressWidth
        {
            get
            {
                return progressWidth;
            }
            set
            {
                SetProperty(ref progressWidth, value);
            }
        }
        /// <summary>
        /// максимальное значение прогресса
        /// </summary>
        public double MaxProgress
        {
            get
            {
                return maxProgress;
            }
            set
            {
                SetProperty(ref maxProgress, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (value <= maxProgress)
                    SetProperty(ref progress, value);
            }
        }
        /// <summary>
        /// текстовая подпись
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                SetProperty(ref text, value);
            }
        }

        /// <summary>
        /// установить значение свойства
        /// </summary>
        /// <typeparam name="T">тип данных свойства</typeparam>
        /// <param name="target">поле свойства</param>
        /// <param name="value">новое значение</param>
        /// <param name="postAction">событие уведомления об изменении свойства</param>
        private void SetProperty<T>(ref T target, T value, Action postAction = null)
        {
            if (!target.Equals(value))
            {
                target = value;
                postAction?.Invoke();

                ProgressUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// рисование прогресс бара
        /// </summary>
        public void PrintProgress()
        {
            double totalProgress = Math.Round(progress * 100 / maxProgress, 2);
            double totalWidth = progressWidth * totalProgress / 100.0d;
            bool addHalf = totalWidth - Math.Truncate(totalWidth) >= 0.5;
            int insertFull = Convert.ToInt32(Math.Floor(totalWidth));
            string progressText = "".PadRight(insertFull, fullProgress);
            if (addHalf)
            {
                progressText += halfProgress;
            }
            progressText = progressText.PadRight(progressWidth, ' ');

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"\r[");
            Console.ForegroundColor = progressColor;
            Console.Write(progressText);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"]\t{totalProgress, 6}% {Text,20}");
        }
    }
}
