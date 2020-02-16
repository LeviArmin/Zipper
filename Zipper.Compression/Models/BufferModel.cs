using System;
using System.Collections.Generic;
using System.Text;

namespace Zipper.Compression.Models
{
    /// <summary>
    /// Нумерованный буфер данных
    /// </summary>
    public class BufferModel
    {
        private int? id;
        private byte[] data;

        /// <summary>
        /// идентификатор блока данных
        /// </summary>
        public int? Id { get => id; set => id = value; }
        /// <summary>
        /// буфер данных
        /// </summary>
        public byte[] Data => data;

        public void Initialize(int? id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
