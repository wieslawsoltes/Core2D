// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace FileSystem.DotNet
{
    /// <summary>
    /// File system implementation using System.IO.
    /// </summary>
    public sealed class DotNetFileSystem : IFileSystem
    {
        /// <inheritdoc/>
        string IFileSystem.GetAssemblyPath(Type type)
        {
#if NETSTANDARD1_3 || NETCORE5_0
            string codeBase = type.GetTypeInfo().Assembly.FullName;
#else
            string codeBase = type == null ? Assembly.GetEntryAssembly().CodeBase : type.GetTypeInfo().Assembly.FullName;
#endif
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /// <inheritdoc/>
        bool IFileSystem.Exists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc/>
        Stream IFileSystem.Open(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        /// <inheritdoc/>
        Stream IFileSystem.Create(string path)
        {
            return new FileStream(path, FileMode.Create);
        }

        /// <inheritdoc/>
        byte[] IFileSystem.ReadBinary(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <inheritdoc/>
        void IFileSystem.WriteBinary(Stream stream, byte[] bytes)
        {
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(bytes);
            }
        }

        /// <inheritdoc/>
        string IFileSystem.ReadUtf8Text(Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <inheritdoc/>
        void IFileSystem.WriteUtf8Text(Stream stream, string text)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(text);
            }
        }

        /// <inheritdoc/>
        string IFileSystem.ReadUtf8Text(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return (this as IFileSystem).ReadUtf8Text(fs);
            }
        }

        /// <inheritdoc/>
        void IFileSystem.WriteUtf8Text(string path, string text)
        {
            using (var fs = File.Create(path))
            {
                (this as IFileSystem).WriteUtf8Text(fs, text);
            }
        }
    }
}
