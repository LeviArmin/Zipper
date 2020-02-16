using System;
using System.Collections.Generic;
using System.Text;

namespace Zipper.Terminal
{
    public static class TerminalHelp
    {
        public static void PrintHelp()
        {
            Console.WriteLine("[compress/decompress] [inputeFile] [outputFile]");
            Console.WriteLine("Пример: zipper.exe compress test1.txt test1.gz");
            Console.WriteLine("Пример: zipper.exe decompress test1.gz result.txt");
        }

        public static void PrintWelcome()
        {
            Console.WriteLine("Программа для компрессии/декопрессии файлов.");
            Console.WriteLine("Выполняется работа с файлом. Пожалуйста дождитесь завершения операции.");
            Console.WriteLine("Чтобы корректно остановить выполнения программы нажмите ctrl+c");
        }
    }
}
