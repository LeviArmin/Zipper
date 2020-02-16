using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Zipper.Terminal
{
    /// <summary>
    /// класс сериализации аргументов командной строки
    /// </summary>
    public class TerminalSerializer
    {
        /// <summary>
        /// десериализовать аргументы в 
        /// </summary>
        /// <param name="args">входные аргументы командной строки</param>
        /// <returns>модель аргументов командной строки</returns>
        public TerminalModel Deserialize(string[] args)
        {
            TerminalModel result = new TerminalModel();

            try
            {
                ValidateArgs(args);
                if (args.Length == 1)
                {
                    result.PrintHelp = true;
                }
                else
                {
                    result.CompressionMode = (CompressionMode)Enum.Parse(typeof(CompressionMode), args[0], true);
                    result.InputFilePath = args[1];
                    result.OutputFilePath = args[2];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        // Валидация аргументов командной строки
        private void ValidateArgs(string[] args)
        {
            switch (args.Length)
            {     
                case 3:
                    if (args[0].ToLower() != "compress" && args[0].ToLower() != "decompress")
                    {
                        throw new ArgumentException($"Неизвестный параметр режима архивации '{args[0]}'.");
                    }

                    if (string.IsNullOrEmpty(args[1]))
                    {
                        throw new ArgumentException("Не указан входной файл!");
                    }
                    
                    if (string.IsNullOrEmpty(args[2]))
                    {
                        throw new ArgumentException("Не указан выходной файл!");
                    }

                    if (!File.Exists(args[1]))
                    {
                        throw new FileNotFoundException("Указаный путь к исходному файлу не найден!");
                    }

                    //if (File.Exists(args[2]))
                    //{
                    //    throw new ArgumentException("Файл вывода уже существует!");
                    //}
                    break;

                default:
                    if (args.Length < 1 || args[0].ToLower() != "help")
                    {
                        throw new ArgumentOutOfRangeException("Некорректное занчение аргументов!");
                    }
                    break;
            }
        }
    }
}
