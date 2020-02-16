using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zipper.Terminal;

namespace GenerateTestFile
{
    class Program
    {
        static TerminalProgress progress;

        static void Main(string[] args)
        {
            progress = new TerminalProgress();

            Parser.Default.ParseArguments<ArgumentsModel>(args).MapResult(ParseSuccess, ParseFail);
        }

        static bool ParseSuccess(ArgumentsModel model)
        {
            bool result = true;
            long length = model.FileSizeMb * 1000000;

            progress.Text = "Генерация файла";
            progress.ProgressWidth = 50;
            progress.ProgressColor = ConsoleColor.Magenta;
            progress.MaxProgress = length;
            progress.PrintProgress();

            try
            {
                using (FileStream stream = new FileStream(model.FileName, FileMode.OpenOrCreate))
                {
                    while (length > 0)
                    {
                        long size = 0;
                        if (length < model.BlockSize)
                            size = length;
                        else
                            size = model.BlockSize;
                        byte[] buffer = Generate(size);
                        stream.Write(buffer, 0, buffer.Length);

                        progress.Progress += size;
                        length -= size;
                        progress.PrintProgress();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generate file error: {ex.Message}");
                result = false;
            }

            return result;
        }

        static byte[] Generate(long blockSize = 1000000)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (long i = 0; i < blockSize; ++i)
            {
                builder.Append((char)random.Next(0, 256));
            }
            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        static bool ParseFail(IEnumerable<Error> errors)
        {
            Console.WriteLine();

            return true;
        }
    }
}
