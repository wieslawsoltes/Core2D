// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Project editor.
    /// </summary>
    public class Editor : ObservableObject, IDisposable
    {
        private ILog _log;
        private Project _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private Renderer[] _renderers;
        private Tool _currentTool;
        private PathTool _currentPathTool;
        private Observer _observer;
        private Action _invalidate;
        private Action _resetZoom;
        private Action _extentZoom;
        private bool _cancelAvailable;
        private BaseShape _hover;
        private IView _view;
        private IProjectFactory _projectFactory;
        private ITextClipboard _textClipboard;
        private ISerializer _serializer;
        private IFileWriter _pdfWriter;
        private IFileWriter _dxfWriter;
        private ITextFieldReader<Database> _csvReader;
        private ITextFieldWriter<Database> _csvWriter;
        private ImmutableArray<RecentProject> _recentProjects = ImmutableArray.Create<RecentProject>();
        private RecentProject _currentRecentProject = default(RecentProject);
        private Container _containerToCopy = default(Container);
        private Document _documentToCopy = default(Document);

        /// <summary>
        /// Gets or sets current log.
        /// </summary>
        public ILog Log
        {
            get { return _log; }
            set { Update(ref _log, value); }
        }

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        public Project Project
        {
            get { return _project; }
            set { Update(ref _project, value); }
        }

        /// <summary>
        /// Gets or sets current project path.
        /// </summary>
        public string ProjectPath
        {
            get { return _projectPath; }
            set { Update(ref _projectPath, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating that current project was modified.
        /// </summary>
        public bool IsProjectDirty
        {
            get { return _isProjectDirty; }
            set { Update(ref _isProjectDirty, value); }
        }

        /// <summary>
        /// Gets or sets current renderer's.
        /// </summary>
        public Renderer[] Renderers
        {
            get { return _renderers; }
            set { Update(ref _renderers, value); }
        }

        /// <summary>
        /// Gets or sets current editor tool.
        /// </summary>
        public Tool CurrentTool
        {
            get { return _currentTool; }
            set { Update(ref _currentTool, value); }
        }

        /// <summary>
        /// Gets or sets current editor path tool.
        /// </summary>
        public PathTool CurrentPathTool
        {
            get { return _currentPathTool; }
            set { Update(ref _currentPathTool, value); }
        }

        /// <summary>
        /// Gets or sets current project collections and objects observer.
        /// </summary>
        public Observer Observer
        {
            get { return _observer; }
            set { Update(ref _observer, value); }
        }

        /// <summary>
        /// Gets or sets invalidate action.
        /// </summary>
        public Action Invalidate
        {
            get { return _invalidate; }
            set { Update(ref _invalidate, value); }
        }

        /// <summary>
        /// Gets or sets reset zoom action.
        /// </summary>
        public Action ResetZoom
        {
            get { return _resetZoom; }
            set { Update(ref _resetZoom, value); }
        }

        /// <summary>
        /// Gets or sets extent zoom action.
        /// </summary>
        public Action ExtentZoom
        {
            get { return _extentZoom; }
            set { Update(ref _extentZoom, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating that current operation can be canceled.
        /// </summary>
        public bool CancelAvailable
        {
            get { return _cancelAvailable; }
            set { Update(ref _cancelAvailable, value); }
        }

        /// <summary>
        /// Get image key using common system open file dialog.
        /// </summary>
        public Func<Task<string>> GetImageKey { get; set; }

        /// <summary>
        /// Gets or sets editor tool helpers dictionary.
        /// </summary>
        public ImmutableDictionary<Tool, ToolBase> Tools { get; set; }

        /// <summary>
        /// Gets or sets editor view.
        /// </summary>
        public IView View
        {
            get { return _view; }
            set { Update(ref _view, value); }
        }

        /// <summary>
        /// Gets or sets project factory.
        /// </summary>
        public IProjectFactory ProjectFactory
        {
            get { return _projectFactory; }
            set { Update(ref _projectFactory, value); }
        }

        /// <summary>
        /// Gets or sets text clipboard.
        /// </summary>
        public ITextClipboard TextClipboard
        {
            get { return _textClipboard; }
            set { Update(ref _textClipboard, value); }
        }

        /// <summary>
        /// Gets or sets object serializer.
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
            set { Update(ref _serializer, value); }
        }

        /// <summary>
        /// Gets or sets Pdf file writer.
        /// </summary>
        public IFileWriter PdfWriter
        {
            get { return _pdfWriter; }
            set { Update(ref _pdfWriter, value); }
        }

        /// <summary>
        /// Gets or sets Dxf file writer.
        /// </summary>
        public IFileWriter DxfWriter
        {
            get { return _dxfWriter; }
            set { Update(ref _dxfWriter, value); }
        }

        /// <summary>
        /// Gets or sets Csv file reader.
        /// </summary>
        public ITextFieldReader<Database> CsvReader
        {
            get { return _csvReader; }
            set { Update(ref _csvReader, value); }
        }

        /// <summary>
        /// Gets or sets Csv file writer.
        /// </summary>
        public ITextFieldWriter<Database> CsvWriter
        {
            get { return _csvWriter; }
            set { Update(ref _csvWriter, value); }
        }

        /// <summary>
        /// Gets or sets recent projects collection.
        /// </summary>
        public ImmutableArray<RecentProject> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }

        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        public RecentProject CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }

        /// <summary>
        /// Create new project, document or container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                var document = _project.Documents.FirstOrDefault(d => d.Containers.Contains(selected));
                if (document != null)
                {
                    var container = default(Container);
                    if (_projectFactory != null)
                    {
                        container = _projectFactory.GetContainer(_project, Constants.DefaultContainerName);
                    }
                    else
                    {
                        container = Container.Create(Constants.DefaultContainerName);
                    }

                    var previous = document.Containers;
                    var next = document.Containers.Add(container);
                    _project.History.Snapshot(previous, next, (p) => document.Containers = p);
                    document.Containers = next;

                    _project.CurrentContainer = container;
                }
            }
            else if (item is Document)
            {
                var selected = item as Document;

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                var previous = selected.Containers;
                var next = selected.Containers.Add(container);
                _project.History.Snapshot(previous, next, (p) => selected.Containers = p);
                selected.Containers = next;

                _project.CurrentContainer = container;
            }
            else if (item is Project)
            {
                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                var previous = _project.Documents;
                var next = _project.Documents.Add(document);
                _project.History.Snapshot(previous, next, (p) => _project.Documents = p);
                _project.Documents = next;

                _project.CurrentDocument = document;
                _project.CurrentContainer = document.Containers.FirstOrDefault();
            }
            else if (item is Editor || item == null)
            {
                Unload();

                if (_projectFactory != null)
                {
                    Load(_projectFactory.GetProject(), string.Empty);
                }
                else
                {
                    Load(Project.Create(), string.Empty);
                }

                if (Invalidate != null)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Open(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var project = Project.Open(path, _serializer);

                Unload();
                Load(project, path);

                AddRecent(path, project.Name);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void OnClose()
        {
            _project.History.Reset();
            Unload();
        }

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Save(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                Project.Save(_project, path, _serializer);

                AddRecent(path, _project.Name);

                if (string.IsNullOrEmpty(_projectPath))
                {
                    _projectPath = path;
                }

                IsProjectDirty = false;
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Close application view.
        /// </summary>
        public void OnExit()
        {
            if (_view != null)
            {
                _view.Close();
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        public void OnImportData(string path)
        {
            if (_project == null)
                return;

            try
            {
                if (_csvReader == null)
                    return;

                var db = _csvReader.Read(path);
                _project.AddDatabase(db);
                _project.CurrentDatabase = db;
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnExportData(string path, Database database)
        {
            try
            {
                if (_csvWriter == null)
                    return;

                _csvWriter.Write(path, database);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnUpdateData(string path, Database database)
        {
            try
            {
                if (_csvReader == null)
                    return;

                var db = _csvReader.Read(path);
                _project.ApplyDatabase(database, db);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Import object from file.
        /// </summary>
        /// <param name="path">The object file path.</param>
        /// <param name="item">The parent object.</param>
        /// <param name="type">The object type.</param>
        public void OnImportObject(string path, object item, ImportType type)
        {
            if (_serializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ImportType.Style:
                        {
                            var sg = item as Library<ShapeStyle>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<ShapeStyle>(json);

                            var previous = sg.Items;
                            var next = sg.Items.Add(import);
                            _project.History.Snapshot(previous, next, (p) => sg.Items = p);
                            sg.Items = next;
                        }
                        break;
                    case ImportType.Styles:
                        {
                            var sg = item as Library<ShapeStyle>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<ShapeStyle>>(json);

                            var builder = sg.Items.ToBuilder();
                            foreach (var style in import)
                            {
                                builder.Add(style);
                            }

                            var previous = sg.Items;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => sg.Items = p);
                            sg.Items = next;
                        }
                        break;
                    case ImportType.StyleLibrary:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Library<ShapeStyle>>(json);

                            var previous = project.StyleLibraries;
                            var next = project.StyleLibraries.Add(import);
                            _project.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                            project.StyleLibraries = next;
                        }
                        break;
                    case ImportType.StyleLibraries:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Library<ShapeStyle>>>(json);

                            var builder = project.StyleLibraries.ToBuilder();
                            foreach (var sg in import)
                            {
                                builder.Add(sg);
                            }

                            var previous = project.StyleLibraries;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                            project.StyleLibraries = next;
                        }
                        break;
                    case ImportType.Group:
                        {
                            var gl = item as Library<XGroup>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<XGroup>(json);

                            var shapes = Enumerable.Repeat(import as XGroup, 1);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var previous = gl.Items;
                            var next = gl.Items.Add(import);
                            _project.History.Snapshot(previous, next, (p) => gl.Items = p);
                            gl.Items = next;
                        }
                        break;
                    case ImportType.Groups:
                        {
                            var gl = item as Library<XGroup>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<XGroup>>(json);

                            var shapes = import;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = gl.Items.ToBuilder();
                            foreach (var group in import)
                            {
                                builder.Add(group);
                            }

                            var previous = gl.Items;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => gl.Items = p);
                            gl.Items = next;
                        }
                        break;
                    case ImportType.GroupLibrary:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Library<XGroup>>(json);

                            var shapes = import.Items;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var previous = project.GroupLibraries;
                            var next = project.GroupLibraries.Add(import);
                            _project.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                            project.GroupLibraries = next;
                        }
                        break;
                    case ImportType.GroupLibraries:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Library<XGroup>>>(json);

                            var shapes = import.SelectMany(x => x.Items);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = project.GroupLibraries.ToBuilder();
                            foreach (var library in import)
                            {
                                builder.Add(library);
                            }

                            var previous = project.GroupLibraries;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                            project.GroupLibraries = next;
                        }
                        break;
                    case ImportType.Template:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Container>(json);

                            var shapes = import.Layers.SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var previous = project.Templates;
                            var next = project.Templates.Add(import);
                            _project.History.Snapshot(previous, next, (p) => project.Templates = p);
                            project.Templates = next;
                        }
                        break;
                    case ImportType.Templates:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Container>>(json);

                            var shapes = import.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = project.Templates.ToBuilder();
                            foreach (var template in import)
                            {
                                builder.Add(template);
                            }

                            var previous = project.Templates;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => project.Templates = p);
                            project.Templates = next;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export object to a file.
        /// </summary>
        /// <param name="path">The object file path.</param>
        /// <param name="item">The parent object.</param>
        /// <param name="type">The object type.</param>
        public void OnExportObject(string path, object item, ExportType type)
        {
            if (_serializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ExportType.Style:
                        {
                            var json = _serializer.Serialize(item as ShapeStyle);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Styles:
                        {
                            var json = _serializer.Serialize((item as Library<ShapeStyle>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibrary:
                        {
                            var json = _serializer.Serialize((item as Library<ShapeStyle>));
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).StyleLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Group:
                        {
                            var json = _serializer.Serialize(item as XGroup);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Groups:
                        {
                            var json = _serializer.Serialize((item as Library<XGroup>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibrary:
                        {
                            var json = _serializer.Serialize(item as Library<XGroup>);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).GroupLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Template:
                        {
                            var json = _serializer.Serialize(item as Container);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Templates:
                        {
                            var json = _serializer.Serialize((item as Project).Templates);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public void OnUndo()
        {
            try
            {
                if (_project.History.CanUndo())
                {
                    Deselect();
                    _project.History.Undo();
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public void OnRedo()
        {
            try
            {
                if (_project.History.CanRedo())
                {
                    Deselect();
                    _project.History.Redo();
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Cut selected document, container or shapes to clipboard.
        /// </summary>
        public void OnCut()
        {
            try
            {
                if (CanCopy())
                {
                    OnCopy();

                    DeleteSelected();
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Copy document, container or shapes to clipboard.
        /// </summary>
        public void OnCopy()
        {
            try
            {
                if (CanCopy())
                {
                    if (_renderers[0].State.SelectedShape != null)
                    {
                        Copy(Enumerable.Repeat(_renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (_renderers[0].State.SelectedShapes != null)
                    {
                        Copy(_renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, container or shapes.
        /// </summary>
        public async void OnPaste()
        {
            try
            {
                if (_textClipboard != null && await CanPaste())
                {
                    var text = await _textClipboard.GetText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        Paste(text);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Cut selected document, container or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
                _project.RemoveContainer(container);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
                _project.RemoveDocument(document);
            }
            else if (item is Editor || item == null)
            {
                OnCut();
            }
        }

        /// <summary>
        /// Copy document, container or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        public void OnCopy(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
            }
            else if (item is Editor || item == null)
            {
                OnCopy();
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, container or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        public void OnPaste(object item)
        {
            if (item is Container)
            {
                if (_containerToCopy != null)
                {
                    var container = item as Container;
                    var document = _project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
                    if (document != null)
                    {
                        int index = document.Containers.IndexOf(container);
                        var clone = Clone(_containerToCopy);

                        var builder = document.Containers.ToBuilder();
                        builder[index] = clone;
                        document.Containers = builder.ToImmutable();

                        var previous = document.Containers;
                        var next = builder.ToImmutable();
                        _project.History.Snapshot(previous, next, (p) => document.Containers = p);
                        document.Containers = next;

                        _project.CurrentContainer = clone;
                    }
                }
            }
            else if (item is Document)
            {
                if (_containerToCopy != null)
                {
                    var document = item as Document;
                    var clone = Clone(_containerToCopy);

                    var previous = document.Containers;
                    var next = document.Containers.Add(clone);
                    _project.History.Snapshot(previous, next, (p) => document.Containers = p);
                    document.Containers = next;

                    _project.CurrentContainer = clone;
                }
                else if (_documentToCopy != null)
                {
                    var document = item as Document;
                    int index = _project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);

                    var builder = _project.Documents.ToBuilder();
                    builder[index] = clone;

                    var previous = _project.Documents;
                    var next = builder.ToImmutable();
                    _project.History.Snapshot(previous, next, (p) => _project.Documents = p);
                    _project.Documents = next;

                    _project.CurrentDocument = clone;
                }
            }
            else if (item is Editor || item == null)
            {
                OnPaste();
            }
        }

        /// <summary>
        /// Delete selected document, container or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is Container)
            {
                _project.RemoveContainer(item as Container);
            }
            else if (item is Document)
            {
                _project.RemoveDocument(item as Document);
            }
            else if (item is Editor || item == null)
            {
                DeleteSelected();
            }
        }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public void OnSelectAll()
        {
            try
            {
                Deselect(_project.CurrentContainer);
                Select(
                    _project.CurrentContainer,
                    ImmutableHashSet.CreateRange<BaseShape>(_project.CurrentContainer.CurrentLayer.Shapes));
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public void OnDeselectAll()
        {
            try
            {
                Deselect(_project.CurrentContainer);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public void OnClearAll()
        {
            try
            {
                // TODO: Add history snapshot.
                _project.CurrentContainer.Clear();
                _project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnGroupSelected()
        {
            var group = _project.Group(_renderers[0].State.SelectedShapes);
            if (group != null)
            {
                Select(_project.CurrentContainer, group);
            }
        }
 
        /// <summary>
        /// 
        /// </summary>
        public void OnUngroupSelected()
        {
            var result = _project.Ungroup(_renderers[0].State.SelectedShape, _renderers[0].State.SelectedShapes);
            if (result == true)
            {
                _renderers[0].State.SelectedShape = null;
                _renderers[0].State.SelectedShapes = null;
            }
        }

        /// <summary>
        /// Bring selected shapes to the top of the stack.
        /// </summary>
        public void OnBringToFrontSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                _project.BringToFront(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    _project.BringToFront(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes one step closer to the front of the stack.
        /// </summary>
        public void OnBringForwardSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                _project.BringForward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    _project.BringForward(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes one step down within the stack.
        /// </summary>
        public void OnSendBackwardSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                _project.SendBackward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    _project.SendBackward(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes to the bottom of the stack.
        /// </summary>
        public void OnSendToBackSelected()
        {
            var source = _renderers[0].State.SelectedShape;
            if (source != null)
            {
                _project.SendToBack(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    _project.SendToBack(s);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnMoveUpSelected()
        {
            _project.MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? -_project.Options.SnapY : -1.0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnMoveDownSelected()
        {
            _project.MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? _project.Options.SnapY : 1.0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnMoveLeftSelected()
        {
            _project.MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                _project.Options.SnapToGrid ? -_project.Options.SnapX : -1.0,
                0.0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnMoveRightSelected()
        {
            _project.MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                _project.Options.SnapToGrid ? _project.Options.SnapX : 1.0,
                0.0);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.None"/>.
        /// </summary>
        public void OnToolNone()
        {
            CurrentTool = Tool.None;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Selection"/>.
        /// </summary>
        public void OnToolSelection()
        {
            CurrentTool = Tool.Selection;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Point"/>.
        /// </summary>
        public void OnToolPoint()
        {
            CurrentTool = Tool.Point;
        }

        /// <summary>
        ///  Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
        /// </summary>
        public void OnToolLine()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.Line)
            {
                CurrentPathTool = PathTool.Line;
            }
            else
            {
                CurrentTool = Tool.Line;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Arc"/> or current path tool to <see cref="PathTool.Arc"/>.
        /// </summary>
        public void OnToolArc()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.Arc)
            {
                CurrentPathTool = PathTool.Arc;
            }
            else
            {
                CurrentTool = Tool.Arc;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Bezier"/> or current path tool to <see cref="PathTool.Bezier"/>.
        /// </summary>
        public void OnToolBezier()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.Bezier)
            {
                CurrentPathTool = PathTool.Bezier;
            }
            else
            {
                CurrentTool = Tool.Bezier;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.QBezier"/> or current path tool to <see cref="PathTool.QBezier"/>.
        /// </summary>
        public void OnToolQBezier()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.QBezier)
            {
                CurrentPathTool = PathTool.QBezier;
            }
            else
            {
                CurrentTool = Tool.QBezier;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Path"/>.
        /// </summary>
        public void OnToolPath()
        {
            CurrentTool = Tool.Path;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Rectangle"/>.
        /// </summary>
        public void OnToolRectangle()
        {
            CurrentTool = Tool.Rectangle;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Ellipse"/>.
        /// </summary>
        public void OnToolEllipse()
        {
            CurrentTool = Tool.Ellipse;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Text"/>.
        /// </summary>
        public void OnToolText()
        {
            CurrentTool = Tool.Text;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Image"/>.
        /// </summary>
        public void OnToolImage()
        {
            CurrentTool = Tool.Image;
        }

        /// <summary>
        /// Set current path tool to <see cref="PathTool.Move"/>.
        /// </summary>
        public void OnToolMove()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.Move)
            {
                CurrentPathTool = PathTool.Move;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.DefaultIsStroked = !_project.Options.DefaultIsStroked;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.DefaultIsFilled = !_project.Options.DefaultIsFilled;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.DefaultIsClosed = !_project.Options.DefaultIsClosed;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.DefaultIsSmoothJoin = !_project.Options.DefaultIsSmoothJoin;
        }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.SnapToGrid = !_project.Options.SnapToGrid;
        }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (_project == null || _project.Options == null)
                return;

            _project.Options.TryToConnect = !_project.Options.TryToConnect;
        }

        /// <summary>
        /// Set current record as selected shape data record.
        /// </summary>
        /// <param name="record">The data record item.</param>
        public void OnApplyRecord(Record record)
        {
            if (_project == null)
                return;

            if (record != null)
            {
                _project.ApplyRecord(
                    record as Record,
                    _renderers[0].State.SelectedShape,
                    _renderers[0].State.SelectedShapes);
            }
        }

        /// <summary>
        /// Add group.
        /// </summary>
        public void OnAddGroup()
        {
            if (_renderers != null && _project == null || _project.CurrentGroupLibrary == null)
                return;

            var group = _renderers[0].State.SelectedShape;
            if (group != null && group is XGroup)
            {
                var clone = Clone(group as XGroup);
                if (clone != null)
                {
                    _project.AddGroup(clone);
                }
            }
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        public void OnRemoveGroup()
        {
            _project.RemoveGroup();
        }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="group">The group instance.</param>
        public void OnInsertGroup(XGroup group)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            DropAsClone(group, 0.0, 0.0);
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        public void OnApplyStyle(ShapeStyle style)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            _project.ApplyStyle(
                style,
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes);
        }

        /// <summary>
        /// Add template.
        /// </summary>
        public void OnAddTemplate()
        {
            if (_project == null)
                return;

            var template = default(Container);
            if (_projectFactory != null)
            {
                template = _projectFactory.GetTemplate(_project, "Empty");
            }
            else
            {
                template = Container.Create(Constants.DefaultContainerName, true);
            }

            _project.AddTemplate(template);
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        public void OnRemoveTemplate()
        {
            _project.RemoveTemplate();
        }

        /// <summary>
        /// Edit current template.
        /// </summary>
        public void OnEditTemplate()
        {
            if (_project == null || _project.CurrentTemplate == null)
                return;

            var template = _project.CurrentTemplate;
            if (template != null)
            {
                _project.CurrentContainer = template;
                _project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Set current template as current container's template.
        /// </summary>
        /// <param name="container">The container item.</param>
        public void OnApplyTemplate(Container container)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            if (container != null)
            {
                _project.ApplyTemplate(container);
            }
        }

        /// <summary>
        /// Add image key.
        /// </summary>
        /// <param name="path">The image path.</param>
        public async Task<string> OnAddImageKey(string path)
        {
            if (_project == null)
                return null;

            if (path == null || string.IsNullOrEmpty(path))
            {
                var key = await GetImageKey();
                if (key == null || string.IsNullOrEmpty(key))
                    return null;

                return key;
            }
            else
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                var key = _project.AddImageFromFile(path, bytes);
                return key;
            }
        }

        /// <summary>
        /// Remove image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        public void OnRemoveImageKey(string key)
        {
            if (_project == null)
                return;

            if (key != null)
            {
                _project.RemoveImage(key);
            }
        }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnSelectedItemChanged(object item)
        {
            if (_project == null)
                return;

            _project.Selected = item;
        }

        /// <summary>
        /// Add container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddContainer(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            var container = default(Container);
            if (_projectFactory != null)
            {
                container = _projectFactory.GetContainer(_project, Constants.DefaultContainerName);
            }
            else
            {
                container = Container.Create(Constants.DefaultContainerName);
            }

            _project.AddContainer(container);
            _project.CurrentContainer = container;
        }

        /// <summary>
        /// Insert container before current container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertContainerBefore(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            if (item is Container)
            {
                var selected = item as Container;
                int index = _project.CurrentDocument.Containers.IndexOf(selected);

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                _project.AddContainerAt(container, index);
                _project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// Insert container after current container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertContainerAfter(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            if (item is Container)
            {
                var selected = item as Container;
                int index = _project.CurrentDocument.Containers.IndexOf(selected);

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                _project.AddContainerAt(container, index + 1);
                _project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddDocument(object item)
        {
            if (_project == null)
                return;

            var document = default(Document);
            if (_projectFactory != null)
            {
                document = _projectFactory.GetDocument(_project, Constants.DefaultDocumentName);
            }
            else
            {
                document = Document.Create(Constants.DefaultDocumentName);
            }

            _project.AddDocument(document);
            _project.CurrentDocument = document;
            _project.CurrentContainer = document.Containers.FirstOrDefault();
        }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentBefore(object item)
        {
            if (_project == null)
                return;

            if (item is Document)
            {
                var selected = item as Document;
                int index = _project.Documents.IndexOf(selected);

                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                _project.AddDocumentAt(document, index);
                _project.CurrentDocument = document;
                _project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentAfter(object item)
        {
            if (_project == null)
                return;

            if (item is Document)
            {
                var selected = item as Document;
                int index = _project.Documents.IndexOf(selected);

                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                _project.AddDocumentAt(document, index + 1);
                _project.CurrentDocument = document;
                _project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Initialize default <see cref="Editor"/> tools.
        /// </summary>
        public void DefaultTools()
        {
            Tools = new Dictionary<Tool, ToolBase>
            {
                { Tool.None,      new ToolNone(this)      },
                { Tool.Selection, new ToolSelection(this) },
                { Tool.Point,     new ToolPoint(this)     },
                { Tool.Line,      new ToolLine(this)      },
                { Tool.Arc,       new ToolArc(this)       },
                { Tool.Bezier,    new ToolBezier(this)    },
                { Tool.QBezier,   new ToolQBezier(this)   },
                { Tool.Path,      new ToolPath(this)      },
                { Tool.Rectangle, new ToolRectangle(this) },
                { Tool.Ellipse,   new ToolEllipse(this)   },
                { Tool.Text,      new ToolText(this)      },
                { Tool.Image,     new ToolImage(this)     }
            }.ToImmutableDictionary();
        }

        /// <summary>
        /// Set renderer's image cache.
        /// </summary>
        /// <param name="cache">The image cache instance.</param>
        private void SetRenderersImageCache(IImageCache cache)
        {
            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.ClearCache(isZooming: false);
                    renderer.State.ImageCache = cache;
                }
            }
        }

        /// <summary>
        /// Load project.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The project path.</param>
        public void Load(Project project, string path = null)
        {
            Deselect();
            SetRenderersImageCache(project);

            Project = project;
            Project.History = new History();
            ProjectPath = path;
            IsProjectDirty = false;
            Observer = new Observer(this);
        }

        /// <summary>
        /// Unload project.
        /// </summary>
        public void Unload()
        {
            if (_project != null)
            {
                if (Observer != null)
                {
                    Observer.Dispose();
                    Observer = null;
                }

                if (_project.History != null)
                {
                    _project.History.Reset();
                    _project.History = null;
                }

                _project.PurgeUnusedImages(Enumerable.Empty<string>().ToImmutableHashSet());
            }

            Deselect();
            SetRenderersImageCache(null);

            Project = null;
            ProjectPath = string.Empty;
            IsProjectDirty = false;
        }

        /// <summary>
        /// Snap value by specified snap amount.
        /// </summary>
        /// <param name="value">The value to snap.</param>
        /// <param name="snap">The snap amount.</param>
        /// <returns>The snapped value.</returns>
        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        /// <summary>
        /// Get all shapes including grouped shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <returns>All shapes including grouped shapes.</returns>
        public static IEnumerable<BaseShape> GetAllShapes(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                yield break;

            foreach (var shape in shapes)
            {
                if (shape is XGroup)
                {
                    foreach (var s in GetAllShapes((shape as XGroup).Shapes))
                    {
                        yield return s;
                    }

                    yield return shape;
                }
                else
                {
                    yield return shape;
                }
            }
        }

        /// <summary>
        /// Get all shapes including grouped shapes of specified type.
        /// </summary>
        /// <typeparam name="T">The type of shape to include.</typeparam>
        /// <param name="shapes">The shapes collection.</param>
        /// <returns>All shapes including grouped shapes of specified type.</returns>
        public static IEnumerable<T> GetAllShapes<T>(IEnumerable<BaseShape> shapes)
        {
            return GetAllShapes(shapes).Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// Get all shapes including grouped shapes of specified type.
        /// </summary>
        /// <typeparam name="T">The type of shapes to include.</typeparam>
        /// <param name="project">The project object.</param>
        /// <returns>All shapes including grouped shapes of specified type.</returns>
        public static IEnumerable<T> GetAllShapes<T>(Project project)
        {
            var shapes = project.Documents
                .SelectMany(d => d.Containers)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes).Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public static void MovePointsBy(IEnumerable<XPoint> points, double dx, double dy)
        {
            foreach (var point in points)
            {
                if (!point.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    point.Move(dx, dy);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public static void MoveShapesBy(IEnumerable<BaseShape> shapes, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(dx, dy);
                }
            }
        }

        /// <summary>
        /// Invalidate renderer's cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating whether is zooming.</param>
        public void InvalidateCache(bool isZooming)
        {
            try
            {
                foreach (var renderer in _renderers)
                {
                    renderer.ClearCache(isZooming);
                }

                _project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export item as Pdf.
        /// </summary>
        /// <param name="path">The Pdf file path.</param>
        /// <param name="item">The item to export.</param>
        public void ExportAsPdf(string path, object item)
        {
            try
            {
                if (_pdfWriter != null)
                {
                    _pdfWriter.Save(path, item, _project);
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export current container as Dxf.
        /// </summary>
        /// <param name="path">The Dxf file path.</param>
        public void ExportAsDxf(string path)
        {
            try
            {
                if (_dxfWriter != null)
                {
                    _dxfWriter.Save(path, _project.CurrentContainer, _project);
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Add recent project file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="name">The project name.</param>
        private void AddRecent(string path, string name)
        {
            var q = _recentProjects.Where(x => x.Path.ToLower() == path.ToLower()).ToList();
            var builder = _recentProjects.ToBuilder();

            if (q.Count() > 0)
            {
                foreach (var r in q)
                {
                    builder.Remove(r);
                }
            }

            builder.Insert(0, RecentProject.Create(name, path));

            RecentProjects = builder.ToImmutable();
            CurrentRecentProject = _recentProjects.FirstOrDefault();
        }

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void LoadRecent(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var json = Project.ReadUtf8Text(path);
                var recent = _serializer.Deserialize<Recent>(json);

                if (recent != null)
                {
                    var remove = recent.RecentProjects.Where(x => System.IO.File.Exists(x.Path) == false).ToList();
                    var builder = recent.RecentProjects.ToBuilder();

                    foreach (var file in remove)
                    {
                        builder.Remove(file);
                    }

                    RecentProjects = builder.ToImmutable();

                    if (recent.CurrentRecentProject != null
                        && System.IO.File.Exists(recent.CurrentRecentProject.Path))
                    {
                        CurrentRecentProject = recent.CurrentRecentProject;
                    }
                    else
                    {
                        CurrentRecentProject = _recentProjects.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void SaveRecent(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var recent = Recent.Create(_recentProjects, _currentRecentProject);
                var json = _serializer.Serialize(recent);
                Project.WriteUtf8Text(path, json);
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Checks if can copy.
        /// </summary>
        /// <returns>Returns true if can copy.</returns>
        public bool CanCopy()
        {
            return IsSelectionAvailable();
        }

        /// <summary>
        /// Checks if can paste.
        /// </summary>
        /// <returns>Returns true if can paste.</returns>
        public async Task<bool> CanPaste()
        {
            try
            {
                return _textClipboard != null && await _textClipboard.ContainsText();
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// Copy selected shapes to clipboard.
        /// </summary>
        /// <param name="shapes"></param>
        private void Copy(IList<BaseShape> shapes)
        {
            try
            {
                if (_textClipboard != null && _serializer != null)
                {
                    var json = _serializer.Serialize(shapes);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _textClipboard.SetText(json);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste text from clipboard as shapes.
        /// </summary>
        /// <param name="text">The text string.</param>
        public void Paste(string text)
        {
            try
            {
                bool havePath = false;

                // Try to parse SVG path geometry. 
                try
                {
                    var geometry = XPathGeometryParser.Parse(text);

                    var path = XPath.Create(
                        "Path",
                        _project.CurrentStyleLibrary.Selected,
                        geometry,
                        _project.Options.DefaultIsStroked,
                        _project.Options.DefaultIsFilled);

                    Paste(Enumerable.Repeat(path, 1));

                    havePath = true;
                }
                catch (Exception)
                {
                    havePath = false;
                }

                // If not successful try to deserialize Json.
                if (!havePath)
                {
                    if (_serializer == null)
                        return;

                    var shapes = _serializer.Deserialize<IList<BaseShape>>(text);
                    if (shapes != null && shapes.Count() > 0)
                    {
                        Paste(shapes);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Try to restore shape styles.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_project.StyleLibraries == null)
                    return;

                var styles = _project.StyleLibraries
                    .Where(sg => sg.Items != null && sg.Items.Length > 0)
                    .SelectMany(sg => sg.Items)
                    .Distinct(new StyleComparer())
                    .ToDictionary(s => s.Name);

                // Reset point shape to container default.
                foreach (var point in shapes.SelectMany(s => s.GetPoints()))
                {
                    point.Shape = _project.Options.PointShape;
                }

                // Try to restore shape styles.
                foreach (var shape in GetAllShapes(shapes))
                {
                    if (shape.Style == null)
                        continue;

                    ShapeStyle style;
                    if (styles.TryGetValue(shape.Style.Name, out style))
                    {
                        // Use existing style.
                        shape.Style = style;
                    }
                    else
                    {
                        // Create Imported style library.
                        if (_project.CurrentStyleLibrary == null)
                        {
                            var sg = Library<ShapeStyle>.Create(Constants.ImportedStyleLibraryName);
                            _project.StyleLibraries = _project.StyleLibraries.Add(sg);
                            _project.CurrentStyleLibrary = sg;
                        }

                        // Add missing style.
                        _project.CurrentStyleLibrary.Items = _project.CurrentStyleLibrary.Items.Add(shape.Style);

                        // Recreate styles dictionary.
                        styles = _project.StyleLibraries
                            .Where(sg => sg.Items != null && sg.Items.Length > 0)
                            .SelectMany(sg => sg.Items)
                            .Distinct(new StyleComparer())
                            .ToDictionary(s => s.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Try to restore shape records.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreRecords(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_project.Databases == null)
                    return;

                var records = _project.Databases
                    .Where(d => d.Records != null && d.Records.Length > 0)
                    .SelectMany(d => d.Records)
                    .ToDictionary(s => s.Id);

                // Try to restore shape record.
                foreach (var shape in GetAllShapes(shapes))
                {
                    if (shape.Data.Record == null)
                        continue;

                    Record record;
                    if (records.TryGetValue(shape.Data.Record.Id, out record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (_project.CurrentDatabase == null)
                        {
                            var db = Database.Create(Constants.ImportedDatabaseName, shape.Data.Record.Columns);
                            _project.Databases = _project.Databases.Add(db);
                            _project.CurrentDatabase = db;
                        }

                        // Add missing data record.
                        _project.CurrentDatabase.Records = _project.CurrentDatabase.Records.Add(shape.Data.Record);

                        // Recreate records dictionary.
                        records = _project.Databases
                            .Where(d => d.Records != null && d.Records.Length > 0)
                            .SelectMany(d => d.Records)
                            .ToDictionary(s => s.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste shapes to current container.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void Paste(IEnumerable<BaseShape> shapes)
        {
            try
            {
                Deselect(_project.CurrentContainer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                var layer = _project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in shapes)
                {
                    builder.Add(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                if (shapes.Count() == 1)
                {
                    Select(_project.CurrentContainer, shapes.FirstOrDefault());
                }
                else
                {
                    Select(_project.CurrentContainer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Clone the <see cref="XGroup"/> object.
        /// </summary>
        /// <param name="group">The <see cref="XGroup"/> object.</param>
        /// <returns>The cloned <see cref="XGroup"/> object.</returns>
        public XGroup Clone(XGroup group)
        {
            if (_serializer == null)
                return null;

            try
            {
                var json = _serializer.Serialize(group);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<XGroup>(json);
                    if (clone != null)
                    {
                        var shapes = Enumerable.Repeat(clone, 1).ToList();
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Clone the <see cref="Container"/> object.
        /// </summary>
        /// <param name="container">The <see cref="Container"/> object.</param>
        /// <returns>The cloned <see cref="Container"/> object.</returns>
        public Container Clone(Container container)
        {
            if (_serializer == null)
                return null;

            try
            {
                var template = container.Template;
                var json = _serializer.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<Container>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        clone.Template = template;
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Clone the <see cref="Document"/> object.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> object.</param>
        /// <returns>The cloned <see cref="Document"/> object.</returns>
        public Document Clone(Document document)
        {
            if (_serializer == null)
                return null;

            try
            {
                var templates = document.Containers.Select(c => c.Template).ToArray();
                var json = _serializer.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<Document>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Containers.Length; i++)
                        {
                            var container = clone.Containers[i];
                            var shapes = container.Layers.SelectMany(l => l.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            container.Template = templates[i];
                        }
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Process dropped files.
        /// </summary>
        /// <param name="files">The files array.</param>
        /// <returns>Returns true if success.</returns>
        public bool Drop(string[] files)
        {
            try
            {
                if (files != null && files.Length >= 1)
                {
                    bool result = false;
                    foreach (var path in files)
                    {
                        if (string.IsNullOrEmpty(path))
                            continue;

                        string ext = System.IO.Path.GetExtension(path);

                        if (string.Compare(ext, Constants.ProjectExtension, true) == 0)
                        {
                            Open(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.CsvExtension, true) == 0)
                        {
                            OnImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleExtension, true) == 0)
                        {
                            OnImportObject(path, _project.CurrentStyleLibrary, ImportType.Style);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StylesExtension, true) == 0)
                        {
                            OnImportObject(path, _project.CurrentStyleLibrary, ImportType.Styles);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleLibraryExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.StyleLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleLibrariesExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.StyleLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupExtension, true) == 0)
                        {
                            OnImportObject(path, _project.CurrentGroupLibrary, ImportType.Group);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupsExtension, true) == 0)
                        {
                            OnImportObject(path, _project.CurrentGroupLibrary, ImportType.Groups);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupLibraryExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.GroupLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupLibrariesExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.GroupLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.TemplateExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.Template);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.TemplatesExtension, true) == 0)
                        {
                            OnImportObject(path, _project, ImportType.Templates);
                            result = true;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Drop <see cref="XGroup"/> object in current container at specified location.
        /// </summary>
        /// <param name="group">The <see cref="XGroup"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropAsClone(XGroup group, double x, double y)
        {
            try
            {
                double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
                double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;

                var clone = Clone(group);
                if (clone != null)
                {
                    Deselect(_project.CurrentContainer);
                    clone.Move(sx, sy);

                    _project.AddShape(clone);

                    Select(_project.CurrentContainer, clone);

                    if (_project.Options.TryToConnect)
                    {
                        TryToConnect(clone);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Drop <see cref="Record"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="Record"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(Record record, double x, double y)
        {
            try
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    _project.ApplyRecord(_renderers[0].State.SelectedShape, record);
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project.ApplyRecord(shape, record);
                    }
                }
                else
                {
                    var container = _project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitTreshold);
                        if (result != null)
                        {
                            _project.ApplyRecord(result, record);
                        }
                        else
                        {
                            DropAsGroup(record, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Drop <see cref="Record"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="Record"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropAsGroup(Record record, double x, double y)
        {
            var g = XGroup.Create("g");
            g.Data.Record = record;

            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;

            var length = record.Values.Length;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            for (int i = 0; i < length; i++)
            {
                var column = record.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + record.Columns[i].Name + "}";

                    var text = XText.Create(
                        px, py,
                        px + width,
                        py + height,
                        _project.CurrentStyleLibrary.Selected,
                        _project.Options.PointShape,
                        binding);

                    g.AddShape(text);

                    py += height;
                }
            }

            var rectangle = XRectangle.Create(
                sx, sy,
                sx + width, sy + (double)length * height,
                _project.CurrentStyleLibrary.Selected,
                _project.Options.PointShape);
            g.AddShape(rectangle);

            double ptx = sx + width / 2;
            double pty = sy;

            double pbx = sx + width / 2;
            double pby = sy + (double)length * height;

            double plx = sx;
            double ply = sy + ((double)length * height) / 2;

            double prx = sx + width;
            double pry = sy + ((double)length * height) / 2;

            var pt = XPoint.Create(ptx, pty, _project.Options.PointShape);
            var pb = XPoint.Create(pbx, pby, _project.Options.PointShape);
            var pl = XPoint.Create(plx, ply, _project.Options.PointShape);
            var pr = XPoint.Create(prx, pry, _project.Options.PointShape);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            _project.AddShape(g);
        }

        /// <summary>
        /// Drop <see cref="ShapeStyle"/> object in current container at specified location.
        /// </summary>
        /// <param name="style">The <see cref="ShapeStyle"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(ShapeStyle style, double x, double y)
        {
            try
            {
                if (_renderers[0].State.SelectedShape != null
                    || (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0))
                {
                    _project.ApplyStyle(style,
                        _renderers[0].State.SelectedShape,
                        _renderers[0].State.SelectedShapes);
                }
                else
                {
                    _project.ApplyStyle(style, x, y);
                }
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Check if can perform the undo action.
        /// </summary>
        /// <returns>Returns true if can undo.</returns>
        public bool CanUndo()
        {
            return _project.History.CanUndo();
        }

        /// <summary>
        /// Check if can perform the redo action.
        /// </summary>
        /// <returns>Returns true if can redo.</returns>
        public bool CanRedo()
        {
            return _project.History.CanRedo();
        }
        
        /// <summary>
        /// Remove selected shapes.
        /// </summary>
        public void DeleteSelected()
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null)
                return;

            if (_renderers[0].State.SelectedShape != null)
            {
                var layer = _project.CurrentContainer.CurrentLayer;

                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(_renderers[0].State.SelectedShape);
                _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                _project.CurrentContainer.CurrentLayer.Invalidate();
                _renderers[0].State.SelectedShape = default(BaseShape);
            }

            if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
            {
                var layer = _project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in _renderers[0].State.SelectedShapes)
                {
                    builder.Remove(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                layer.Invalidate();
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="container">The owner container.</param>
        /// <param name="shape">The shape to select.</param>
        public void Select(Container container, BaseShape shape)
        {
            if (container != null)
            {
                container.CurrentShape = shape;
                _renderers[0].State.SelectedShape = shape;
                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                container.CurrentLayer.Invalidate();
            }
        }

        /// <summary>
        /// Select shape.s
        /// </summary>
        /// <param name="container">The owner container.</param>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(Container container, ImmutableHashSet<BaseShape> shapes)
        {
            if (container != null)
            {
                container.CurrentShape = default(BaseShape);
                _renderers[0].State.SelectedShape = default(BaseShape);
                _renderers[0].State.SelectedShapes = shapes;
                container.CurrentLayer.Invalidate();
            }
        }

        /// <summary>
        /// De-select selected shape(s).
        /// </summary>
        public void Deselect()
        {
            if (_renderers != null)
            {
                _renderers[0].State.SelectedShape = default(BaseShape);
                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
            }
        }

        /// <summary>
        ///  De-select selected shape(s).
        /// </summary>
        /// <param name="container">The container object.</param>
        public void Deselect(Container container)
        {
            if (container != null && _renderers != null)
            {
                if (_renderers[0].State.SelectedShape != null
                    || _renderers[0].State.SelectedShapes != null)
                {
                    _renderers[0].State.SelectedShape = default(BaseShape);
                    _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);

                    container.CurrentShape = default(BaseShape);
                    container.CurrentLayer.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool TryToSelectShapes(Container container, XRectangle rectangle)
        {
            if (container != null)
            {
                var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);

                var result = ShapeBounds.HitTest(container, rect, _project.Options.HitTreshold);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        if (result.Count == 1)
                        {
                            Select(container, result.FirstOrDefault());
                        }
                        else
                        {
                            Select(container, result);
                        }
                        return true;
                    }
                }

                Deselect(container);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToSelectShape(Container container, double x, double y)
        {
            if (container != null)
            {
                var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitTreshold);
                if (result != null)
                {
                    Select(container, result);
                    return true;
                }

                Deselect(container);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void Hover(BaseShape shape)
        {
            Select(_project.CurrentContainer, shape);
            _hover = shape;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dehover()
        {
            _hover = default(BaseShape);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Dehover(Container container)
        {
            _hover = default(BaseShape);
            Deselect(container);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool TryToHoverShape(double x, double y)
        {
            if (_project == null || _project.CurrentContainer == null)
                return false;

            if (_renderers[0].State.SelectedShapes == null
                && !(_renderers[0].State.SelectedShape != null && _hover != _renderers[0].State.SelectedShape))
            {
                var result = ShapeBounds.HitTest(
                    _project.CurrentContainer,
                    new Vector2(x, y),
                    _project.Options.HitTreshold);
                if (result != null)
                {
                    Select(_project.CurrentContainer, result);
                    _hover = result;

                    return true;
                }
                else
                {
                    if (_renderers[0].State.SelectedShape != null
                        && _renderers[0].State.SelectedShape == _hover)
                    {
                        _hover = default(BaseShape);
                        Deselect(_project.CurrentContainer);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="point"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public bool TryToSplitLine(double x, double y, XPoint point, bool select = false)
        {
            if (_project == null || _project.CurrentContainer == null || _project.Options == null)
                return false;

            var result = ShapeBounds.HitTest(
                _project.CurrentContainer,
                new Vector2(x, y),
                _project.Options.HitTreshold);

            if (result is XLine)
            {
                var line = result as XLine;

                if (!_project.Options.SnapToGrid)
                {
                    var a = new Vector2(line.Start.X, line.Start.Y);
                    var b = new Vector2(line.End.X, line.End.Y);
                    var nearest = MathHelpers.NearestPointOnLine(a, b, new Vector2(x, y));
                    point.X = nearest.X;
                    point.Y = nearest.Y;
                }

                var split = XLine.Create(
                    x, y,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);

                double ds = point.DistanceTo(line.Start);
                double de = point.DistanceTo(line.End);

                if (ds < de)
                {
                    split.Start = line.Start;
                    split.End = point;

                    line.Start = point;
                }
                else
                {
                    split.Start = point;
                    split.End = line.End;

                    line.End = point;
                }

                _project.AddShape(split);

                if (select)
                {
                    Select(_project.CurrentContainer, point);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public bool TryToSplitLine(XLine line, XPoint p0, XPoint p1)
        {
            if (_project == null || _project.Options == null)
                return false;

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            XLine split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = XLine.Create(
                    p0,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);
                line.End = p1;
            }
            else
            {
                split = XLine.Create(
                    p1,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);
                line.End = p0;
            }

            _project.AddShape(split);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool TryToConnect(XGroup group)
        {
            if (_project == null
                || _project.CurrentContainer == null
                || _project.CurrentContainer.CurrentLayer == null
                || _project.Options == null)
                return false;

            var layer = _project.CurrentContainer.CurrentLayer;
            if (group.Connectors.Length > 0)
            {
                var wires = GetAllShapes<XLine>(layer.Shapes);
                var dict = new Dictionary<XLine, IList<XPoint>>();

                // Find possible group to line connections.
                foreach (var connector in group.Connectors)
                {
                    var p = new Vector2(connector.X, connector.Y);
                    var t = _project.Options.HitTreshold;
                    XLine result = null;
                    foreach (var line in wires)
                    {
                        if (ShapeBounds.HitTestLine(line, p, t, 0, 0))
                        {
                            result = line;
                            break;
                        }
                    }

                    if (result != null)
                    {
                        if (dict.ContainsKey(result))
                        {
                            dict[result].Add(connector);
                        }
                        else
                        {
                            dict.Add(result, new List<XPoint>());
                            dict[result].Add(connector);
                        }
                    }
                }

                bool success = false;

                // Try to split lines using group connectors.
                foreach (var kv in dict)
                {
                    IList<XPoint> points = kv.Value;
                    if (points.Count == 2)
                    {
                        success = TryToSplitLine(kv.Key, points[0], points[1]);
                    }
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftDownAvailable()
        {
            return _project != null
                && _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.Selected != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLeftUpAvailable()
        {
            return _project != null
                && _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.Selected != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightDownAvailable()
        {
            return _project != null
                && _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.Selected != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRightUpAvailable()
        {
            return _project != null
                && _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.Selected != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsMoveAvailable()
        {
            return _project != null
                && _project.CurrentContainer != null
                && _project.CurrentContainer.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project.CurrentStyleLibrary != null
                && _project.CurrentStyleLibrary.Selected != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSelectionAvailable()
        {
            return _renderers[0].State.SelectedShape != null
                || _renderers[0].State.SelectedShapes != null;
        }

        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftDown(double x, double y)
        {
            Tools[CurrentTool].LeftDown(x, y);
        }

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftUp(double x, double y)
        {
            Tools[CurrentTool].LeftUp(x, y);
        }

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightDown(double x, double y)
        {
            Tools[CurrentTool].RightDown(x, y);
        }

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightUp(double x, double y)
        {
            Tools[CurrentTool].RightUp(x, y);
        }

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void Move(double x, double y)
        {
            Tools[CurrentTool].Move(x, y);
        }

        /// <summary>
        /// Checks if edit mode is active.
        /// </summary>
        /// <returns>Return true if edit mode is active.</returns>
        public bool IsEditMode()
        {
            return true;
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~Editor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _log != null)
            {
                _log.Close();
            }
        }
    }
}
