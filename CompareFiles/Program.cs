using System;

namespace CompareFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Zipper.TestCompareFiles compareFiles = new Zipper.TestCompareFiles();
            if (args.Length == 2)
            {
                bool result = compareFiles.CompareFiles(args[0], args[1]);
                if (result)
                {
                    Console.WriteLine("Файлы одинаковые!");
                }
                else
                {
                    Console.WriteLine("Файлы содержат разные данные!");
                }
            }
            else
            {
                Console.WriteLine("Для сравнения файлов необходимо указать два файла [файл 1] [файл 2]");
            }
        }
    }
}
