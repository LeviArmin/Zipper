using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace Zipper.Terminal
{
    /// <summary>
    /// модель аргументов командной строки
    /// </summary>
    public class TerminalModel
    {
        /// <summary>
        /// режим сжатия (упаковать/распаковать)
        /// </summary>
        public CompressionMode CompressionMode { get; set; }
        /// <summary>
        /// входной файл
        /// </summary>
        public string InputFilePath { get; set; }
        /// <summary>
        /// выходной файл
        /// </summary>
        public string OutputFilePath { get; set; }
        /// <summary>
        /// вывести справку помощи
        /// </summary>
        public bool PrintHelp { get; set; }
        /// <summary>
        /// размер блока
        /// </summary>
        public int BlockSize { get; set; } = 1000000;
    }
}
