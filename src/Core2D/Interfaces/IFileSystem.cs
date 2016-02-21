// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines file system contract.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream Open(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Stream Create(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        byte[] ReadBinary(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        void WriteBinary(Stream stream, byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        string ReadUtf8Text(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="text"></param>
        void WriteUtf8Text(Stream stream, string text);

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
