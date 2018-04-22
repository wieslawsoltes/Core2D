// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System;
using System.Reflection;
using System.Text;

namespace Core2D.FileSystem.DotNet
{
    /// <summary>
    /// File system implementation using System.IO.
    /// </summary>
    public sealed class DotNetFileSystem : IFileSystem
    {
        /// <inheritdoc/>
        string IFileSystem.GetAssemblyPath(Type type)
        {
            // HACK: Commented to get CoreRT working.
            /*
#if NETSTANDARD2_0
            string codeBase = type.GetTypeInfo().Assembly.FullName;
#else
            string codeBase = type == null ? Assembly.GetEntryAssembly().CodeBase : type.GetTypeInfo().Assembly.FullName;
#endif
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
            */
            return "";
        }

        /// <inheritdoc/>
        bool IFileSystem.Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <inheritdoc/>
        System.IO.Stream IFileSystem.Open(string path)
        {
            return new System.IO.FileStream(path, System.IO.FileMode.Open);
        }

        /// <inheritdoc/>
        System.IO.Stream IFileSystem.Create(string path)
        {
            return new System.IO.FileStream(path, System.IO.FileMode.Create);
        }

        /// <inheritdoc/>
        byte[] IFileSystem.ReadBinary(System.IO.Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
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
        void IFileSystem.WriteBinary(System.IO.Stream stream, byte[] bytes)
        {
            using (var bw = new System.IO.BinaryWriter(stream))
            {
                bw.Write(bytes);
            }
        }

        /// <inheritdoc/>
        string IFileSystem.ReadUtf8Text(System.IO.Stream stream)
        {
            using (var sr = new System.IO.StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <inheritdoc/>
        void IFileSystem.WriteUtf8Text(System.IO.Stream stream, string text)
        {
            using (var sw = new System.IO.StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(text);
            }
        }

        /// <inheritdoc/>
        string IFileSystem.ReadUtf8Text(string path)
        {
            using (var fs = System.IO.File.OpenRead(path))
            {
                return (this as IFileSystem).ReadUtf8Text(fs);
            }
        }

        /// <inheritdoc/>
        void IFileSystem.WriteUtf8Text(string path, string text)
        {
            using (var fs = System.IO.File.Create(path))
            {
                (this as IFileSystem).WriteUtf8Text(fs, text);
            }
        }
    }
}
