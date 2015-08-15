// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utf8TextFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="text"></param>
        public static void Compress(Stream fs, string text)
        {
            using (var cs = new GZipStream(fs, CompressionMode.Compress))
            {
                using (var sw = new StreamWriter(cs, Encoding.UTF8))
                {
                    sw.Write(text);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static string Decompress(Stream fs)
        {
            using (var cs = new GZipStream(fs, CompressionMode.Decompress))
            {
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static string Read(string path, bool compressed = true)
        {
            if (compressed)
            {
                using (var fs = File.OpenRead(path))
                {
                    return Decompress(fs);
                }
            }
            else
            {
                using (var fs = File.OpenRead(path))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="compressed"></param>
        public static void Write(string path, string text, bool compressed = true)
        {
            if (compressed)
            {
                using (var fs = File.Create(path))
                {
                    Compress(fs, text);
                }
            }
            else
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
}
