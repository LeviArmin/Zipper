using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zipper.Terminal;

namespace Zipper
{
    public class TestCompareFiles
    {
        private int blockSize = 10000000;
        private TerminalProgress progress;

        public TestCompareFiles()
        {
            progress = new TerminalProgress();
            progress.ProgressColor = ConsoleColor.Cyan;
            progress.ProgressWidth = 50;
            progress.MaxProgress = 1;
        }

        public bool CompareFiles(string sourceFile, string resultFile)
        {
            bool result = true;
            byte[] sourceBuffer;
            byte[] resultBuffer;
            int length;

            try
            {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
                using (FileStream resultStream = new FileStream(resultFile, FileMode.Open))
                {
                    progress.Text = "Проверка размеров файлов...";
                    if (sourceStream.Length != resultStream.Length)
                    {
                        result = false;
                    }
                    progress.Text = "Проверка значений файлов...";
                    progress.MaxProgress += sourceStream.Length;
                    progress.Progress++;

                    while (result && sourceStream.Position < sourceStream.Length &&
                        resultStream.Position < resultStream.Length)
                    {
                        if (sourceStream.Length - sourceStream.Position < blockSize)
                        {
                            length = Convert.ToInt32(sourceStream.Length - sourceStream.Position);
                            sourceBuffer = new byte[length];
                            resultBuffer = new byte[length];
                        }
                        else
                        {
                            length = blockSize;
                            sourceBuffer = new byte[length];
                            resultBuffer = new byte[length];
                        }

                        sourceStream.Read(sourceBuffer, 0, length);
                        resultStream.Read(resultBuffer, 0, length);

                        result = sourceBuffer.SequenceEqual(resultBuffer);
                        
                        if (result)
                        {
                            progress.Progress += length;
                        }
                        progress.PrintProgress();
                    }
                }
                progress.Text = "Сравнение завершено.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("При сравнении файлов произошла ошибка!");
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return result;
        }
    }
}
