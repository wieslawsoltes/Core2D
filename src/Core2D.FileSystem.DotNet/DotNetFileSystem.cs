// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;
using Core2D.Interfaces;

namespace Core2D.FileSystem.DotNet
{
    /// <summary>
    /// File system implementation using System.IO.
    /// </summary>
    public sealed class DotNetFileSystem : IFileSystem
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DotNetFileSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        string IFileSystem.GetBaseDirectory()
        {
            return AppContext.BaseDirectory;
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
