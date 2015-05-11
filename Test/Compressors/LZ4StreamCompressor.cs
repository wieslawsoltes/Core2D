// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.IO.Compression;
using LZ4;
using Test2d;

namespace Test.Compressors
{
    public class LZ4StreamCompressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new LZ4Stream(ms, CompressionMode.Compress, true))
                {
                    cs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new LZ4Stream(ms, CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = cs.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
        }
    }
}
