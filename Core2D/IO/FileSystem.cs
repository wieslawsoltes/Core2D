// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// File system implementation using System.IO.
    /// </summary>
    public sealed class FileSystem : IFileSystem
    {
        /// <inheritdoc/>
        public Stream Open(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        /// <inheritdoc/>
        public Stream Create(string path)
        {
            return new FileStream(path, FileMode.Create);
        }

        /// <inheritdoc/>
        public byte[] ReadBinary(Stream stream)
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
        public void WriteBinary(Stream stream, byte[] bytes)
        {
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(bytes);
            }
        }

        /// <inheritdoc/>
        public string ReadUtf8Text(Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <inheritdoc/>
        public void WriteUtf8Text(Stream stream, string text)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(text);
            }
        }

        /// <inheritdoc/>
        public string ReadUtf8Text(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <inheritdoc/>
        public void WriteUtf8Text(string path, string text)
        {
            using (var fs = File.Create(path))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(text);
                }
            }
        }
    }
}
