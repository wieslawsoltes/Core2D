// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines file system contract.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the location of the assembly as specified originally.
        /// </summary>
        /// <param name="type">The type included in assembly.</param>
        /// <returns>The location of the assembly as specified originally.</returns>
        string GetAssemblyPath(Type type);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>True if path contains the name of an existing file; otherwise, false.</returns>
        bool Exists(string path);

        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>A read-only stream on the specified path.</returns>
        System.IO.Stream Open(string path);

        /// <summary>
        /// Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="path">The path and name of the file to create.</param>
        /// <returns> A stream that provides read/write access to the file specified in path.</returns>
        System.IO.Stream Create(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        byte[] ReadBinary(System.IO.Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        void WriteBinary(System.IO.Stream stream, byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        string ReadUtf8Text(System.IO.Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="text"></param>
        void WriteUtf8Text(System.IO.Stream stream, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ReadUtf8Text(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        void WriteUtf8Text(string path, string text);
    }
}
