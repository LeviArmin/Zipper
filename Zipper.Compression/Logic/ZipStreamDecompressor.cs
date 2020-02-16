using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Zipper.Compression.Abstractions;
using Zipper.Compression.Models;

namespace Zipper.Compression.Logic
{
    /// <summary>
    /// класс обработки декомпрессии данных
    /// </summary>
    public class ZipStreamDecompressor : ZipStreamBase
    {
        public override event Action<BufferModel> ZipComplated;

        public ZipStreamDecompressor()
            : base()
        {
        }

        protected override void Zipping(BufferModel model)
        {
            BufferModel result = new BufferModel();
            int length = ReadLength(model);
            result.Initialize(model.Id, new byte[length]);

            using (MemoryStream memoryStream = new MemoryStream(model.Data))
            using (GZipStream zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                zipStream.Read(result.Data, 0, result.Data.Length);

                ZipComplated?.Invoke(result);
            }
        }

        private int ReadLength(BufferModel model)
        {
            return BitConverter.ToInt32(model.Data, model.Data.Length - 4);
        }

        protected override void Disposing()
        {
        }
    }
}
