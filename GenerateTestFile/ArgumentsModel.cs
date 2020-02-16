using System;
using System.Collections.Generic;
using System.Text;

namespace GenerateTestFile
{
    public class ArgumentsModel
    {
        [CommandLine.Option(longName: "size", Default = 32000, HelpText = "Size file in MB", Required = false)]
        public long FileSizeMb { get; set; }

        [CommandLine.Option(longName: "fileName", Default = "text.txt", Required = false, HelpText = "Generated file")]
        public string FileName { get; set; }
        [CommandLine.Option(longName: "blockSize", Default = 1000000, Required = false, HelpText = "Generate block size in bytes")]
        public long BlockSize { get; set; }
    }
}
