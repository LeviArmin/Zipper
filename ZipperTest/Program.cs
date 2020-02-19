using System;
using System.Diagnostics;

namespace ZipperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateFile();
            Compress();
            Decompress();
            //Compare();
        }

        static void Compress()
        {
            StartProc("Zipper.dll compress text.txt text.txt.gz");
        }
        static void Decompress()
        {
            StartProc("Zipper.dll decompress text.txt.gz result.txt");
        }
        static void Compare()
        {
            StartProc("CompareFiles.dll text.txt result.txt");
        }

        static void StartProc(string arguments)
        {
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            proc.Start();
            proc.WaitForExit();
            proc.Dispose();
        }

        static void CreateFile()
        {
            StartProc("GenerateTestFile.dll --size=5000");
        }

    }
}
