// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
        /// <param name="path"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static string Read(string path, bool compressed = true)
        {
            if (compressed)
            {
                using (var fs = System.IO.File.OpenRead(path))
                {
                    using (var cs = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
                    {
                        using (var sr = new System.IO.StreamReader(cs, Encoding.UTF8))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            else
            {
                using (var fs = System.IO.File.OpenRead(path))
                {
                    using (var sr = new System.IO.StreamReader(fs, Encoding.UTF8))
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
                using (var fs = System.IO.File.Create(path))
                {
                    using (var cs = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Compress))
                    {
                        using (var sw = new System.IO.StreamWriter(cs, Encoding.UTF8))
                        {
                            sw.Write(text);
                        }
                    }
                }
            }
            else
            {
                using (var fs = System.IO.File.Create(path))
                {
                    using (var sw = new System.IO.StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(text);
                    }
                }
            }
        }
    }
}
