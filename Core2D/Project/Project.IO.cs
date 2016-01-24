// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Project input/output.
    /// </summary>
    public partial class Project
    {
        /// <summary>
        /// Project Json data entry name.
        /// </summary>
        public const string ProjectEntryName = "Project.json";

        /// <summary>
        /// Image Key prefix entry name.
        /// </summary>
        public const string ImageEntryNamePrefix = "Images\\";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileIO"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Project Open(string path, IFileSystem fileIO, ISerializer serializer)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || fileIO == null || serializer == null)
                return null;

            using (var stream = fileIO.Open(path))
            {
                return Open(stream, fileIO, serializer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="path"></param>
        /// <param name="fileIO"></param>
        /// <param name="serializer"></param>
        public static void Save(Project project, string path, IFileSystem fileIO, ISerializer serializer)
        {
            using (var stream = fileIO.Create(path))
            {
                Save(project, stream, fileIO, serializer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileIO"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        private static Project Open(Stream stream, IFileSystem fileIO, ISerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var projectEntry = archive.Entries.FirstOrDefault(e => e.FullName == ProjectEntryName);
                if (projectEntry == null)
                    return null;

                var project = default(Project);

                // Read project entry and deserialize from json.
                using (var entryStream = projectEntry.Open())
                {
                    string json = fileIO.ReadUtf8Text(entryStream);
                    project = serializer.Deserialize<Project>(json);
                }

                // Read image entries.
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.StartsWith(ImageEntryNamePrefix))
                    {
                        using (var entryStream = entry.Open())
                        {
                            var bytes = fileIO.ReadBinary(entryStream);
                            project.AddImage(entry.FullName, bytes);
                        }
                    }
                }

                return project;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="stream"></param>
        /// <param name="fileIO"></param>
        /// <param name="serializer"></param>
        private static void Save(Project project, Stream stream, IFileSystem fileIO, ISerializer serializer)
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                // Write project as json.
                var jsonEntry = archive.CreateEntry(ProjectEntryName);
                using (var jsonStream = jsonEntry.Open())
                {
                    fileIO.WriteUtf8Text(jsonStream, serializer.Serialize(project));
                }

                // Get only used image keys so unused image data is removed when saving project to a file.
                var keys = Editor.GetAllShapes<XImage>(project).Select(i => i.Key).Distinct();

                // Write only used project images.
                foreach (var key in keys)
                {
                    var imageEntry = archive.CreateEntry(key);
                    using (var imageStream = imageEntry.Open())
                    {
                        fileIO.WriteBinary(imageStream, project.GetImage(key));
                    }
                }
            }
        }
    }
}
