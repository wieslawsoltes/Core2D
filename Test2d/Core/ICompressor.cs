// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public interface ICompressor
    {
        byte[] Compress(byte[] data);
        byte[] Decompress(byte[] data);
    }
}
