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
    /// класс обработки компрессии данных
    /// </summary>
    public class ZipStreamCompressor : ZipStreamBase
    {
        public override event Action<BufferModel> ZipComplated;

        public ZipStreamCompressor()
            : base()
        {
        }

        protected override void Zipping(BufferModel model)
        {
            BufferModel result = new BufferModel();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    zipStream.Write(model.Data, 0, model.Data.Length);
                }
                result.Initialize(model.Id, memoryStream.ToArray());
                ZipComplated?.Invoke(result);
            }
        }

        protected override void Disposing()
        {
        }
    }
}
