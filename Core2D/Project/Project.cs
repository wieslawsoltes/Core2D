// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Core2D
{
    /// <summary>
    /// The <see cref="IImageCache"/> implementation for <see cref="Project"/> class.
    /// </summary>
    public partial class Project : ObservableObject, IImageCache
    {
        private static string ProjectEntryName = "Project.json";
        private static string ImageEntryNamePrefix = "Images\\";
        private IDictionary<string, byte[]> _images = new Dictionary<string, byte[]>();

        /// <inheritdoc/>
        public IEnumerable<ImageKey> Keys
        {
            get
            {
                return _images.Select(i => new ImageKey() { Key = i.Key }).ToList();
            }
        }

        /// <inheritdoc/>
        public string AddImageFromFile(string path, byte[] bytes)
        {
            var name = System.IO.Path.GetFileName(path);
            var key = ImageEntryNamePrefix + name;

            if (_images.Keys.Contains(key))
                return key;

            _images.Add(key, bytes);
            Notify("Keys");
            return key;
        }

        /// <inheritdoc/>
        public void AddImage(string key, byte[] bytes)
        {
            if (_images.Keys.Contains(key))
                return;

            _images.Add(key, bytes);
            Notify("Keys");
        }

        /// <inheritdoc/>
        public byte[] GetImage(string key)
        {
            byte[] bytes;
            if (_images.TryGetValue(key, out bytes))
                return bytes;
            else
                return null;
        }

        /// <inheritdoc/>
        public void RemoveImage(string key)
        {
            _images.Remove(key);
            Notify("Keys");
        }

        /// <inheritdoc/>
        public void PurgeUnusedImages(ICollection<string> used)
        {
            foreach (var kvp in _images.ToList())
            {
                if (!used.Contains(kvp.Key))
                {
                    _images.Remove(kvp.Key);
                }
            }
            Notify("Keys");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Project : ObservableObject
    {
        private string _name;
        private Options _options;
        private History _history;
        private ImmutableArray<Database> _databases;
        private Database _currentDatabase;
        private ImmutableArray<Library<ShapeStyle>> _styleLibraries;
        private Library<ShapeStyle> _currentStyleLibrary;
        private ImmutableArray<Library<XGroup>> _groupLibraries;
        private Library<XGroup> _currentGroupLibrary;
        private ImmutableArray<Container> _templates;
        private Container _currentTemplate;
        private ImmutableArray<Document> _documents;
        private Document _currentDocument;
        private Container _currentContainer;
        private object _selected;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Options Options
        {
            get { return _options; }
            set { Update(ref _options, value); }
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public History History
        {
            get { return _history; }
            set { Update(ref _history, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Database CurrentDatabase
        {
            get { return _currentDatabase; }
            set { Update(ref _currentDatabase, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Database> Databases
        {
            get { return _databases; }
            set { Update(ref _databases, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Library<ShapeStyle>> StyleLibraries
        {
            get { return _styleLibraries; }
            set { Update(ref _styleLibraries, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Library<ShapeStyle> CurrentStyleLibrary
        {
            get { return _currentStyleLibrary; }
            set { Update(ref _currentStyleLibrary, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Library<XGroup>> GroupLibraries
        {
            get { return _groupLibraries; }
            set { Update(ref _groupLibraries, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Library<XGroup> CurrentGroupLibrary
        {
            get { return _currentGroupLibrary; }
            set { Update(ref _currentGroupLibrary, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Container> Templates
        {
            get { return _templates; }
            set { Update(ref _templates, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container CurrentTemplate
        {
            get { return _currentTemplate; }
            set { Update(ref _currentTemplate, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Document> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Document CurrentDocument
        {
            get { return _currentDocument; }
            set { Update(ref _currentDocument, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container CurrentContainer
        {
            get { return _currentContainer; }
            set { Update(ref _currentContainer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Selected
        {
            get { return _selected; }
            set
            {
                var item = value;

                if (item is Container && _documents != null)
                {
                    var container = item as Container;
                    var document = _documents.FirstOrDefault(d => d.Containers.Contains(container));
                    if (document != null)
                    {
                        CurrentDocument = document;
                        CurrentContainer = container;
                        CurrentContainer.Invalidate();
                    }
                }
                else if (item is Document)
                {
                    var document = item as Document;
                    CurrentDocument = document;
                }

                Update(ref _selected, value);
            }
        }

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
        
        /// <summary>
        /// Creates a new <see cref="Project"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Project Create(string name = "Project")
        {
            var p = new Project()
            {
                Name = name,
                Options = Options.Create(),
                Databases = ImmutableArray.Create<Database>(),
                StyleLibraries = ImmutableArray.Create<Library<ShapeStyle>>(),
                GroupLibraries = ImmutableArray.Create<Library<XGroup>>(),
                Templates = ImmutableArray.Create<Container>(),
                Documents = ImmutableArray.Create<Document>(),
            };
            return p;
        }
    }
}
