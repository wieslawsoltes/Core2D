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
        private ISerializer _jsonSerializer;
        private ISerializer _xamlSerializer;
        private IFileWriter _pdfWriter;
        private IFileWriter _dxfWriter;
        private ITextFieldReader<Database> _csvReader;
        private ITextFieldWriter<Database> _csvWriter;
        private ImmutableArray<RecentProject> _recentProjects = ImmutableArray.Create<RecentProject>();
        private RecentProject _currentRecentProject = default(RecentProject);
        private Page _pageToCopy = default(Page);
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
        /// Gets or sets Json serializer.
        /// </summary>
        public ISerializer JsonSerializer
        {
            get { return _jsonSerializer; }
            set { Update(ref _jsonSerializer, value); }
        }

        /// <summary>
        /// Gets or sets Xaml serializer.
        /// </summary>
        public ISerializer XamlSerializer
        {
            get { return _xamlSerializer; }
            set { Update(ref _xamlSerializer, value); }
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
        /// Create new project, document or page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is Page)
            {
                var selected = item as Page;
                var document = _project.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
                if (document != null)
                {
                    var page = default(Page);
                    if (_projectFactory != null)
                    {
                        page = _projectFactory.GetPage(_project, Constants.DefaultPageName);
                    }
                    else
                    {
                        page = Page.Create(Constants.DefaultPageName);
                    }

                    var previous = document.Pages;
                    var next = document.Pages.Add(page);
                    _project.History.Snapshot(previous, next, (p) => document.Pages = p);
                    document.Pages = next;

                    _project.CurrentContainer = page;
                }
            }
            else if (item is Document)
            {
                var selected = item as Document;

                var page = default(Page);
                if (_projectFactory != null)
                {
                    page = _projectFactory.GetPage(_project, Constants.DefaultPageName);
                }
                else
                {
                    page = Page.Create(Constants.DefaultPageName);
                }

                var previous = selected.Pages;
                var next = selected.Pages.Add(page);
                _project.History.Snapshot(previous, next, (p) => selected.Pages = p);
                selected.Pages = next;

                _project.CurrentContainer = page;
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
                _project.CurrentContainer = document.Pages.FirstOrDefault();
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
            if (_jsonSerializer == null)
                return;

            try
            {
                var project = Project.Open(path, _jsonSerializer);

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
            if (_jsonSerializer == null)
                return;

            try
            {
                Project.Save(_project, path, _jsonSerializer);

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
                _project.UpdateDatabase(database, db);
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
        /// Import Xaml from file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        public void OnImportXaml(string path)
        {
            try
            {
                var xaml = Project.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    OnImportXamlString(xaml);
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
        /// Import Xaml string.
        /// </summary>
        /// <remarks>
        /// Supported Xaml types:
        /// - The shape style <see cref="Core2D.ShapeStyle"/>.
        /// - The shape object based on <see cref="Core2D.BaseShape"/> class.
        /// - The styles library using <see cref="Styles"/> container.
        /// - The shapes library using <see cref="Shapes"/> container.
        /// - The groups library using <see cref="Groups"/> container.
        /// - The <see cref="Core2D.Data"/> class.
        /// - The <see cref="Core2D.Database"/> class.
        /// - The <see cref="Core2D.Layer"/> class.
        /// - The <see cref="Core2D.Template"/> class.
        /// - The <see cref="Core2D.Page"/> class.
        /// - The <see cref="Core2D.Document"/> class.
        /// - The <see cref="Core2D.Options"/> class.
        /// - The <see cref="Core2D.Project"/> class.
        /// </remarks>
        /// <param name="xaml">The xaml string.</param>
        public void OnImportXamlString(string xaml)
        {
            if (_xamlSerializer == null)
                return;

            var item = _xamlSerializer.Deserialize<object>(xaml);
            if (item != null)
            {
                if (item is ShapeStyle)
                {
                    _project.AddStyle(_project.CurrentStyleLibrary, item as ShapeStyle);
                }
                else if (item is BaseShape)
                {
                    var shapes = Enumerable.Repeat(item as BaseShape, 1);
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);

                    _project.AddShape(_project.CurrentContainer.CurrentLayer, item as BaseShape);
                }
                else if (item is Styles)
                {
                    var styles = item as Styles;
                    var library = Library<ShapeStyle>.Create(styles.Name, styles.Children);
                    _project.AddStyleLibrary(library);
                }
                else if (item is Shapes)
                {
                    var shapes = (item as Shapes).Children;
                    if (shapes.Count > 0)
                    {
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);

                        _project.AddShapes(_project.CurrentContainer.CurrentLayer, shapes);
                    }
                }
                else if (item is Groups)
                {
                    var groups = item as Groups;
                    var library = Library<XGroup>.Create(groups.Name, groups.Children);
                    _project.AddGroupLibrary(library);
                }
                else if (item is Data)
                {
                    if (_renderers[0].State.SelectedShape != null
                        || (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0))
                    {
                        OnApplyData(item as Data);
                    }
                    else
                    {
                        var page = _project.CurrentContainer as Page;
                        if (page != null)
                        {
                            page.Data = item as Data;
                        }
                    }
                }
                else if (item is Database)
                {
                    _project.AddDatabase(item as Database);
                }
                else if (item is Layer)
                {
                    _project.AddLayer(_project.CurrentContainer, item as Layer);
                }
                else if (item is Template)
                {
                    _project.AddTemplate(item as Template);
                }
                else if (item is Page)
                {
                    _project.AddPage(_project.CurrentDocument, item as Page);
                }
                else if (item is Document)
                {
                    _project.AddDocument(item as Document);
                }
                else if (item is Options)
                {
                    _project.Options = item as Options;
                }
                else if (item is Project)
                {
                    Unload();
                    Load(item as Project, string.Empty);
                }
                else
                {
                    throw new NotSupportedException("Not supported Xaml object.");
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
            if (_jsonSerializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ImportType.Style:
                        {
                            var sg = item as Library<ShapeStyle>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _jsonSerializer.Deserialize<ShapeStyle>(json);

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
                            var import = _jsonSerializer.Deserialize<IList<ShapeStyle>>(json);

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
                            var import = _jsonSerializer.Deserialize<Library<ShapeStyle>>(json);

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
                            var import = _jsonSerializer.Deserialize<IList<Library<ShapeStyle>>>(json);

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
                            var import = _jsonSerializer.Deserialize<XGroup>(json);

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
                            var import = _jsonSerializer.Deserialize<IList<XGroup>>(json);

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
                            var import = _jsonSerializer.Deserialize<Library<XGroup>>(json);

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
                            var import = _jsonSerializer.Deserialize<IList<Library<XGroup>>>(json);

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
                            var import = _jsonSerializer.Deserialize<Template>(json);

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
                            var import = _jsonSerializer.Deserialize<IList<Template>>(json);

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
            if (_jsonSerializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ExportType.Style:
                        {
                            var json = _jsonSerializer.Serialize(item as ShapeStyle);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Styles:
                        {
                            var json = _jsonSerializer.Serialize((item as Library<ShapeStyle>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibrary:
                        {
                            var json = _jsonSerializer.Serialize((item as Library<ShapeStyle>));
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibraries:
                        {
                            var json = _jsonSerializer.Serialize((item as Project).StyleLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Group:
                        {
                            var json = _jsonSerializer.Serialize(item as XGroup);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Groups:
                        {
                            var json = _jsonSerializer.Serialize((item as Library<XGroup>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibrary:
                        {
                            var json = _jsonSerializer.Serialize(item as Library<XGroup>);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibraries:
                        {
                            var json = _jsonSerializer.Serialize((item as Project).GroupLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Template:
                        {
                            var json = _jsonSerializer.Serialize(item as Container);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Templates:
                        {
                            var json = _jsonSerializer.Serialize((item as Project).Templates);
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
        /// Cut selected document, page or shapes to clipboard.
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
        /// Copy document, page or shapes to clipboard.
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
        /// Paste text from clipboard as document, page or shapes.
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
        /// Cut selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is Page)
            {
                var page = item as Page;
                _pageToCopy = page;
                _documentToCopy = default(Document);
                _project.RemovePage(page);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _pageToCopy = default(Page);
                _documentToCopy = document;
                _project.RemoveDocument(document);
            }
            else if (item is Editor || item == null)
            {
                OnCut();
            }
        }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        public void OnCopy(object item)
        {
            if (item is Page)
            {
                var page = item as Page;
                _pageToCopy = page;
                _documentToCopy = default(Document);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _pageToCopy = default(Page);
                _documentToCopy = document;
            }
            else if (item is Editor || item == null)
            {
                OnCopy();
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        public void OnPaste(object item)
        {
            if (item is Page)
            {
                if (_pageToCopy != null)
                {
                    var page = item as Page;
                    var document = _project.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                    if (document != null)
                    {
                        int index = document.Pages.IndexOf(page);
                        var clone = Clone(_pageToCopy);

                        var builder = document.Pages.ToBuilder();
                        builder[index] = clone;
                        document.Pages = builder.ToImmutable();

                        var previous = document.Pages;
                        var next = builder.ToImmutable();
                        _project.History.Snapshot(previous, next, (p) => document.Pages = p);
                        document.Pages = next;

                        _project.CurrentContainer = clone;
                    }
                }
            }
            else if (item is Document)
            {
                if (_pageToCopy != null)
                {
                    var document = item as Document;
                    var clone = Clone(_pageToCopy);

                    var previous = document.Pages;
                    var next = document.Pages.Add(clone);
                    _project.History.Snapshot(previous, next, (p) => document.Pages = p);
                    document.Pages = next;

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
        /// Delete selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is Page)
            {
                _project.RemovePage(item as Page);
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
        /// Group selected shapes.
        /// </summary>
        public void OnGroupSelected()
        {
            var group = Group(_renderers[0].State.SelectedShapes, Constants.DefaulGroupName);
            if (group != null)
            {
                Select(_project.CurrentContainer, group);
            }
        }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public void OnUngroupSelected()
        {
            var result = Ungroup(_renderers[0].State.SelectedShape, _renderers[0].State.SelectedShapes);
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
                BringToFront(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
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
                BringForward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringForward(s);
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
                SendBackward(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
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
                SendToBack(source);
            }

            var sources = _renderers[0].State.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        public void OnMoveUpSelected()
        {
            MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? -_project.Options.SnapY : -1.0);
        }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public void OnMoveDownSelected()
        {
            MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? _project.Options.SnapY : 1.0);
        }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public void OnMoveLeftSelected()
        {
            MoveBy(
                _renderers[0].State.SelectedShape,
                _renderers[0].State.SelectedShapes,
                _project.Options.SnapToGrid ? -_project.Options.SnapX : -1.0,
                0.0);
        }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public void OnMoveRightSelected()
        {
            MoveBy(
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
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        /// <param name="record">The data record item.</param>
        public void OnApplyRecord(Record record)
        {
            if (_project == null)
                return;

            if (record != null)
            {
                // Selected shape.
                if (_renderers[0].State.SelectedShape != null)
                {
                    _project.ApplyRecord(_renderers[0].State.SelectedShape.Data, record);
                }

                // Selected shapes.
                if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project.ApplyRecord(shape.Data, record);
                    }
                }

                // Current page.
                if (_renderers[0].State.SelectedShape == null && _renderers[0].State.SelectedShapes == null)
                {
                    var page = _project.CurrentContainer as Page;
                    if (page != null)
                    {
                        _project.ApplyRecord(page.Data, record);
                    }
                }
            }
        }

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="library">The group library.</param>
        public void OnAddGroup(Library<XGroup> library)
        {
            if (_renderers != null && _project == null || library == null)
                return;

            var group = _renderers[0].State.SelectedShape as XGroup;
            if (group != null)
            {
                var clone = CloneShape(group);
                if (clone != null)
                {
                    _project.AddGroup(library, clone);
                }
            }
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        /// <param name="group">The group item.</param>
        public void OnRemoveGroup(XGroup group)
        {
            if (_project == null)
                return;

            if (group != null)
            {
                _project.RemoveGroup(group);
            }
        }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="group">The group instance.</param>
        public void OnInsertGroup(XGroup group)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            DropShapeAsClone(group, 0.0, 0.0);
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        public void OnApplyStyle(ShapeStyle style)
        {
            if (_project == null)
                return;

            if (style != null)
            {
                // Selected shape.
                if (_renderers[0].State.SelectedShape != null)
                {
                    _project.ApplyStyle(_renderers[0].State.SelectedShape, style);
                }

                // Selected shapes.
                if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project.ApplyStyle(shape, style);
                    }
                }
            }
        }

        /// <summary>
        /// Set current data as selected shape data.
        /// </summary>
        /// <param name="data">The data item.</param>
        public void OnApplyData(Data data)
        {
            if (_project == null)
                return;

            if (data != null)
            {
                // Selected shape.
                if (_renderers[0].State.SelectedShape != null)
                {
                    _project.ApplyData(_renderers[0].State.SelectedShape, data);
                }

                // Selected shapes.
                if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project.ApplyData(shape, data);
                    }
                }
            }
        }

        /// <summary>
        /// Add shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        public void OnAddShape(BaseShape shape)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            if (layer != null && shape != null)
            {
                _project.AddShape(layer, shape);
            }

        }

        /// <summary>
        /// Remove shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        public void OnRemoveShape(BaseShape shape)
        {
            if (_project == null || _project.CurrentContainer == null)
                return;

            var layer = _project.CurrentContainer.CurrentLayer;
            if (layer != null && shape != null)
            {
                _project.RemoveShape(layer, shape);
                _project.CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
            }
        }

        /// <summary>
        /// Add template.
        /// </summary>
        public void OnAddTemplate()
        {
            if (_project == null)
                return;

            var template = default(Template);
            if (_projectFactory != null)
            {
                template = _projectFactory.GetTemplate(_project, "Empty");
            }
            else
            {
                template = Template.Create(Constants.DefaultTemplateName);
            }

            _project.AddTemplate(template);
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnRemoveTemplate(Template template)
        {
            if (_project != null && template != null)
            {
                _project.RemoveTemplate(template);
            }
        }

        /// <summary>
        /// Edit template.
        /// </summary>
        public void OnEditTemplate(Template template)
        {
            if (_project != null && template != null)
            {
                _project.CurrentContainer = template;
                _project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnApplyTemplate(Template template)
        {
            if (_project == null)
                return;

            var page = _project.CurrentContainer as Page;
            if (page != null && template != null)
            {
                _project.ApplyTemplate(page, template);
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
        /// Add page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddPage(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            var container = default(Page);
            if (_projectFactory != null)
            {
                container = _projectFactory.GetPage(_project, Constants.DefaultPageName);
            }
            else
            {
                container = Page.Create(Constants.DefaultPageName);
            }

            _project.AddPage(_project.CurrentDocument, container);
            _project.CurrentContainer = container;
        }

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageBefore(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            if (item is Page)
            {
                var selected = item as Page;
                int index = _project.CurrentDocument.Pages.IndexOf(selected);

                var container = default(Page);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetPage(_project, Constants.DefaultPageName);
                }
                else
                {
                    container = Page.Create(Constants.DefaultPageName);
                }

                _project.AddPageAt(_project.CurrentDocument, container, index);
                _project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageAfter(object item)
        {
            if (_project == null || _project.CurrentDocument == null)
                return;

            if (item is Page)
            {
                var selected = item as Page;
                int index = _project.CurrentDocument.Pages.IndexOf(selected);

                var container = default(Page);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetPage(_project, Constants.DefaultPageName);
                }
                else
                {
                    container = Page.Create(Constants.DefaultPageName);
                }

                _project.AddPageAt(_project.CurrentDocument, container, index + 1);
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
            _project.CurrentContainer = document.Pages.FirstOrDefault();
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
                _project.CurrentContainer = document.Pages.FirstOrDefault();
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
                _project.CurrentContainer = document.Pages.FirstOrDefault();
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
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes).Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// Move points by specified offset.
        /// </summary>
        /// <param name="points">The points collection.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
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
        /// Move shapes by specified offset.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
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
            if (_jsonSerializer == null)
                return;

            try
            {
                var json = Project.ReadUtf8Text(path);
                var recent = _jsonSerializer.Deserialize<Recent>(json);

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
            if (_jsonSerializer == null)
                return;

            try
            {
                var recent = Recent.Create(_recentProjects, _currentRecentProject);
                var json = _jsonSerializer.Serialize(recent);
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
                if (_textClipboard != null && _jsonSerializer != null)
                {
                    var json = _jsonSerializer.Serialize(shapes);
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
                var exception = default(Exception);

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
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Try to deserialize Xaml.
                try
                {
                    OnImportXamlString(text);
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Try to deserialize Json.
                try
                {
                    if (_jsonSerializer != null)
                    {
                        var shapes = _jsonSerializer.Deserialize<IList<BaseShape>>(text);
                        if (shapes != null && shapes.Count() > 0)
                        {
                            Paste(shapes);
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                throw exception;
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

        private void ResetPointShapeToDefault(IEnumerable<BaseShape> shapes)
        {
            foreach (var point in shapes.SelectMany(s => s.GetPoints()))
            {
                point.Shape = _project.Options.PointShape;
            }
        }

        private IDictionary<string, ShapeStyle> GenerateStyleDictionaryByName()
        {
            return _project.StyleLibraries
                .Where(sl => sl.Items != null && sl.Items.Length > 0)
                .SelectMany(sl => sl.Items)
                .Distinct(new StyleComparer())
                .ToDictionary(s => s.Name);
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

                var styles = GenerateStyleDictionaryByName();

                // Reset point shape to defaults.
                ResetPointShapeToDefault(shapes);

                // Try to restore shape styles.
                foreach (var shape in GetAllShapes(shapes))
                {
                    if (shape.Style == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(shape.Style.Name))
                    {
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
                                var sl = Library<ShapeStyle>.Create(Constants.ImportedStyleLibraryName);
                                _project.AddStyleLibrary(sl);
                                _project.CurrentStyleLibrary = sl;
                            }

                            // Add missing style.
                            _project.AddStyle(_project.CurrentStyleLibrary, shape.Style);

                            // Recreate styles dictionary.
                            styles = GenerateStyleDictionaryByName();
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

        private IDictionary<Guid, Record> GenerateRecordsDictionaryById()
        {
            return _project.Databases
                .Where(d => d.Records != null && d.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
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

                var records = GenerateRecordsDictionaryById();

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
                            _project.AddDatabase(db);
                            _project.CurrentDatabase = db;
                        }

                        // Add missing data record.
                        shape.Data.Record.Owner = _project.CurrentDatabase;
                        _project.AddRecord(_project.CurrentDatabase, shape.Data.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordsDictionaryById();
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

                _project.AddShapes(_project.CurrentContainer.CurrentLayer, shapes);

                Select(shapes);
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
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void Select(IEnumerable<BaseShape> shapes)
        {
            if (shapes.Count() == 1)
            {
                Select(_project.CurrentContainer, shapes.FirstOrDefault());
            }
            else
            {
                Select(_project.CurrentContainer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
            }
        }

        /// <summary>
        /// Clone the <see cref="BaseShape"/> object.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <returns>The cloned <see cref="BaseShape"/> object.</returns>
        public T CloneShape<T>(T shape) where T : BaseShape
        {
            if (_jsonSerializer == null)
                return default(T);

            try
            {
                var json = _jsonSerializer.Serialize(shape);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer.Deserialize<T>(json);
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

            return default(T);
        }

        /// <summary>
        /// Clone the <see cref="Template"/> object.
        /// </summary>
        /// <param name="template">The <see cref="Template"/> object.</param>
        /// <returns>The cloned <see cref="Template"/> object.</returns>
        public Container Clone(Template template)
        {
            if (_jsonSerializer == null)
                return default(Template);

            try
            {
                var json = _jsonSerializer.Serialize(template);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer.Deserialize<Page>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
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

            return default(Template);
        }

        /// <summary>
        /// Clone the <see cref="Page"/> object.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> object.</param>
        /// <returns>The cloned <see cref="Page"/> object.</returns>
        public Page Clone(Page page)
        {
            if (_jsonSerializer == null)
                return default(Page);

            try
            {
                var template = page.Template;
                var json = _jsonSerializer.Serialize(page);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer.Deserialize<Page>(json);
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

            return default(Page);
        }

        /// <summary>
        /// Clone the <see cref="Document"/> object.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> object.</param>
        /// <returns>The cloned <see cref="Document"/> object.</returns>
        public Document Clone(Document document)
        {
            if (_jsonSerializer == null)
                return default(Document);

            try
            {
                var templates = document.Pages.Select(c => c.Template).ToArray();
                var json = _jsonSerializer.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer.Deserialize<Document>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Pages.Length; i++)
                        {
                            var container = clone.Pages[i];
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

            return default(Document);
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
                        else if (string.Compare(ext, Constants.XamlExtension, true) == 0)
                        {
                            OnImportXaml(path);
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
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropShape(BaseShape shape, double x, double y)
        {
            try
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    var target = _renderers[0].State.SelectedShape;
                    if (target is XPoint)
                    {
                        var point = target as XPoint;
                        if (point != null)
                        {
                            point.Shape = shape;
                        }
                    }
                }
                else if (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var target in _renderers[0].State.SelectedShapes)
                    {
                        if (target is XPoint)
                        {
                            var point = target as XPoint;
                            if (point != null)
                            {
                                point.Shape = shape;
                            }
                        }
                    }
                }
                else
                {
                    var container = _project.CurrentContainer;
                    if (container != null)
                    {
                        var target = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitThreshold);
                        if (target != null)
                        {
                            if (target is XPoint)
                            {
                                var point = target as XPoint;
                                if (point != null)
                                {
                                    point.Shape = shape;
                                }
                            }
                        }
                        else
                        {
                            DropShapeAsClone(shape, x, y);
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
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropShapeAsClone<T>(T shape, double x, double y) where T : BaseShape
        {
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;

            try
            {
                var clone = CloneShape(shape);
                if (clone != null)
                {
                    Deselect(_project.CurrentContainer);
                    clone.Move(sx, sy);

                    _project.AddShape(_project.CurrentContainer.CurrentLayer, clone);

                    Select(_project.CurrentContainer, clone);

                    if (_project.Options.TryToConnect)
                    {
                        if (clone is XGroup)
                        {
                            TryToConnectLines(
                                GetAllShapes<XLine>(_project.CurrentContainer.CurrentLayer.Shapes),
                                (clone as XGroup).Connectors,
                                _project.Options.HitThreshold);
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
        /// Drop <see cref="Record"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="Record"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(Record record, double x, double y)
        {
            try
            {
                if (_renderers[0].State.SelectedShape != null
                    || (_renderers[0].State.SelectedShapes != null && _renderers[0].State.SelectedShapes.Count > 0))
                {
                    OnApplyRecord(record);
                }
                else
                {
                    var container = _project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitThreshold);
                        if (result != null)
                        {
                            _project.ApplyRecord(result.Data, record);
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
            var g = XGroup.Create(Constants.DefaulGroupName);

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

            var pt = XPoint.Create(sx + width / 2, sy, _project.Options.PointShape);
            var pb = XPoint.Create(sx + width / 2, sy + (double)length * height, _project.Options.PointShape);
            var pl = XPoint.Create(sx, sy + ((double)length * height) / 2, _project.Options.PointShape);
            var pr = XPoint.Create(sx + width, sy + ((double)length * height) / 2, _project.Options.PointShape);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            _project.AddShape(_project.CurrentContainer.CurrentLayer, g);
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
                    OnApplyStyle(style);
                }
                else
                {
                    var container = _project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitThreshold);
                        if (result != null)
                        {
                            _project.ApplyStyle(result, style);
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

                _renderers[0].State.SelectedShape = default(BaseShape);
                layer.Invalidate();
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
        /// <param name="shape">The shape to select.</param>
        public void Select(BaseShape shape)
        {
            if (_renderers != null)
            {
                _renderers[0].State.SelectedShape = shape;

                if (_renderers[0].State.SelectedShapes != null)
                {
                    _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                }
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(ImmutableHashSet<BaseShape> shapes)
        {
            if (_renderers != null)
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    _renderers[0].State.SelectedShape = default(BaseShape);
                }

                _renderers[0].State.SelectedShapes = shapes;
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        public void Deselect()
        {
            if (_renderers != null)
            {
                if (_renderers[0].State.SelectedShape != null)
                {
                    _renderers[0].State.SelectedShape = default(BaseShape);
                }

                if (_renderers[0].State.SelectedShapes != null)
                {
                    _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                }
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
                Select(shape);

                container.CurrentShape = shape;

                if (container.CurrentLayer != null)
                {
                    container.CurrentLayer.Invalidate();
                }
                else
                {
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="container">The owner container.</param>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(Container container, ImmutableHashSet<BaseShape> shapes)
        {
            if (container != null)
            {
                Select(shapes);

                if (container.CurrentShape != null)
                {
                    container.CurrentShape = default(BaseShape);
                }

                if (container.CurrentLayer != null)
                {
                    container.CurrentLayer.Invalidate();
                }
                else
                {
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        /// <param name="container">The container object.</param>
        public void Deselect(Container container)
        {
            if (container != null)
            {
                Deselect();

                if (container.CurrentShape != null)
                {
                    container.CurrentShape = default(BaseShape);
                }

                if (container.CurrentLayer != null)
                {
                    container.CurrentLayer.Invalidate();
                }
                else
                {
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Try to select shape at specified coordinates.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <returns>True if selecting shape was successful.</returns>
        public bool TryToSelectShape(Container container, double x, double y)
        {
            if (container != null)
            {
                var result = ShapeBounds.HitTest(container, new Vector2(x, y), _project.Options.HitThreshold);
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
        /// Try to select shapes inside rectangle.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <returns>True if selecting shapes was successful.</returns>
        public bool TryToSelectShapes(Container container, XRectangle rectangle)
        {
            if (container != null)
            {
                var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
                var result = ShapeBounds.HitTest(container, rect, _project.Options.HitThreshold);
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
        /// Hover shape.
        /// </summary>
        /// <param name="container">The container object.</param>
        /// <param name="shape">The shape to hover.</param>
        public void Hover(Container container, BaseShape shape)
        {
            if (container != null)
            {
                Select(container, shape);
                _hover = shape;
            }
        }

        /// <summary>
        /// De-hover shape.
        /// </summary>
        /// <param name="container">The container object.</param>
        public void Dehover(Container container)
        {
            if (container != null && _hover != null)
            {
                _hover = default(BaseShape);
                Deselect(container);
            }
        }

        /// <summary>
        /// Try to hover shape at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <returns>True if hovering shape was successful.</returns>
        public bool TryToHoverShape(double x, double y)
        {
            if (_project == null || _project.CurrentContainer == null)
                return false;

            if (_renderers[0].State.SelectedShapes == null
                && !(_renderers[0].State.SelectedShape != null && _hover != _renderers[0].State.SelectedShape))
            {
                var result = ShapeBounds.HitTest(_project.CurrentContainer, new Vector2(x, y), _project.Options.HitThreshold);
                if (result != null)
                {
                    Hover(_project.CurrentContainer, result);
                    return true;
                }
                else
                {
                    if (_renderers[0].State.SelectedShape != null && _renderers[0].State.SelectedShape == _hover)
                    {
                        Dehover(_project.CurrentContainer);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Try to split line at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="point">The point used for split line start or end.</param>
        /// <param name="select">The flag indicating whether to select split line.</param>
        /// <returns>True if line split was successful.</returns>
        public bool TryToSplitLine(double x, double y, XPoint point, bool select = false)
        {
            if (_project == null || _project.CurrentContainer == null || _project.Options == null)
                return false;

            var result = ShapeBounds.HitTest(
                _project.CurrentContainer,
                new Vector2(x, y),
                _project.Options.HitThreshold);

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

                    // Swap line start point.
                    var previous = line.Start;
                    var next = point;
                    _project.History.Snapshot(previous, next, (p) => line.Start = p);
                    line.Start = next;
                }
                else
                {
                    split.Start = point;
                    split.End = line.End;

                    // Swap line end point.
                    var previous = line.End;
                    var next = point;
                    _project.History.Snapshot(previous, next, (p) => line.End = p);
                    line.End = next;
                }

                _project.AddShape(_project.CurrentContainer.CurrentLayer, split);

                if (select)
                {
                    Select(_project.CurrentContainer, point);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to split lines using group connectors.
        /// </summary>
        /// <param name="line">The line to split.</param>
        /// <param name="p0">The first connector point.</param>
        /// <param name="p1">The second connector point.</param>
        /// <returns>True if line split was successful.</returns>
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

                // Swap line end point.
                var previous = line.End;
                var next = p1;
                _project.History.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }
            else
            {
                split = XLine.Create(
                    p1,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);

                // Swap line end point.
                var previous = line.End;
                var next = p0;
                _project.History.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }

            _project.AddShape(_project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        /// <summary>
        ///  Try to connect lines to connectors.
        /// </summary>
        /// <param name="lines">The lines to connect.</param>
        /// <param name="connectors">The connectors array.</param>
        /// <param name="threshold">The connection threshold.</param>
        /// <returns>True if connection was successful.</returns>
        public bool TryToConnectLines(IEnumerable<XLine> lines, ImmutableArray<XPoint> connectors, double threshold)
        {
            if (connectors.Length > 0)
            {
                var lineToPoints = new Dictionary<XLine, IList<XPoint>>();

                // Find possible connector to line connections.
                foreach (var connector in connectors)
                {
                    XLine result = null;
                    foreach (var line in lines)
                    {
                        if (ShapeBounds.HitTestLine(line, new Vector2(connector.X, connector.Y), threshold, 0, 0))
                        {
                            result = line;
                            break;
                        }
                    }

                    if (result != null)
                    {
                        if (lineToPoints.ContainsKey(result))
                        {
                            lineToPoints[result].Add(connector);
                        }
                        else
                        {
                            lineToPoints.Add(result, new List<XPoint>());
                            lineToPoints[result].Add(connector);
                        }
                    }
                }

                // Try to split lines using connectors.
                bool success = false;
                foreach (var kv in lineToPoints)
                {
                    XLine line = kv.Key;
                    IList<XPoint> points = kv.Value;
                    if (points.Count == 2)
                    {
                        var p0 = points[0];
                        var p1 = points[1];
                        bool horizontal = Math.Abs(p0.Y - p1.Y) < threshold;
                        bool vertical = Math.Abs(p0.X - p1.X) < threshold;

                        // Points are aligned horizontally.
                        if (horizontal && !vertical)
                        {
                            if (p0.X <= p1.X)
                            {
                                success = TryToSplitLine(line, p0, p1);
                            }
                            else
                            {
                                success = TryToSplitLine(line, p1, p0);
                            }
                        }

                        // Points are aligned vertically.
                        if (!horizontal && vertical)
                        {
                            if (p0.Y >= p1.Y)
                            {
                                success = TryToSplitLine(line, p1, p0);
                            }
                            else
                            {
                                success = TryToSplitLine(line, p0, p1);
                            }
                        }
                    }
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Group shapes.
        /// </summary>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="name">The group name.</param>
        public XGroup Group(ImmutableHashSet<BaseShape> shapes, string name)
        {
            if (_project == null)
                return null;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var source = layer.Shapes.ToBuilder();
                    var group = XGroup.Group(name, shapes, source);

                    var previous = layer.Shapes;
                    var next = source.ToImmutable();
                    _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    return group;
                }
            }
            return null;
        }

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        public bool Ungroup(BaseShape shape, ImmutableHashSet<BaseShape> shapes)
        {
            if (_project == null)
                return false;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    if (shape != null && shape is XGroup && layer != null)
                    {
                        var source = layer.Shapes.ToBuilder();

                        XGroup.Ungroup(shape as XGroup, source);

                        var previous = layer.Shapes;
                        var next = source.ToImmutable();
                        _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                        layer.Shapes = next;

                        return true;
                    }

                    if (shapes != null && layer != null)
                    {
                        var source = layer.Shapes.ToBuilder();

                        XGroup.Ungroup(shapes, source);

                        var previous = layer.Shapes;
                        var next = source.ToImmutable();
                        _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                        layer.Shapes = next;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Move shape from source index to target index position in an array. 
        /// </summary>
        /// <param name="source">The source shape.</param>
        /// <param name="sourceIndex">The source shape index.</param>
        /// <param name="targetIndex">The target shape index.</param>
        private void Move(BaseShape source, int sourceIndex, int targetIndex)
        {
            if (_project == null)
                return;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    if (items != null)
                    {
                        if (sourceIndex < targetIndex)
                        {
                            var builder = items.ToBuilder();
                            builder.Insert(targetIndex + 1, source);
                            builder.RemoveAt(sourceIndex);

                            var previous = layer.Shapes;
                            var next = builder.ToImmutable();
                            _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                            layer.Shapes = next;
                        }
                        else
                        {
                            int removeIndex = sourceIndex + 1;
                            if (items.Length + 1 > removeIndex)
                            {
                                var builder = items.ToBuilder();
                                builder.Insert(targetIndex, source);
                                builder.RemoveAt(removeIndex);

                                var previous = layer.Shapes;
                                var next = builder.ToImmutable();
                                _project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                                layer.Shapes = next;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bring a shape to the top of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringToFront(BaseShape source)
        {
            if (_project == null)
                return;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = items.Length - 1;
                    if (targetIndex >= 0 && sourceIndex != targetIndex)
                    {
                        Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringForward(BaseShape source)
        {
            if (_project == null)
                return;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = sourceIndex + 1;
                    if (targetIndex < items.Length)
                    {
                        Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendBackward(BaseShape source)
        {
            if (_project == null)
                return;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = sourceIndex - 1;
                    if (targetIndex >= 0)
                    {
                        Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendToBack(BaseShape source)
        {
            if (_project == null)
                return;

            var container = _project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = 0;
                    if (sourceIndex != targetIndex)
                    {
                        Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move shape(s) by specified offset.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public void MoveBy(BaseShape shape, ImmutableHashSet<BaseShape> shapes, double dx, double dy)
        {
            if (_project == null)
                return;

            if (shape != null)
            {
                var state = shape.State;

                switch (_project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var items = Enumerable.Repeat(shape, 1);
                                var points = items.SelectMany(s => s.GetPoints()).Distinct().ToList();

                                Editor.MovePointsBy(points, dx, dy);

                                var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                                var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                                _project.History.Snapshot(previous, next, (s) => Editor.MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked) && !state.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var items = Enumerable.Repeat(shape, 1).ToList();

                                Editor.MoveShapesBy(items, dx, dy);

                                var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = items };
                                var next = new { DeltaX = dx, DeltaY = dy, Shapes = items };
                                _project.History.Snapshot(previous, next, (s) => Editor.MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                }
            }

            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));

                switch (_project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var points = items.SelectMany(s => s.GetPoints()).Distinct().ToList();

                            Editor.MovePointsBy(points, dx, dy);

                            var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                            var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                            _project.History.Snapshot(previous, next, (s) => Editor.MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            Editor.MoveShapesBy(items, dx, dy);

                            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = items.ToList() };
                            var next = new { DeltaX = dx, DeltaY = dy, Shapes = items.ToList() };
                            _project.History.Snapshot(previous, next, (s) => Editor.MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                        }
                        break;
                }
            }
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
        /// Check if left down action is available.
        /// </summary>
        /// <returns>True if left down action is available.</returns>
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
        /// Check if left up action is available.
        /// </summary>
        /// <returns>True if left up action is available.</returns>
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
        /// Check if right down action is available.
        /// </summary>
        /// <returns>True if right down action is available.</returns>
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
        /// Check if right up action is available.
        /// </summary>
        /// <returns>True if right up action is available.</returns>
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
        /// Check if move action is available.
        /// </summary>
        /// <returns>True if move action is available.</returns>
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
        /// Check if selection is available.
        /// </summary>
        /// <returns>True if selection is available.</returns>
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
        /// Initialize non-platform specific editor commands.
        /// </summary>
        public void InitializeCommands()
        {
            Commands.NewCommand =
                Command<object>.Create(
                    (item) => OnNew(item),
                    (item) => IsEditMode());

            Commands.CloseCommand =
                Command.Create(
                    () => OnClose(),
                    () => IsEditMode());

            Commands.ExitCommand =
                Command.Create(
                    () => OnExit(),
                    () => true);

            Commands.UndoCommand =
                Command.Create(
                    () => OnUndo(),
                    () => IsEditMode() /* && CanUndo() */);

            Commands.RedoCommand =
                Command.Create(
                    () => OnRedo(),
                    () => IsEditMode() /* && CanRedo() */);

            Commands.CutCommand =
                Command<object>.Create(
                    (item) => OnCut(item),
                    (item) => IsEditMode() /* && CanCopy() */);

            Commands.CopyCommand =
                Command<object>.Create(
                    (item) => OnCopy(item),
                    (item) => IsEditMode() /* && CanCopy() */);

            Commands.PasteCommand =
                Command<object>.Create(
                    (item) => OnPaste(item),
                    (item) => IsEditMode() /* && CanPaste() */);

            Commands.DeleteCommand =
                Command<object>.Create(
                    (item) => OnDelete(item),
                    (item) => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SelectAllCommand =
                Command.Create(
                    () => OnSelectAll(),
                    () => IsEditMode());

            Commands.DeselectAllCommand =
                Command.Create(
                    () => OnDeselectAll(),
                    () => IsEditMode());

            Commands.ClearAllCommand =
                Command.Create(
                    () => OnClearAll(),
                    () => IsEditMode());

            Commands.GroupCommand =
                Command.Create(
                    () => OnGroupSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.UngroupCommand =
                Command.Create(
                    () => OnUngroupSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.BringToFrontCommand =
                Command.Create(
                    () => OnBringToFrontSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SendToBackCommand =
                Command.Create(
                    () => OnSendToBackSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.BringForwardCommand =
                Command.Create(
                    () => OnBringForwardSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.SendBackwardCommand =
                Command.Create(
                    () => OnSendBackwardSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveUpCommand =
                Command.Create(
                    () => OnMoveUpSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveDownCommand =
                Command.Create(
                    () => OnMoveDownSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveLeftCommand =
                Command.Create(
                    () => OnMoveLeftSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.MoveRightCommand =
                Command.Create(
                    () => OnMoveRightSelected(),
                    () => IsEditMode() /* && IsSelectionAvailable() */);

            Commands.ToolNoneCommand =
                Command.Create(
                    () => OnToolNone(),
                    () => IsEditMode());

            Commands.ToolSelectionCommand =
                Command.Create(
                    () => OnToolSelection(),
                    () => IsEditMode());

            Commands.ToolPointCommand =
                Command.Create(
                    () => OnToolPoint(),
                    () => IsEditMode());

            Commands.ToolLineCommand =
                Command.Create(
                    () => OnToolLine(),
                    () => IsEditMode());

            Commands.ToolArcCommand =
                Command.Create(
                    () => OnToolArc(),
                    () => IsEditMode());

            Commands.ToolBezierCommand =
                Command.Create(
                    () => OnToolBezier(),
                    () => IsEditMode());

            Commands.ToolQBezierCommand =
                Command.Create(
                    () => OnToolQBezier(),
                    () => IsEditMode());

            Commands.ToolPathCommand =
                Command.Create(
                    () => OnToolPath(),
                    () => IsEditMode());

            Commands.ToolRectangleCommand =
                Command.Create(
                    () => OnToolRectangle(),
                    () => IsEditMode());

            Commands.ToolEllipseCommand =
                Command.Create(
                    () => OnToolEllipse(),
                    () => IsEditMode());

            Commands.ToolTextCommand =
                Command.Create(
                    () => OnToolText(),
                    () => IsEditMode());

            Commands.ToolImageCommand =
                Command.Create(
                    () => OnToolImage(),
                    () => IsEditMode());

            Commands.ToolMoveCommand =
                Command.Create(
                    () => OnToolMove(),
                    () => IsEditMode());

            Commands.DefaultIsStrokedCommand =
                Command.Create(
                    () => OnToggleDefaultIsStroked(),
                    () => IsEditMode());

            Commands.DefaultIsFilledCommand =
                Command.Create(
                    () => OnToggleDefaultIsFilled(),
                    () => IsEditMode());

            Commands.DefaultIsClosedCommand =
                Command.Create(
                    () => OnToggleDefaultIsClosed(),
                    () => IsEditMode());

            Commands.DefaultIsSmoothJoinCommand =
                Command.Create(
                    () => OnToggleDefaultIsSmoothJoin(),
                    () => IsEditMode());

            Commands.SnapToGridCommand =
                Command.Create(
                    () => OnToggleSnapToGrid(),
                    () => IsEditMode());

            Commands.TryToConnectCommand =
                Command.Create(
                    () => OnToggleTryToConnect(),
                    () => IsEditMode());

            Commands.AddDatabaseCommand =
                Command.Create(
                    () => Project.AddDatabase(Database.Create(Constants.DefaultDatabaseName)),
                    () => IsEditMode());

            Commands.RemoveDatabaseCommand =
                Command<Database>.Create(
                    (db) => Project.RemoveDatabase(db),
                    (db) => IsEditMode());

            Commands.AddColumnCommand =
                Command<Database>.Create(
                    (db) => Project.AddColumn(db, Column.Create(db, Constants.DefaulColumnName)),
                    (db) => IsEditMode());

            Commands.RemoveColumnCommand =
                Command<Column>.Create(
                    (column) => Project.RemoveColumn(column),
                    (column) => IsEditMode());

            Commands.AddRecordCommand =
                Command<Database>.Create(
                    (db) => Project.AddRecord(db, Record.Create(db, Constants.DefaulValue)),
                    (db) => IsEditMode());

            Commands.RemoveRecordCommand =
                Command<Record>.Create(
                    (record) => Project.RemoveRecord(record),
                    (record) => IsEditMode());

            Commands.ResetRecordCommand =
                Command<Data>.Create(
                    (data) => Project.ResetRecord(data),
                    (data) => IsEditMode());

            Commands.ApplyRecordCommand =
                Command<Record>.Create(
                    (record) => OnApplyRecord(record),
                    (record) => IsEditMode());

            Commands.AddPropertyCommand =
                Command<Data>.Create(
                    (data) => Project.AddProperty(data, Property.Create(data, Constants.DefaulPropertyName, Constants.DefaulValue)),
                    (data) => IsEditMode());

            Commands.RemovePropertyCommand =
                Command<Property>.Create(
                    (property) => Project.RemoveProperty(property),
                    (property) => IsEditMode());

            Commands.AddGroupLibraryCommand =
                Command.Create(
                    () => Project.AddGroupLibrary(Library<XGroup>.Create(Constants.DefaulGroupLibraryName)),
                    () => IsEditMode());

            Commands.RemoveGroupLibraryCommand =
                Command<Library<XGroup>>.Create(
                    (library) => Project.RemoveGroupLibrary(library),
                    (library) => IsEditMode());

            Commands.AddGroupCommand =
                Command<Library<XGroup>>.Create(
                    (library) => OnAddGroup(library),
                    (library) => IsEditMode());

            Commands.RemoveGroupCommand =
                Command<XGroup>.Create(
                    (group) => OnRemoveGroup(group),
                    (group) => IsEditMode());

            Commands.InsertGroupCommand =
                Command<XGroup>.Create(
                    (group) => OnInsertGroup(group),
                    (group) => IsEditMode());

            Commands.AddLayerCommand =
                Command<Container>.Create(
                    (container) => Project.AddLayer(container, Layer.Create(Constants.DefaultLayerName, container)),
                    (container) => IsEditMode());

            Commands.RemoveLayerCommand =
                Command<Layer>.Create(
                    (layer) => Project.RemoveLayer(layer),
                    (layer) => IsEditMode());

            Commands.AddStyleLibraryCommand =
                Command.Create(
                    () => Project.AddStyleLibrary(Library<ShapeStyle>.Create(Constants.DefaulStyleLibraryName)),
                    () => IsEditMode());

            Commands.RemoveStyleLibraryCommand =
                Command<Library<ShapeStyle>>.Create(
                    (library) => Project.RemoveStyleLibrary(library),
                    (library) => IsEditMode());

            Commands.AddStyleCommand =
                Command<Library<ShapeStyle>>.Create(
                    (library) => Project.AddStyle(library, ShapeStyle.Create(Constants.DefaulStyleName)),
                    (library) => IsEditMode());

            Commands.RemoveStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) => Project.RemoveStyle(style),
                    (style) => IsEditMode());

            Commands.ApplyStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) => OnApplyStyle(style),
                    (style) => IsEditMode());

            Commands.AddShapeCommand =
                Command<BaseShape>.Create(
                    (shape) => OnAddShape(shape),
                    (shape) => IsEditMode());

            Commands.RemoveShapeCommand =
                Command<BaseShape>.Create(
                    (shape) => OnRemoveShape(shape),
                    (shape) => IsEditMode());

            Commands.AddTemplateCommand =
                Command.Create(
                    () => OnAddTemplate(),
                    () => IsEditMode());

            Commands.RemoveTemplateCommand =
                Command<Template>.Create(
                    (template) => OnRemoveTemplate(template),
                    (template) => IsEditMode());

            Commands.EditTemplateCommand =
                Command<Template>.Create(
                    (template) => OnEditTemplate(template),
                    (template) => IsEditMode());

            Commands.ApplyTemplateCommand =
                Command<Template>.Create(
                    (template) => OnApplyTemplate(template),
                    (template) => true);

            Commands.AddImageKeyCommand =
                Command.Create(
                    async () => await OnAddImageKey(null),
                    () => IsEditMode());

            Commands.RemoveImageKeyCommand =
                Command<string>.Create(
                    (key) => OnRemoveImageKey(key),
                    (key) => IsEditMode());

            Commands.SelectedItemChangedCommand =
                Command<object>.Create(
                    (item) => OnSelectedItemChanged(item),
                    (item) => IsEditMode());

            Commands.AddPageCommand =
                Command<object>.Create(
                    (item) => OnAddPage(item),
                    (item) => IsEditMode());

            Commands.InsertPageBeforeCommand =
                Command<object>.Create(
                    (item) => OnInsertPageBefore(item),
                    (item) => IsEditMode());

            Commands.InsertPageAfterCommand =
                Command<object>.Create(
                    (item) => OnInsertPageAfter(item),
                    (item) => IsEditMode());

            Commands.AddDocumentCommand =
                Command<object>.Create(
                    (item) => OnAddDocument(item),
                    (item) => IsEditMode());

            Commands.InsertDocumentBeforeCommand =
                Command<object>.Create(
                    (item) => OnInsertDocumentBefore(item),
                    (item) => IsEditMode());

            Commands.InsertDocumentAfterCommand =
                Command<object>.Create(
                    (item) => OnInsertDocumentAfter(item),
                    (item) => IsEditMode());
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
