using System;
using System.Collections.Generic;
using System.Text;
using Zipper.Compression.Models;

namespace Zipper.Compression.Interfaces
{
    public interface IZip : IDisposable
    {
        event Action<BufferModel> Done;

        void Execute(BufferModel model);
    }
}
