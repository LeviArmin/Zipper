using System;
using System.Collections.Generic;
using System.Text;

namespace Zipper.Compression.Models
{
    public class ZipProgressModel
    {
        public int NumberBlocks { get; set; } = 0;

        public int ReadingProgress { get; set; } = 0;
        public int WritingProgress { get; set; } = 0;
        public int ZippingProgress { get; set; } = 0;
    }
}
