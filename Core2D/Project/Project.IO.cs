// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// Project input/output.
    /// </summary>
    public partial class Project
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Project Open(string path, ISerializer serializer)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || serializer == null)
                return null;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                return Open(stream, serializer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Project Open(Stream stream, ISerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var projectEntry = archive.Entries.FirstOrDefault(e => e.FullName == ProjectEntryName);
                if (projectEntry == null)
                    return null;

                var project = default(Project);

                // First step is to read project entry and deserialize project object.
                using (var entryStream = projectEntry.Open())
                {
                    string json = ReadUtf8Text(entryStream);
                    project = serializer.Deserialize<Project>(json);
                }

                // Second step is to read (if any) project images.
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.StartsWith(ImageEntryNamePrefix))
                    {
                        using (var entryStream = entry.Open())
                        {
                            var bytes = ReadBinary(entryStream);
                            project.AddImage(entry.FullName, bytes);
                        }
                    }
                    else
                    {
                        // Ignore all other entries.
                    }
                }

                return project;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="path"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static void Save(Project project, string path, ISerializer serializer)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                Save(project, stream, serializer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="stream"></param>
        /// <param name="serializer"></param>
        public static void Save(Project project, Stream stream, ISerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                // First step is to write project entry.
                var jsonEntry = archive.CreateEntry(ProjectEntryName);
                using (var jsonStream = jsonEntry.Open())
                {
                    var json = serializer.Serialize(project);
                    WriteUtf8Text(jsonStream, json);
                }

                // Second step is to write (if any) project images.
                var keys = Editor.GetAllShapes<XImage>(project).Select(i => i.Key).Distinct();
                foreach (var key in keys)
                {
                    var bytes = project.GetImage(key);
                    if (bytes != null)
                    {
                        var imageEntry = archive.CreateEntry(key);
                        using (var imageStream = imageEntry.Open())
                        {
                            WriteBinary(imageStream, bytes);
                        }
                    }
                }

                // NOTE: Purge deleted images from memory is not called here to enable Undo/Redo.
                //project.PurgeUnusedImages(new HashSet<string>(keys));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBinary(Stream stream)
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
        public static void WriteBinary(Stream stream, byte[] bytes)
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
        public static string ReadUtf8Text(Stream stream)
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
        public static void WriteUtf8Text(Stream stream, string text)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadUtf8Text(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public static void WriteUtf8Text(string path, string text)
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
