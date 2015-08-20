// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectFile : ObservableObject
    {
        private string _path;
        private string _json;
        private IDictionary<string, byte[]> _images;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Json
        {
            get { return _json; }
            set { Update(ref _json, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, byte[]> Images
        {
            get { return _images; }
            set { Update(ref _images, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ProjectEntryName = "Project.json";

        /// <summary>
        /// 
        /// </summary>
        public static string ImageEntryNamePrefix = "Images\\";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ProjectFile Open(string path)
        {
            if (File.Exists(path))
            {
                string json = null;
                var images = new Dictionary<string, byte[]>();

                using (var fs = new FileStream(path, FileMode.Open))
                {
                    using (var archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (entry.FullName == ProjectEntryName)
                            {
                                using (var stream = entry.Open())
                                {
                                    json = ReadText(stream);
                                }
                            }
                            else if (entry.FullName.StartsWith(ImageEntryNamePrefix))
                            {
                                using (var stream = entry.Open())
                                {
                                    var bytes = ReadBinary(stream);
                                    images.Add(entry.Name, bytes);
                                }
                            }
                            else
                            {
                                // NOTE: Ignore other entries.
                            }
                        }

                        return new ProjectFile()
                        {

                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static void Save(string path, ProjectFile file)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    var jsonEntry = archive.CreateEntry(ProjectEntryName);
                    using (var jsonStream = jsonEntry.Open())
                    {
                        WriteText(jsonStream, file.Json);
                    }

                    foreach (var image in file.Images)
                    {
                        var imageEntry = archive.CreateEntry(image.Key);
                        using (var imageStream = imageEntry.Open())
                        {
                            WriteBinary(imageStream, image.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string AddImage(string path, byte[] bytes)
        {
            var name = System.IO.Path.GetFileName(path);
            var key = ImageEntryNamePrefix + name;
            _images.Add(key, bytes);
            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void RemoveImage(string key)
        {
            _images.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] ReadBinary(Stream stream)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytes"></param>
        private static void WriteBinary(Stream stream, byte[] bytes)
        {
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(bytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string ReadText(Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="text"></param>
        private static void WriteText(Stream stream, string text)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(text);
            }
        }
    }
}
