// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using LZ4;
using Test2d;

namespace Test.Compressors
{
    public class LZ4CodecCompressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            return LZ4Codec.Wrap(data, 0, data.Length);
        }

        public byte[] Decompress(byte[] data)
        {
            return LZ4Codec.Unwrap(data, 0);
        }
    }
}
