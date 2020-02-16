using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Zipper.Compression.Abstractions;
using Zipper.Compression.Logic;
using Zipper.Compression.Models;
using Zipper.Terminal;

namespace Zipper
{
    /// <summary>
    /// класс управления программой компрессии/декопрессии
    /// </summary>
    public class Zipping : IDisposable
    {
        private TerminalGroupProgress groupProgress;
        private ZipBase zip;
        private bool cancel;
        private object locker;

        public Zipping()
        {
            locker = new object();
            cancel = false;
            groupProgress = new TerminalGroupProgress();
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        //передача отчета прогресса в прогресс бары
        private void ProgressReport(ZipProgressModel obj)
        {
            lock (locker)
            {
                if (!cancel)
                    groupProgress.UpdateProgress(obj.ReadingProgress, obj.WritingProgress, obj.ZippingProgress);
            }
        }

        /// <summary>
        /// запустить 
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            TerminalSerializer serializer = new TerminalSerializer();
            try
            {
                TerminalModel model = serializer.Deserialize(args);

                if (model.PrintHelp)
                {
                    TerminalHelp.PrintHelp();
                }
                else
                {
                    ExecuteZip(model);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка во время выполнения программы: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }


        private void ExecuteZip(TerminalModel model)
        {
            switch (model.CompressionMode)
            {
                case CompressionMode.Compress:
                    zip = new ZipCompressor();
                    break;

                case CompressionMode.Decompress:
                    zip = new ZipDecompressor();
                    break;
            }

            zip.Done += Zip_Done;
            zip.ProgressReport += ProgressReport;
            zip.ZipException += Zip_ZipException;
            zip.Initialize(model.InputFilePath, model.OutputFilePath, model.BlockSize);
            

            groupProgress.AddProgress("Чтение", zip.NumberBlocks, 30, ConsoleColor.Red);
            groupProgress.AddProgress("Запись", zip.NumberBlocks, 30, ConsoleColor.Green);
            groupProgress.AddProgress("Архивация", zip.NumberBlocks, 30, ConsoleColor.Yellow);

            zip.Start();
            zip.Waiting();
            zip.Stop();
            System.Threading.Thread.Sleep(1000);
            groupProgress.PrintAll();
        }

        private void Zip_Done(ZipBase obj)
        {
            lock (locker)
            {
                groupProgress.Terminated = true;
            }
        }

        private void Zip_ZipException(ZipBase zip, Exception ex)
        {
            cancel = true;
            lock (locker)
            {
                groupProgress.Terminated = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Во время выполнения программы произошла одна или несколько ошибок: ");
                Exception currentEx = ex;
                while (currentEx != null)
                {
                    Console.WriteLine(ex.Message);
                    currentEx = ex.InnerException;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            cancel = true;
            zip.Stop();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    Console.CancelKeyPress -= Console_CancelKeyPress;
                    if (zip != null)
                    {
                        zip.ProgressReport -= ProgressReport;
                        zip.Dispose();
                        zip = null;
                    }
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~Zipping()
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
