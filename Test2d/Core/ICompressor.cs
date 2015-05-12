// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Represents a binary data compressor.
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compresses the input byte array using external compressor.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <returns>The compressed input byte array.</returns>
        byte[] Compress(byte[] data);

        /// <summary>
        /// Decompresses the input byte array using external decompressor.
        /// </summary>
        /// <param name="data">The input byte array.</param>
        /// <returns>The decompressed input byte array.</returns>
        byte[] Decompress(byte[] data);
    }
}
