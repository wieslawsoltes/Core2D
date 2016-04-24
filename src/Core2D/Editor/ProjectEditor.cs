// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Collections;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Interfaces;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools;
using Core2D.Editor.Views;
using Core2D.History;
using Core2D.Interfaces;
using Core2D.Math;
using Core2D.Path.Parser;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using static System.Math;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor.
    /// </summary>
    public class ProjectEditor : ObservableObject
    {
        private XProject _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private ImmutableDictionary<Tool, ToolBase> _tools;
        private Tool _currentTool;
        private PathTool _currentPathTool;
        private Action _invalidate;
        private Action _resetZoom;
        private Action _extentZoom;
        private Action _loadLayout;
        private Action _saveLayout;
        private Action _resetLayout;
        private bool _cancelAvailable;
        private XContainer _pageToCopy;
        private XDocument _documentToCopy;
        private BaseShape _hover;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private ImmutableArray<ViewBase> _views;
        private ViewBase _currentView;
        private DashboardView _dashboardView;
        private EditorView _editorView;
        private IEditorApplication _application;
        private ILog _log;
        private CommandManager _commandManager;
        private ShapeRenderer[] _renderers;
        private IFileSystem _fileIO;
        private IProjectFactory _projectFactory;
        private ITextClipboard _textClipboard;
        private ITextSerializer _jsonSerializer;
        private ITextSerializer _xamlSerializer;
        private ImmutableArray<IFileWriter> _fileWriters;
        private ITextFieldReader<XDatabase> _csvReader;
        private ITextFieldWriter<XDatabase> _csvWriter;

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        public XProject Project
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
        /// Gets or sets current project collections and objects observer.
        /// </summary>
        public ProjectObserver Observer
        {
            get { return _observer; }
            set { Update(ref _observer, value); }
        }

        /// <summary>
        /// Gets or sets editor tools dictionary.
        /// </summary>
        public ImmutableDictionary<Tool, ToolBase> Tools
        {
            get { return _tools; }
            set { Update(ref _tools, value); }
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
        /// Gets or sets invalidate action.
        /// </summary>
        /// <remarks>Invalidate current container control.</remarks>
        public Action Invalidate
        {
            get { return _invalidate; }
            set { Update(ref _invalidate, value); }
        }

        /// <summary>
        /// Gets or sets reset zoom action.
        /// </summary>
        /// <remarks>Reset view size to defaults.</remarks>
        public Action ResetZoom
        {
            get { return _resetZoom; }
            set { Update(ref _resetZoom, value); }
        }

        /// <summary>
        /// Gets or sets extent zoom action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        public Action AutoFitZoom
        {
            get { return _extentZoom; }
            set { Update(ref _extentZoom, value); }
        }

        /// <summary>
        /// Gets or sets load layout action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        public Action LoadLayout
        {
            get { return _loadLayout; }
            set { Update(ref _loadLayout, value); }
        }

        /// <summary>
        /// Gets or sets save layout action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        public Action SaveLayout
        {
            get { return _saveLayout; }
            set { Update(ref _saveLayout, value); }
        }

        /// <summary>
        /// Gets or sets reset layout action.
        /// </summary>
        /// <remarks>Reset editor layout.</remarks>
        public Action ResetLayout
        {
            get { return _resetLayout; }
            set { Update(ref _resetLayout, value); }
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
        /// Gets or sets recent projects collection.
        /// </summary>
        public ImmutableArray<RecentFile> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }

        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        public RecentFile CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }

        /// <summary>
        /// Gets or sets registered views.
        /// </summary>
        public ImmutableArray<ViewBase> Views
        {
            get { return _views; }
            set { Update(ref _views, value); }
        }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        public ViewBase CurrentView
        {
            get { return _currentView; }
            set { Update(ref _currentView, value); }
        }

        /// <summary>
        /// Gets or sets current editor application.
        /// </summary>
        public IEditorApplication Application
        {
            get { return _application; }
            set { Update(ref _application, value); }
        }

        /// <summary>
        /// Gets or sets current log.
        /// </summary>
        public ILog Log
        {
            get { return _log; }
            set { Update(ref _log, value); }
        }

        /// <summary>
        /// Gets or sets current command manager.
        /// </summary>
        public CommandManager CommandManager
        {
            get { return _commandManager; }
            set { Update(ref _commandManager, value); }
        }

        /// <summary>
        /// Gets or sets current renderer's.
        /// </summary>
        public ShapeRenderer[] Renderers
        {
            get { return _renderers; }
            set { Update(ref _renderers, value); }
        }

        /// <summary>
        /// Gets or sets current file system.
        /// </summary>
        public IFileSystem FileIO
        {
            get { return _fileIO; }
            set { Update(ref _fileIO, value); }
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
        public ITextSerializer JsonSerializer
        {
            get { return _jsonSerializer; }
            set { Update(ref _jsonSerializer, value); }
        }

        /// <summary>
        /// Gets or sets Xaml serializer.
        /// </summary>
        public ITextSerializer XamlSerializer
        {
            get { return _xamlSerializer; }
            set { Update(ref _xamlSerializer, value); }
        }

        /// <summary>
        /// Gets or sets available file writers.
        /// </summary>
        public ImmutableArray<IFileWriter> FileWriters
        {
            get { return _fileWriters; }
            set { Update(ref _fileWriters, value); }
        }

        /// <summary>
        /// Gets or sets Csv file reader.
        /// </summary>
        public ITextFieldReader<XDatabase> CsvReader
        {
            get { return _csvReader; }
            set { Update(ref _csvReader, value); }
        }

        /// <summary>
        /// Gets or sets Csv file writer.
        /// </summary>
        public ITextFieldWriter<XDatabase> CsvWriter
        {
            get { return _csvWriter; }
            set { Update(ref _csvWriter, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditor"/> class.
        /// </summary>
        public ProjectEditor()
        {
            _tools = new Dictionary<Tool, ToolBase>
            {
                [Tool.None] = new ToolNone(this),
                [Tool.Selection] = new ToolSelection(this),
                [Tool.Point] = new ToolPoint(this),
                [Tool.Line] = new ToolLine(this),
                [Tool.Arc] = new ToolArc(this),
                [Tool.CubicBezier] = new ToolCubicBezier(this),
                [Tool.QuadraticBezier] = new ToolQuadraticBezier(this),
                [Tool.Path] = new ToolPath(this),
                [Tool.Rectangle] = new ToolRectangle(this),
                [Tool.Ellipse] = new ToolEllipse(this),
                [Tool.Text] = new ToolText(this),
                [Tool.Image] = new ToolImage(this)
            }.ToImmutableDictionary();

            _pageToCopy = default(XContainer);
            _documentToCopy = default(XDocument);
            _hover = default(BaseShape);

            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default(RecentFile);

            _dashboardView = new DashboardView { Name = "Dashboard", Context = this };
            _editorView = new EditorView { Name = "Editor", Context = this };
            _views = new List<ViewBase> { _dashboardView, _editorView }.ToImmutableArray();
            _currentView = _dashboardView;
        }

        /// <summary>
        /// Get object item name.
        /// </summary>
        /// <param name="item">The object item.</param>
        public string GetName(object item)
        {
            if (item != null)
            {
                if (item is BaseStyle)
                    return (item as BaseStyle).Name;
                else if (item is BaseShape)
                    return (item as BaseShape).Name;
                else if (item is XLibrary<ShapeStyle>)
                    return (item as XLibrary<ShapeStyle>).Name;
                else if (item is XLibrary<XGroup>)
                    return (item as XLibrary<XGroup>).Name;
                if (item is XContainer)
                    return (item as XContainer).Name;
                if (item is XLayer)
                    return (item as XLayer).Name;
                if (item is XDocument)
                    return (item as XDocument).Name;
                if (item is XProject)
                    return (item as XProject).Name;
                if (item is XDatabase)
                    return (item as XDatabase).Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is XContainer)
            {
                OnNewPage(item as XContainer);
            }
            else if (item is XDocument)
            {
                OnNewPage(item as XDocument);
            }
            else if (item is XProject)
            {
                OnNewDocument();
            }
            else if (item is ProjectEditor)
            {
                OnNewProject();
            }
            else if (item == null)
            {
                if (_project == null)
                {
                    OnNewProject();
                }
                else
                {
                    if (_project.CurrentDocument == null)
                    {
                        OnNewDocument();
                    }
                    else
                    {
                        OnNewPage(_project.CurrentDocument);
                    }
                }
            }
        }

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected page.</param>
        private void OnNewPage(XContainer selected)
        {
            var document = _project?.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
            if (document != null)
            {
                var page =
                    _projectFactory?.GetPage(_project, Constants.DefaultPageName)
                    ?? XContainer.CreatePage(Constants.DefaultPageName);

                _project?.AddPage(document, page);
                _project?.SetCurrentContainer(page);
            }
        }

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected document.</param>
        private void OnNewPage(XDocument selected)
        {
            var page =
                _projectFactory?.GetPage(_project, Constants.DefaultPageName)
                ?? XContainer.CreatePage(Constants.DefaultPageName);

            _project?.AddPage(selected, page);
            _project?.SetCurrentContainer(page);
        }

        /// <summary>
        /// Create new document.
        /// </summary>
        private void OnNewDocument()
        {
            var document =
                _projectFactory?.GetDocument(_project, Constants.DefaultDocumentName)
                ?? XDocument.Create(Constants.DefaultDocumentName);

            _project?.AddDocument(document);
            _project?.SetCurrentDocument(document);
            _project?.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        private void OnNewProject()
        {
            Unload();
            Load(_projectFactory?.GetProject() ?? XProject.Create(), string.Empty);
            OnChangeCurrentView(_editorView);
            Invalidate?.Invoke();
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Open(string path)
        {
            try
            {
                if (_fileIO != null && _jsonSerializer != null)
                {
                    if (!string.IsNullOrEmpty(path) && _fileIO.Exists(path))
                    {
                        var project = XProject.Open(path, _fileIO, _jsonSerializer);
                        if (project != null)
                        {
                            Unload();
                            Load(project, path);
                            AddRecent(path, project.Name);
                            OnChangeCurrentView(_editorView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void OnClose()
        {
            OnChangeCurrentView(_dashboardView);
            _project?.History?.Reset();
            Unload();
        }

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Save(string path)
        {
            try
            {
                if (_project != null && _fileIO != null && _jsonSerializer != null)
                {
                    XProject.Save(_project, path, _fileIO, _jsonSerializer);
                    AddRecent(path, _project.Name);

                    if (string.IsNullOrEmpty(_projectPath))
                    {
                        _projectPath = path;
                    }

                    IsProjectDirty = false;
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        public void OnImportData(string path)
        {
            try
            {
                if (_project != null)
                {
                    var db = _csvReader?.Read(path, _fileIO);
                    if (db != null)
                    {
                        _project.AddDatabase(db);
                        _project.SetCurrentDatabase(db);
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnExportData(string path, XDatabase database)
        {
            try
            {
                _csvWriter?.Write(path, _fileIO, database);
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnUpdateData(string path, XDatabase database)
        {
            try
            {
                var db = _csvReader?.Read(path, _fileIO);
                if (db != null)
                {
                    _project?.UpdateDatabase(database, db);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="item">The object to import.</param>
        /// <param name="restore">Try to restore objects by name.</param>
        public void OnImportObject(object item, bool restore)
        {
            if (item is ShapeStyle)
            {
                _project?.AddStyle(_project?.CurrentStyleLibrary, item as ShapeStyle);
            }
            else if (item is IList<ShapeStyle>)
            {
                _project.AddItems(_project?.CurrentStyleLibrary, item as IList<ShapeStyle>);
            }
            else if (item is BaseShape)
            {
                if (item is XGroup)
                {
                    var group = item as XGroup;
                    if (restore)
                    {
                        var shapes = Enumerable.Repeat(group, 1);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    _project.AddGroup(_project?.CurrentGroupLibrary, group);
                }
                else
                {
                    _project?.AddShape(_project?.CurrentContainer?.CurrentLayer, item as BaseShape);
                }
            }
            else if (item is IList<XGroup>)
            {
                var groups = item as IList<XGroup>;
                if (restore)
                {
                    TryToRestoreStyles(groups);
                    TryToRestoreRecords(groups);
                }
                _project.AddItems(_project?.CurrentGroupLibrary, groups);
            }
            else if (item is XLibrary<ShapeStyle>)
            {
                _project.AddStyleLibrary(item as XLibrary<ShapeStyle>);
            }
            else if (item is IList<XLibrary<ShapeStyle>>)
            {
                _project.AddStyleLibraries(item as IList<XLibrary<ShapeStyle>>);
            }
            else if (item is XLibrary<XGroup>)
            {
                var gl = item as XLibrary<XGroup>;
                TryToRestoreStyles(gl.Items);
                TryToRestoreRecords(gl.Items);
                _project.AddGroupLibrary(gl);
            }
            else if (item is IList<XLibrary<XGroup>>)
            {
                var gll = item as IList<XLibrary<XGroup>>;
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);
                _project.AddGroupLibraries(gll);
            }
            else if (item is XStyles)
            {
                var styles = item as XStyles;
                var library = XLibrary<ShapeStyle>.Create(styles.Name, styles.Children);
                _project?.AddStyleLibrary(library);
            }
            else if (item is XShapes)
            {
                var shapes = (item as XShapes).Children;
                if (shapes.Length > 0)
                {
                    _project?.AddShapes(_project?.CurrentContainer?.CurrentLayer, shapes);
                }
            }
            else if (item is XGroups)
            {
                var groups = item as XGroups;
                var library = XLibrary<XGroup>.Create(groups.Name, groups.Children);
                _project?.AddGroupLibrary(library);
            }
            else if (item is XDatabases)
            {
                var databases = (item as XDatabases).Children;
                if (databases.Length > 0)
                {
                    foreach (var database in databases)
                    {
                        _project?.AddDatabase(database);
                    }
                }
            }
            else if (item is XTemplates)
            {
                var templates = (item as XTemplates).Children;
                if (templates.Length > 0)
                {
                    foreach (var template in templates)
                    {
                        _project?.AddTemplate(template);
                    }
                }
            }
            else if (item is XContext)
            {
                if (_renderers?[0]?.State?.SelectedShape != null || (_renderers?[0]?.State?.SelectedShapes?.Count > 0))
                {
                    OnApplyData(item as XContext);
                }
                else
                {
                    var container = _project?.CurrentContainer;
                    if (container != null)
                    {
                        container.Data = item as XContext;
                    }
                }
            }
            else if (item is XDatabase)
            {
                var db = item as XDatabase;
                _project?.AddDatabase(db);
                _project?.SetCurrentDatabase(db);
            }
            else if (item is XLayer)
            {
                var layer = item as XLayer;
                if (restore)
                {
                    TryToRestoreStyles(layer.Shapes);
                    TryToRestoreRecords(layer.Shapes);
                }
                _project?.AddLayer(_project?.CurrentContainer, layer);
            }
            else if (item is XContainer)
            {
                var container = item as XContainer;
                if (container.Template == null)
                {
                    // Import as template.
                    if (restore)
                    {
                        var shapes = container.Layers.SelectMany(x => x.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    _project?.AddTemplate(container);
                }
                else
                {
                    // Import as page.
                    if (restore)
                    {
                        var shapes = Enumerable.Concat(
                            container.Layers.SelectMany(x => x.Shapes),
                            container.Template?.Layers.SelectMany(x => x.Shapes));
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    _project?.AddPage(_project?.CurrentDocument, container);
                }
            }
            else if (item is IList<XContainer>)
            {
                var templates = item as IList<XContainer>;
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                _project.AddTemplates(templates);
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        document.Pages.SelectMany(x => x.Layers).SelectMany(x => x.Shapes),
                        document.Pages.SelectMany(x => x.Template.Layers).SelectMany(x => x.Shapes));
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }
                _project?.AddDocument(document);
            }
            else if (item is XOptions)
            {
                if (_project != null)
                {
                    _project.Options = item as XOptions;
                }
            }
            else if (item is XProject)
            {
                Unload();
                Load(item as XProject, string.Empty);
            }
            else
            {
                throw new NotSupportedException("Not supported import object.");
            }
        }

        /// <summary>
        /// Import Xaml from a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        public void OnImportXaml(string path)
        {
            try
            {
                var xaml = _fileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    OnImportXamlString(xaml);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import Xaml string.
        /// </summary>
        /// <param name="xaml">The xaml string.</param>
        public void OnImportXamlString(string xaml)
        {
            var item = _xamlSerializer?.Deserialize<object>(xaml);
            if (item != null)
            {
                OnImportObject(item, false);
            }
        }

        /// <summary>
        /// Export Xaml to a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        /// <param name="item">The object item.</param>
        public void OnExportXaml(string path, object item)
        {
            try
            {
                var xaml = _xamlSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    _fileIO?.WriteUtf8Text(path, xaml);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import Json from a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        public void OnImportJson(string path)
        {
            try
            {
                var json = _fileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    OnImportJsonString(json);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import Json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        private void OnImportJsonString(string json)
        {
            var item = _jsonSerializer.Deserialize<object>(json);
            if (item != null)
            {
                OnImportObject(item, true);
            }
        }

        /// <summary>
        /// Export Json to a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        /// <param name="item">The object item.</param>
        public void OnExportJson(string path, object item)
        {
            try
            {
                var json = _jsonSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _fileIO?.WriteUtf8Text(path, json);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public void OnUndo()
        {
            try
            {
                if (_project?.History.CanUndo() ?? false)
                {
                    Deselect();
                    _project?.History.Undo();
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public void OnRedo()
        {
            try
            {
                if (_project?.History.CanRedo() ?? false)
                {
                    Deselect();
                    _project?.History.Redo();
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                    if (_renderers?[0]?.State?.SelectedShape != null)
                    {
                        Copy(Enumerable.Repeat(_renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (_renderers?[0]?.State?.SelectedShapes != null)
                    {
                        Copy(_renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        public async void OnPaste()
        {
            try
            {
                if (await CanPaste())
                {
                    var text = await (_textClipboard?.GetText() ?? Task.FromResult(string.Empty));
                    if (!string.IsNullOrEmpty(text))
                    {
                        Paste(text);
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Cut selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is XContainer)
            {
                var page = item as XContainer;
                _pageToCopy = page;
                _documentToCopy = default(XDocument);
                _project?.RemovePage(page);
                _project?.SetCurrentContainer(_project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
                _pageToCopy = default(XContainer);
                _documentToCopy = document;
                _project?.RemoveDocument(document);

                var selected = _project?.Documents.FirstOrDefault();
                _project?.SetCurrentDocument(selected);
                _project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
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
            if (item is XContainer)
            {
                var page = item as XContainer;
                _pageToCopy = page;
                _documentToCopy = default(XDocument);
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
                _pageToCopy = default(XContainer);
                _documentToCopy = document;
            }
            else if (item is ProjectEditor || item == null)
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
            if (_project != null && item is XContainer)
            {
                if (_pageToCopy != null)
                {
                    var page = item as XContainer;
                    var document = _project?.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                    if (document != null)
                    {
                        int index = document.Pages.IndexOf(page);
                        var clone = Clone(_pageToCopy);
                        _project.ReplacePage(document, clone, index);
                        _project.SetCurrentContainer(clone);
                    }
                }
            }
            else if (_project != null && item is XDocument)
            {
                if (_pageToCopy != null)
                {
                    var document = item as XDocument;
                    var clone = Clone(_pageToCopy);
                    _project?.AddPage(document, clone);
                    _project.SetCurrentContainer(clone);
                }
                else if (_documentToCopy != null)
                {
                    var document = item as XDocument;
                    int index = _project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);
                    _project.ReplaceDocument(clone, index);
                    _project.SetCurrentDocument(clone);
                    _project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
                }
            }
            else if (item is ProjectEditor || item == null)
            {
                OnPaste();
            }
        }

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is XLayer)
            {
                var layer = item as XLayer;
                _project?.RemoveLayer(item as XLayer);

                var selected = _project?.CurrentContainer?.Layers.FirstOrDefault();
                layer?.Owner?.SetCurrentLayer(selected);
            }
            if (item is XContainer)
            {
                _project?.RemovePage(item as XContainer);

                var selected = _project?.CurrentDocument?.Pages.FirstOrDefault();
                _project?.SetCurrentContainer(selected);
            }
            else if (item is XDocument)
            {
                _project?.RemoveDocument(item as XDocument);

                var selected = _project?.Documents.FirstOrDefault();
                _project?.SetCurrentDocument(selected);
                _project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
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
                Deselect(_project?.CurrentContainer?.CurrentLayer);
                Select(
                    _project?.CurrentContainer?.CurrentLayer,
                    ImmutableHashSet.CreateRange<BaseShape>(_project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public void OnDeselectAll()
        {
            try
            {
                Deselect(_project?.CurrentContainer?.CurrentLayer);
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public void OnClearAll()
        {
            try
            {
                var container = _project?.CurrentContainer;
                if (container != null)
                {
                    foreach (var layer in container.Layers)
                    {
                        _project?.ClearLayer(layer);
                    }

                    container.WorkingLayer.Shapes = ImmutableArray.Create<BaseShape>();
                    container.HelperLayer.Shapes = ImmutableArray.Create<BaseShape>();

                    _project.CurrentContainer.Invalidate();
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        public void OnGroupSelected()
        {
            var group = Group(_renderers?[0]?.State?.SelectedShapes, Constants.DefaulGroupName);
            if (group != null)
            {
                Select(_project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public void OnUngroupSelected()
        {
            var result = Ungroup(_renderers?[0]?.State?.SelectedShape, _renderers?[0]?.State?.SelectedShapes);
            if (result == true && _renderers?[0]?.State != null)
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
            var source = _renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringToFront(source);
            }

            var sources = _renderers?[0]?.State?.SelectedShapes;
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
            var source = _renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringForward(source);
            }

            var sources = _renderers?[0]?.State?.SelectedShapes;
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
            var source = _renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendBackward(source);
            }

            var sources = _renderers?[0]?.State?.SelectedShapes;
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
            var source = _renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendToBack(source);
            }

            var sources = _renderers?[0]?.State?.SelectedShapes;
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
                _renderers?[0]?.State?.SelectedShape,
                _renderers?[0]?.State?.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? -_project.Options.SnapY : -1.0);
        }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public void OnMoveDownSelected()
        {
            MoveBy(
                _renderers?[0]?.State?.SelectedShape,
                _renderers?[0]?.State?.SelectedShapes,
                0.0,
                _project.Options.SnapToGrid ? _project.Options.SnapY : 1.0);
        }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public void OnMoveLeftSelected()
        {
            MoveBy(
                _renderers?[0]?.State?.SelectedShape,
                _renderers?[0]?.State?.SelectedShapes,
                _project.Options.SnapToGrid ? -_project.Options.SnapX : -1.0,
                0.0);
        }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public void OnMoveRightSelected()
        {
            MoveBy(
                _renderers?[0]?.State?.SelectedShape,
                _renderers?[0]?.State?.SelectedShapes,
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
        /// Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
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
        /// Set current tool to <see cref="Tool.CubicBezier"/> or current path tool to <see cref="PathTool.CubicBezier"/>.
        /// </summary>
        public void OnToolCubicBezier()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.CubicBezier)
            {
                CurrentPathTool = PathTool.CubicBezier;
            }
            else
            {
                CurrentTool = Tool.CubicBezier;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.QuadraticBezier"/> or current path tool to <see cref="PathTool.QuadraticBezier"/>.
        /// </summary>
        public void OnToolQuadraticBezier()
        {
            if (CurrentTool == Tool.Path && CurrentPathTool != PathTool.QuadraticBezier)
            {
                CurrentPathTool = PathTool.QuadraticBezier;
            }
            else
            {
                CurrentTool = Tool.QuadraticBezier;
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
        /// Toggle <see cref="XOptions.DefaultIsStroked"/> option.
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (_project?.Options != null)
            {
                _project.Options.DefaultIsStroked = !_project.Options.DefaultIsStroked;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsFilled"/> option.
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (_project?.Options != null)
            {
                _project.Options.DefaultIsFilled = !_project.Options.DefaultIsFilled;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsClosed"/> option.
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (_project?.Options != null)
            {
                _project.Options.DefaultIsClosed = !_project.Options.DefaultIsClosed;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (_project?.Options != null)
            {
                _project.Options.DefaultIsSmoothJoin = !_project.Options.DefaultIsSmoothJoin;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.SnapToGrid"/> option.
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (_project?.Options != null)
            {
                _project.Options.SnapToGrid = !_project.Options.SnapToGrid;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.TryToConnect"/> option.
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (_project?.Options != null)
            {
                _project.Options.TryToConnect = !_project.Options.TryToConnect;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.CloneStyle"/> option.
        /// </summary>
        public void OnToggleCloneStyle()
        {
            if (_project?.Options != null)
            {
                _project.Options.CloneStyle = !_project.Options.CloneStyle;
            }
        }

        /// <summary>
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        /// <param name="record">The data record item.</param>
        public void OnApplyRecord(XRecord record)
        {
            if (record != null)
            {
                // Selected shape.
                if (_renderers?[0]?.State?.SelectedShape != null)
                {
                    _project?.ApplyRecord(_renderers[0].State.SelectedShape?.Data, record);
                }

                // Selected shapes.
                if (_renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project.ApplyRecord(shape.Data, record);
                    }
                }

                // Current page.
                if (_renderers[0].State.SelectedShape == null && _renderers[0].State.SelectedShapes == null)
                {
                    var container = _project?.CurrentContainer;
                    if (container != null)
                    {
                        _project?.ApplyRecord(container.Data, record);
                    }
                }
            }
        }

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="library">The group library.</param>
        public void OnAddGroup(XLibrary<XGroup> library)
        {
            if (_project != null && library != null)
            {
                var group = _renderers?[0]?.State?.SelectedShape as XGroup;
                if (group != null)
                {
                    var clone = CloneShape(group);
                    if (clone != null)
                    {
                        _project?.AddGroup(library, clone);
                    }
                }
            }
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        /// <param name="group">The group item.</param>
        public void OnRemoveGroup(XGroup group)
        {
            if (_project != null && group != null)
            {
                var library = _project.RemoveGroup(group);
                library?.SetSelected(library?.Items.FirstOrDefault());
            }
        }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="group">The group instance.</param>
        public void OnInsertGroup(XGroup group)
        {
            if (_project?.CurrentContainer != null)
            {
                DropShapeAsClone(group, 0.0, 0.0);
            }
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        public void OnApplyStyle(ShapeStyle style)
        {
            if (style != null)
            {
                // Selected shape.
                if (_renderers[0]?.State?.SelectedShape != null)
                {
                    _project?.ApplyStyle(_renderers[0].State.SelectedShape, style);
                }

                // Selected shapes.
                if (_renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project?.ApplyStyle(shape, style);
                    }
                }
            }
        }

        /// <summary>
        /// Set current data as selected shape data.
        /// </summary>
        /// <param name="data">The data item.</param>
        public void OnApplyData(XContext data)
        {
            if (data != null)
            {
                // Selected shape.
                if (_renderers?[0]?.State?.SelectedShape != null)
                {
                    _project?.ApplyData(_renderers[0].State.SelectedShape, data);
                }

                // Selected shapes.
                if (_renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in _renderers[0].State.SelectedShapes)
                    {
                        _project?.ApplyData(shape, data);
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
            var layer = _project?.CurrentContainer?.CurrentLayer;
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
            var layer = _project?.CurrentContainer?.CurrentLayer;
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
            if (_project != null)
            {
                var template = _projectFactory.GetTemplate(_project, "Empty");
                if (template == null)
                {
                    template = XContainer.CreateTemplate(Constants.DefaultTemplateName);
                }

                _project.AddTemplate(template);
            }
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnRemoveTemplate(XContainer template)
        {
            if (template != null)
            {
                _project?.RemoveTemplate(template);
                _project?.SetCurrentTemplate(_project?.Templates.FirstOrDefault());
            }
        }

        /// <summary>
        /// Edit template.
        /// </summary>
        public void OnEditTemplate(XContainer template)
        {
            if (_project != null && template != null)
            {
                _project.SetCurrentContainer(template);
                _project.CurrentContainer?.Invalidate();
            }
        }

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnApplyTemplate(XContainer template)
        {
            var page = _project?.CurrentContainer;
            if (page != null && template != null)
            {
                _project.ApplyTemplate(page, template);
                _project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Add image key.
        /// </summary>
        /// <param name="path">The image path.</param>
        public async Task<string> OnAddImageKey(string path)
        {
            if (_project != null)
            {
                if (path == null || string.IsNullOrEmpty(path))
                {
                    var key = await (GetImageKey() ?? Task.FromResult(string.Empty));
                    if (key == null || string.IsNullOrEmpty(key))
                        return null;

                    return key;
                }
                else
                {
                    byte[] bytes;
                    using (var stream = _fileIO?.Open(path))
                    {
                        bytes = _fileIO?.ReadBinary(stream);
                    }
                    var key = _project.AddImageFromFile(path, bytes);
                    return key;
                }
            }

            return null;
        }

        /// <summary>
        /// Remove image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        public void OnRemoveImageKey(string key)
        {
            if (key != null)
            {
                _project?.RemoveImage(key);
            }
        }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnSelectedItemChanged(XSelectable item)
        {
            if (_project != null)
            {
                _project.Selected = item;
            }
        }

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddPage(object item)
        {
            if (_project?.CurrentDocument != null)
            {
                var page =
                    _projectFactory?.GetPage(_project, Constants.DefaultPageName)
                    ?? XContainer.CreatePage(Constants.DefaultPageName);

                _project.AddPage(_project.CurrentDocument, page);
                _project.SetCurrentContainer(page);
            }
        }

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageBefore(object item)
        {
            if (_project?.CurrentDocument != null)
            {
                if (item is XContainer)
                {
                    var selected = item as XContainer;
                    int index = _project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        _projectFactory?.GetPage(_project, Constants.DefaultPageName)
                        ?? XContainer.CreatePage(Constants.DefaultPageName);

                    _project.AddPageAt(_project.CurrentDocument, page, index);
                    _project.SetCurrentContainer(page);
                }
            }
        }

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageAfter(object item)
        {
            if (_project?.CurrentDocument != null)
            {
                if (item is XContainer)
                {
                    var selected = item as XContainer;
                    int index = _project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        _projectFactory?.GetPage(_project, Constants.DefaultPageName)
                        ?? XContainer.CreatePage(Constants.DefaultPageName);

                    _project.AddPageAt(_project.CurrentDocument, page, index + 1);
                    _project.SetCurrentContainer(page);
                }
            }
        }

        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddDocument(object item)
        {
            if (_project != null)
            {
                var document =
                    _projectFactory?.GetDocument(_project, Constants.DefaultDocumentName)
                    ?? XDocument.Create(Constants.DefaultDocumentName);

                _project.AddDocument(document);
                _project.SetCurrentDocument(document);
                _project.SetCurrentContainer(document?.Pages.FirstOrDefault());
            }
        }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentBefore(object item)
        {
            if (_project != null)
            {
                if (item is XDocument)
                {
                    var selected = item as XDocument;
                    int index = _project.Documents.IndexOf(selected);
                    var document =
                        _projectFactory?.GetDocument(_project, Constants.DefaultDocumentName)
                        ?? XDocument.Create(Constants.DefaultDocumentName);

                    _project.AddDocumentAt(document, index);
                    _project.SetCurrentDocument(document);
                    _project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
        }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentAfter(object item)
        {
            if (_project != null)
            {
                if (item is XDocument)
                {
                    var selected = item as XDocument;
                    int index = _project.Documents.IndexOf(selected);
                    var document =
                        _projectFactory?.GetDocument(_project, Constants.DefaultDocumentName)
                        ?? XDocument.Create(Constants.DefaultDocumentName);

                    _project.AddDocumentAt(document, index + 1);
                    _project.SetCurrentDocument(document);
                    _project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
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
        public void Load(XProject project, string path = null)
        {
            if (project != null)
            {
                Deselect();
                SetRenderersImageCache(project);
                Project = project;
                Project.History = new StackHistory();
                ProjectPath = path;
                IsProjectDirty = false;
                Observer = new ProjectObserver(this);
            }
        }

        /// <summary>
        /// Unload project.
        /// </summary>
        public void Unload()
        {
            Observer?.Dispose();
            Observer = null;

            if (_project?.History != null)
            {
                _project.History.Reset();
                _project.History = null;
            }

            _project?.PurgeUnusedImages(Enumerable.Empty<string>().ToImmutableHashSet());

            Deselect();
            SetRenderersImageCache(null);
            Project = null;
            ProjectPath = string.Empty;
            IsProjectDirty = false;

            GC.Collect();
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
        /// Invalidate renderer's cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating whether is zooming.</param>
        public void InvalidateCache(bool isZooming)
        {
            try
            {
                if (_renderers != null)
                {
                    foreach (var renderer in _renderers)
                    {
                        renderer.ClearCache(isZooming);
                    }
                }

                _project?.CurrentContainer?.Invalidate();
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Export item.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="item">The item to export.</param>
        /// <param name="writer">The file writer.</param>
        public void Export(string path, object item, IFileWriter writer)
        {
            try
            {
                writer?.Save(path, item, _project);
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Add recent project file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="name">The project name.</param>
        private void AddRecent(string path, string name)
        {
            if (_recentProjects != null)
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

                builder.Insert(0, RecentFile.Create(name, path));

                RecentProjects = builder.ToImmutable();
                CurrentRecentProject = _recentProjects.FirstOrDefault();
            }
        }

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void LoadRecent(string path)
        {
            if (_jsonSerializer != null)
            {
                try
                {
                    var json = _fileIO.ReadUtf8Text(path);
                    var recent = _jsonSerializer.Deserialize<Recents>(json);
                    if (recent != null)
                    {
                        var remove = recent.Files.Where(x => _fileIO?.Exists(x.Path) == false).ToList();
                        var builder = recent.Files.ToBuilder();

                        foreach (var file in remove)
                        {
                            builder.Remove(file);
                        }

                        RecentProjects = builder.ToImmutable();

                        if (recent.Current != null
                            && (_fileIO?.Exists(recent.Current.Path) ?? false))
                        {
                            CurrentRecentProject = recent.Current;
                        }
                        else
                        {
                            CurrentRecentProject = _recentProjects.FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void SaveRecent(string path)
        {
            if (_jsonSerializer != null)
            {
                try
                {
                    var recent = Recents.Create(_recentProjects, _currentRecentProject);
                    var json = _jsonSerializer.Serialize(recent);
                    _fileIO.WriteUtf8Text(path, json);
                }
                catch (Exception ex)
                {
                    _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Check if undo action is available.
        /// </summary>
        /// <returns>Returns true if undo action is available.</returns>
        public bool CanUndo()
        {
            return _project?.History?.CanUndo() ?? false;
        }

        /// <summary>
        /// Check if redo action is available.
        /// </summary>
        /// <returns>Returns true if redo action is available.</returns>
        public bool CanRedo()
        {
            return _project?.History?.CanRedo() ?? false;
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
                return await (_textClipboard?.ContainsText() ?? Task.FromResult(false));
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                var json = _jsonSerializer?.Serialize(shapes);
                if (!string.IsNullOrEmpty(json))
                {
                    _textClipboard?.SetText(json);
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                    var style = _project?.CurrentStyleLibrary?.Selected;
                    if (style != null)
                    {
                        var path = XPath.Create(
                            "Path",
                            _project.Options.CloneStyle ? style.Clone() : style,
                            geometry,
                            _project.Options.DefaultIsStroked,
                            _project.Options.DefaultIsFilled);

                        Paste(Enumerable.Repeat(path, 1));
                        return;
                    }
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
                    var shapes = _jsonSerializer?.Deserialize<IList<BaseShape>>(text);
                    if (shapes?.Count() > 0)
                    {
                        Paste(shapes);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        private void ResetPointShapeToDefault(IEnumerable<BaseShape> shapes)
        {
            foreach (var point in shapes?.SelectMany(s => s?.GetPoints()))
            {
                point.Shape = _project?.Options?.PointShape;
            }
        }

        private IDictionary<string, ShapeStyle> GenerateStyleDictionaryByName()
        {
            return _project?.StyleLibraries
                .Where(sl => sl?.Items != null && sl?.Items.Length > 0)
                .SelectMany(sl => sl.Items)
                .Distinct(new ShapeStyleByNameComparer())
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
                if (_project?.StyleLibraries == null)
                    return;

                var styles = GenerateStyleDictionaryByName();

                // Reset point shape to defaults.
                ResetPointShapeToDefault(shapes);

                // Try to restore shape styles.
                foreach (var shape in XProject.GetAllShapes(shapes))
                {
                    if (shape?.Style == null)
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
                            if (_project?.CurrentStyleLibrary == null)
                            {
                                var sl = XLibrary<ShapeStyle>.Create(Constants.ImportedStyleLibraryName);
                                _project.AddStyleLibrary(sl);
                                _project.SetCurrentStyleLibrary(sl);
                            }

                            // Add missing style.
                            _project?.AddStyle(_project?.CurrentStyleLibrary, shape.Style);

                            // Recreate styles dictionary.
                            styles = GenerateStyleDictionaryByName();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        private IDictionary<Guid, XRecord> GenerateRecordDictionaryById()
        {
            return _project?.Databases
                .Where(d => d?.Records != null && d?.Records.Length > 0)
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
                if (_project?.Databases == null)
                    return;

                var records = GenerateRecordDictionaryById();

                // Try to restore shape record.
                foreach (var shape in XProject.GetAllShapes(shapes))
                {
                    if (shape?.Data?.Record == null)
                        continue;

                    XRecord record;
                    if (records.TryGetValue(shape.Data.Record.Id, out record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (_project?.CurrentDatabase == null)
                        {
                            var db = XDatabase.Create(Constants.ImportedDatabaseName, shape.Data.Record.Columns);
                            _project.AddDatabase(db);
                            _project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Data.Record.Owner = _project.CurrentDatabase;
                        _project?.AddRecord(_project?.CurrentDatabase, shape.Data.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordDictionaryById();
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Deselect(_project?.CurrentContainer?.CurrentLayer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                _project.AddShapes(_project?.CurrentContainer?.CurrentLayer, shapes);

                Select(shapes);
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void Select(IEnumerable<BaseShape> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(_project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(_project?.CurrentContainer?.CurrentLayer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
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
            try
            {
                var json = _jsonSerializer?.Serialize(shape);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer?.Deserialize<T>(json);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(T);
        }
        
        /// <summary>
        /// Clone the <see cref="XContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> object.</param>
        /// <returns>The cloned <see cref="XContainer"/> object.</returns>
        public XContainer Clone(XContainer container)
        {
            try
            {
                var template = container?.Template;
                var json = _jsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer?.Deserialize<XContainer>(json);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(XContainer);
        }

        /// <summary>
        /// Clone the <see cref="XDocument"/> object.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/> object.</param>
        /// <returns>The cloned <see cref="XDocument"/> object.</returns>
        public XDocument Clone(XDocument document)
        {
            try
            {
                var templates = document?.Pages.Select(c => c?.Template)?.ToArray();
                var json = _jsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _jsonSerializer?.Deserialize<XDocument>(json);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(XDocument);
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
                if (files?.Length >= 1)
                {
                    bool result = false;
                    foreach (var path in files)
                    {
                        if (string.IsNullOrEmpty(path))
                            continue;

                        string ext = System.IO.Path.GetExtension(path);

                        if (string.Compare(ext, Constants.ProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Open(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.CsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.JsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportJson(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.XamlExtension, StringComparison.OrdinalIgnoreCase) == 0)
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                if (_renderers?[0]?.State?.SelectedShape != null)
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
                else if (_renderers?[0]?.State?.SelectedShapes != null && _renderers?[0]?.State?.SelectedShapes?.Count > 0)
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
                    var layer = _project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var target = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), _project.Options.HitThreshold);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                    Deselect(_project?.CurrentContainer?.CurrentLayer);
                    clone.Move(sx, sy);

                    _project.AddShape(_project?.CurrentContainer?.CurrentLayer, clone);

                    Select(_project?.CurrentContainer?.CurrentLayer, clone);

                    if (_project.Options.TryToConnect)
                    {
                        if (clone is XGroup)
                        {
                            TryToConnectLines(
                                XProject.GetAllShapes<XLine>(_project?.CurrentContainer?.CurrentLayer?.Shapes),
                                (clone as XGroup).Connectors,
                                _project.Options.HitThreshold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Drop <see cref="XRecord"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="XRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(XRecord record, double x, double y)
        {
            try
            {
                if (_renderers?[0]?.State?.SelectedShape != null
                    || (_renderers?[0]?.State?.SelectedShapes != null && _renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    OnApplyRecord(record);
                }
                else
                {
                    var layer = _project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), _project.Options.HitThreshold);
                        if (result != null)
                        {
                            _project?.ApplyRecord(result.Data, record);
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
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Drop <see cref="XRecord"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="XRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropAsGroup(XRecord record, double x, double y)
        {
            var selected = _project.CurrentStyleLibrary.Selected;
            var style = _project.Options.CloneStyle ? selected.Clone() : selected;
            var point = _project?.Options?.PointShape;
            var layer = _project?.CurrentContainer?.CurrentLayer;
            double sx = _project.Options.SnapToGrid ? Snap(x, _project.Options.SnapX) : x;
            double sy = _project.Options.SnapToGrid ? Snap(y, _project.Options.SnapY) : y;

            var g = XGroup.Create(Constants.DefaulGroupName);

            g.Data.Record = record;

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
                    var text = XText.Create(px, py, px + width, py + height, style, point, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = XRectangle.Create(sx, sy, sx + width, sy + length * height, style, point);
            g.AddShape(rectangle);

            var pt = XPoint.Create(sx + width / 2, sy, point);
            var pb = XPoint.Create(sx + width / 2, sy + length * height, point);
            var pl = XPoint.Create(sx, sy + (length * height) / 2, point);
            var pr = XPoint.Create(sx + width, sy + (length * height) / 2, point);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            _project.AddShape(layer, g);
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
                if (_renderers?[0]?.State?.SelectedShape != null
                    || (_renderers?[0]?.State?.SelectedShapes != null && _renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    OnApplyStyle(style);
                }
                else
                {
                    var layer = _project.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), _project.Options.HitThreshold);
                        if (result != null)
                        {
                            _project.ApplyStyle(result, style);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Remove selected shapes.
        /// </summary>
        public void DeleteSelected()
        {
            if (_project?.CurrentContainer?.CurrentLayer == null || _renderers?[0]?.State == null)
                return;

            if (_renderers[0].State.SelectedShape != null)
            {
                var layer = _project.CurrentContainer.CurrentLayer;

                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(_renderers[0].State.SelectedShape);
                _project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
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
                _project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
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
            if (_renderers?[0]?.State != null)
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
            if (_renderers?[0]?.State != null)
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
            if (_renderers?[0].State?.SelectedShape != null)
            {
                _renderers[0].State.SelectedShape = default(BaseShape);
            }

            if (_renderers?[0].State?.SelectedShapes != null)
            {
                _renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shape">The shape to select.</param>
        public void Select(XLayer layer, BaseShape shape)
        {
            Select(shape);

            if (layer?.Owner != null)
            {
                layer.Owner.CurrentShape = shape;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(XLayer layer, ImmutableHashSet<BaseShape> shapes)
        {
            Select(shapes);

            if (layer?.Owner?.CurrentShape != null)
            {
                layer.Owner.CurrentShape = default(BaseShape);
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Deselect(XLayer layer)
        {
            Deselect();

            if (layer?.Owner?.CurrentShape != null)
            {
                layer.Owner.CurrentShape = default(BaseShape);
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                if (Invalidate != null)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Try to select shape at specified coordinates.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="x">The X coordinate in layer.</param>
        /// <param name="y">The Y coordinate in layer.</param>
        /// <returns>True if selecting shape was successful.</returns>
        public bool TryToSelectShape(XLayer layer, double x, double y)
        {
            if (layer != null)
            {
                var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), _project.Options.HitThreshold);
                if (result != null)
                {
                    Select(layer, result);
                    return true;
                }

                Deselect(layer);
            }

            return false;
        }

        /// <summary>
        /// Try to select shapes inside rectangle.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <returns>True if selecting shapes was successful.</returns>
        public bool TryToSelectShapes(XLayer layer, XRectangle rectangle)
        {
            if (layer != null)
            {
                var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
                var result = ShapeHitTestSelection.HitTest(layer.Shapes, rect, _project.Options.HitThreshold);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        if (result.Count == 1)
                        {
                            Select(layer, result.FirstOrDefault());
                        }
                        else
                        {
                            Select(layer, result);
                        }
                        return true;
                    }
                }

                Deselect(layer);
            }

            return false;
        }

        /// <summary>
        /// Hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="shape">The shape to hover.</param>
        public void Hover(XLayer layer, BaseShape shape)
        {
            if (layer != null)
            {
                Select(layer, shape);
                _hover = shape;
            }
        }

        /// <summary>
        /// De-hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Dehover(XLayer layer)
        {
            if (layer != null && _hover != null)
            {
                _hover = default(BaseShape);
                Deselect(layer);
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
            if (_project?.CurrentContainer?.CurrentLayer == null)
                return false;

            if (_renderers?[0]?.State?.SelectedShapes == null
                && !(_renderers?[0]?.State?.SelectedShape != null && _hover != _renderers?[0]?.State?.SelectedShape))
            {
                var result = ShapeHitTestPoint.HitTest(_project.CurrentContainer?.CurrentLayer?.Shapes, new Vector2(x, y), _project.Options.HitThreshold);
                if (result != null)
                {
                    Hover(_project.CurrentContainer?.CurrentLayer, result);
                    return true;
                }
                else
                {
                    if (_renderers[0].State.SelectedShape != null && _renderers[0].State.SelectedShape == _hover)
                    {
                        Dehover(_project.CurrentContainer?.CurrentLayer);
                    }
                }
            }

            return false;
        }

        private void SwapLineStart(XLine line, XPoint point)
        {
            if (line?.Start != null && point != null)
            {
                var previous = line.Start;
                var next = point;
                _project?.History?.Snapshot(previous, next, (p) => line.Start = p);
                line.Start = next;
            }
        }

        private void SwapLineEnd(XLine line, XPoint point)
        {
            if (line?.End != null && point != null)
            {
                var previous = line.End;
                var next = point;
                _project?.History?.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }
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
            if (_project?.CurrentContainer == null || _project?.Options == null)
                return false;

            var result = ShapeHitTestPoint.HitTest(
                _project.CurrentContainer.CurrentLayer.Shapes,
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
                    SwapLineStart(line, point);
                }
                else
                {
                    split.Start = point;
                    split.End = line.End;
                    SwapLineEnd(line, point);
                }

                _project.AddShape(_project.CurrentContainer.CurrentLayer, split);

                if (select)
                {
                    Select(_project.CurrentContainer.CurrentLayer, point);
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
            if (_project?.Options == null)
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

                SwapLineEnd(line, p1);
            }
            else
            {
                split = XLine.Create(
                    p1,
                    line.End,
                    line.Style,
                    _project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            _project.AddShape(_project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        /// <summary>
        /// Try to connect lines to connectors.
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
                        if (LineBounds.Contains(line, new Vector2(connector.X, connector.Y), threshold, 0, 0))
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
                        bool horizontal = Abs(p0.Y - p1.Y) < threshold;
                        bool vertical = Abs(p0.X - p1.X) < threshold;

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

        private XGroup GroupWithHistory(XLayer layer, ImmutableHashSet<BaseShape> shapes, string name)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();
                var group = XGroup.Group(name, shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                _project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                return group;
            }

            return null;
        }

        private void UngroupWithHistory(XLayer layer, ImmutableHashSet<BaseShape> shapes)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();

                XGroup.Ungroup(shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                _project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        private void UngroupWithHistory(XLayer layer, BaseShape shape)
        {
            if (layer != null && shape != null)
            {
                var source = layer.Shapes.ToBuilder();

                XGroup.Ungroup(shape as XGroup, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                _project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Group shapes.
        /// </summary>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="name">The group name.</param>
        public XGroup Group(ImmutableHashSet<BaseShape> shapes, string name)
        {
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                return GroupWithHistory(layer, shapes, name);
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
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                if (shape != null && shape is XGroup)
                {
                    UngroupWithHistory(layer, shape);
                    return true;
                }

                if (shapes != null)
                {
                    UngroupWithHistory(layer, shapes);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Swap shape from source index to target index position in an array. 
        /// </summary>
        /// <param name="shape">The source shape.</param>
        /// <param name="sourceIndex">The source shape index.</param>
        /// <param name="targetIndex">The target shape index.</param>
        private void Swap(BaseShape shape, int sourceIndex, int targetIndex)
        {
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer?.Shapes != null)
            {
                if (sourceIndex < targetIndex)
                {
                    _project.SwapShape(layer, shape, targetIndex + 1, sourceIndex);
                }
                else
                {
                    if (layer.Shapes.Length + 1 > sourceIndex + 1)
                    {
                        _project.SwapShape(layer, shape, targetIndex, sourceIndex + 1);
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
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = items.Length - 1;
                if (targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringForward(BaseShape source)
        {
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex + 1;
                if (targetIndex < items.Length)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendBackward(BaseShape source)
        {
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex - 1;
                if (targetIndex >= 0)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendToBack(BaseShape source)
        {
            var layer = _project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = 0;
                if (sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
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

        private void MoveShapesByWithHistory(IEnumerable<BaseShape> shapes, double dx, double dy)
        {
            MoveShapesBy(shapes, dx, dy);

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            _project?.History?.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
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
            if (shape != null)
            {
                switch (_project?.Options?.MoveMode)
                {
                    case XMoveMode.Point:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var distinct = Enumerable.Repeat(shape, 1).SelectMany(s => s.GetPoints()).Distinct().ToList();
                                MoveShapesByWithHistory(distinct, dx, dy);
                            }
                        }
                        break;
                    case XMoveMode.Shape:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked) && !shape.State.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var items = Enumerable.Repeat(shape, 1).ToList();
                                MoveShapesByWithHistory(items, dx, dy);
                            }
                        }
                        break;
                }
            }

            if (shapes != null)
            {
                switch (_project?.Options?.MoveMode)
                {
                    case XMoveMode.Point:
                        {
                            var distinct = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked)).SelectMany(s => s.GetPoints()).Distinct().ToList();
                            MoveShapesByWithHistory(distinct, dx, dy);
                        }
                        break;
                    case XMoveMode.Shape:
                        {
                            var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                            MoveShapesByWithHistory(items, dx, dy);
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
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <summary>
        /// Check if left up action is available.
        /// </summary>
        /// <returns>True if left up action is available.</returns>
        public bool IsLeftUpAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <summary>
        /// Check if right down action is available.
        /// </summary>
        /// <returns>True if right down action is available.</returns>
        public bool IsRightDownAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <summary>
        /// Check if right up action is available.
        /// </summary>
        /// <returns>True if right up action is available.</returns>
        public bool IsRightUpAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <summary>
        /// Check if move action is available.
        /// </summary>
        /// <returns>True if move action is available.</returns>
        public bool IsMoveAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <summary>
        /// Check if selection is available.
        /// </summary>
        /// <returns>True if selection is available.</returns>
        public bool IsSelectionAvailable()
        {
            return _renderers?[0]?.State?.SelectedShape != null
                || _renderers?[0]?.State?.SelectedShapes != null;
        }

        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftDown(double x, double y)
        {
            Tools?[CurrentTool]?.LeftDown(x, y);
        }

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftUp(double x, double y)
        {
            Tools?[CurrentTool]?.LeftUp(x, y);
        }

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightDown(double x, double y)
        {
            Tools?[CurrentTool]?.RightDown(x, y);
        }

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightUp(double x, double y)
        {
            Tools?[CurrentTool]?.RightUp(x, y);
        }

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void Move(double x, double y)
        {
            Tools?[CurrentTool]?.Move(x, y);
        }

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        public void OnChangeCurrentView(ViewBase view)
        {
            if (_currentView != view)
            {
                CurrentView = view;
            }
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

            Commands.ToolCubicBezierCommand =
                Command.Create(
                    () => OnToolCubicBezier(),
                    () => IsEditMode());

            Commands.ToolQuadraticBezierCommand =
                Command.Create(
                    () => OnToolQuadraticBezier(),
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

            Commands.CloneStyleCommand =
                Command.Create(
                    () => OnToggleCloneStyle(),
                    () => IsEditMode());

            Commands.AddDatabaseCommand =
                Command.Create(
                    () =>
                    {
                        var db = XDatabase.Create(Constants.DefaultDatabaseName);
                        Project.AddDatabase(db);
                        Project.SetCurrentDatabase(db);
                    },
                    () => IsEditMode());

            Commands.RemoveDatabaseCommand =
                Command<XDatabase>.Create(
                    (db) =>
                    {
                        Project.RemoveDatabase(db);
                        Project.SetCurrentDatabase(Project.Databases.FirstOrDefault());
                    },
                    (db) => IsEditMode());

            Commands.AddColumnCommand =
                Command<XDatabase>.Create(
                    (db) => Project.AddColumn(db, XColumn.Create(db, Constants.DefaulColumnName)),
                    (db) => IsEditMode());

            Commands.RemoveColumnCommand =
                Command<XColumn>.Create(
                    (column) => Project.RemoveColumn(column),
                    (column) => IsEditMode());

            Commands.AddRecordCommand =
                Command<XDatabase>.Create(
                    (db) => Project.AddRecord(db, XRecord.Create(db, Constants.DefaulValue)),
                    (db) => IsEditMode());

            Commands.RemoveRecordCommand =
                Command<XRecord>.Create(
                    (record) => Project.RemoveRecord(record),
                    (record) => IsEditMode());

            Commands.ResetRecordCommand =
                Command<XContext>.Create(
                    (data) => Project.ResetRecord(data),
                    (data) => IsEditMode());

            Commands.ApplyRecordCommand =
                Command<XRecord>.Create(
                    (record) => OnApplyRecord(record),
                    (record) => IsEditMode());

            Commands.AddPropertyCommand =
                Command<XContext>.Create(
                    (data) => Project.AddProperty(data, XProperty.Create(data, Constants.DefaulPropertyName, Constants.DefaulValue)),
                    (data) => IsEditMode());

            Commands.RemovePropertyCommand =
                Command<XProperty>.Create(
                    (property) => Project.RemoveProperty(property),
                    (property) => IsEditMode());

            Commands.AddGroupLibraryCommand =
                Command.Create(
                    () =>
                    {
                        var gl = XLibrary<XGroup>.Create(Constants.DefaulGroupLibraryName);
                        Project.AddGroupLibrary(gl);
                        Project.SetCurrentGroupLibrary(gl);
                    },
                    () => IsEditMode());

            Commands.RemoveGroupLibraryCommand =
                Command<XLibrary<XGroup>>.Create(
                    (library) =>
                    {
                        Project.RemoveGroupLibrary(library);
                        Project.SetCurrentGroupLibrary(_project?.GroupLibraries.FirstOrDefault());
                    },
                    (library) => IsEditMode());

            Commands.AddGroupCommand =
                Command<XLibrary<XGroup>>.Create(
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
                Command<XContainer>.Create(
                    (container) => Project.AddLayer(container, XLayer.Create(Constants.DefaultLayerName, container)),
                    (container) => IsEditMode());

            Commands.RemoveLayerCommand =
                Command<XLayer>.Create(
                    (layer) =>
                    {
                        Project.RemoveLayer(layer);
                        layer.Owner.SetCurrentLayer(layer.Owner.Layers.FirstOrDefault());
                    },
                    (layer) => IsEditMode());

            Commands.AddStyleLibraryCommand =
                Command.Create(
                    () =>
                    {
                        var sl = XLibrary<ShapeStyle>.Create(Constants.DefaulStyleLibraryName);
                        Project.AddStyleLibrary(sl);
                        Project.SetCurrentStyleLibrary(sl);
                    },
                    () => IsEditMode());

            Commands.RemoveStyleLibraryCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (library) =>
                    {
                        Project.RemoveStyleLibrary(library);
                        Project.SetCurrentStyleLibrary(_project?.StyleLibraries.FirstOrDefault());
                    },
                    (library) => IsEditMode());

            Commands.AddStyleCommand =
                Command<XLibrary<ShapeStyle>>.Create(
                    (library) => Project.AddStyle(library, ShapeStyle.Create(Constants.DefaulStyleName)),
                    (library) => IsEditMode());

            Commands.RemoveStyleCommand =
                Command<ShapeStyle>.Create(
                    (style) =>
                    {
                        var library = Project.RemoveStyle(style);
                        library?.SetSelected(library?.Items.FirstOrDefault());
                    },
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
                Command<XContainer>.Create(
                    (template) => OnRemoveTemplate(template),
                    (template) => IsEditMode());

            Commands.EditTemplateCommand =
                Command<XContainer>.Create(
                    (template) => OnEditTemplate(template),
                    (template) => IsEditMode());

            Commands.ApplyTemplateCommand =
                Command<XContainer>.Create(
                    (template) => OnApplyTemplate(template),
                    (template) => true);

            Commands.AddImageKeyCommand =
                Command.Create(
                    async () => await (OnAddImageKey(null) ?? Task.FromResult(string.Empty)),
                    () => IsEditMode());

            Commands.RemoveImageKeyCommand =
                Command<string>.Create(
                    (key) => OnRemoveImageKey(key),
                    (key) => IsEditMode());

            Commands.SelectedItemChangedCommand =
                Command<XSelectable>.Create(
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

            Commands.OpenCommand =
                 Command<string>.Create(
                     async (path) => await (Application?.OnOpenAsync(path) ?? Task.FromResult<object>(null)),
                     (path) => IsEditMode());

            Commands.SaveCommand =
                Command.Create(
                    async () => await (Application?.OnSaveAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    async () => await (Application?.OnSaveAsAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ImportObjectCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportObjectAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportObjectCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportObjectAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ImportXamlCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportXamlAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportXamlCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportXamlAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ImportJsonCommand =
                Command<string>.Create(
                    async (path) => await (Application?.OnImportJsonAsync(path) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportJsonCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportJsonAsync(item) ?? Task.FromResult<object>(null)),
                    (path) => IsEditMode());

            Commands.ExportCommand =
                Command<object>.Create(
                    async (item) => await (Application?.OnExportAsync(item) ?? Task.FromResult<object>(null)),
                    (item) => IsEditMode());

            Commands.ExitCommand =
                Command.Create(
                    () => Application?.OnCloseView(),
                    () => true);

            Commands.ImportDataCommand =
                Command<XProject>.Create(
                    async (project) => await (Application?.OnImportDataAsync() ?? Task.FromResult<object>(null)),
                    (project) => IsEditMode());

            Commands.ExportDataCommand =
                Command<XDatabase>.Create(
                    async (db) => await (Application?.OnExportDataAsync() ?? Task.FromResult<object>(null)),
                    (db) => IsEditMode());

            Commands.UpdateDataCommand =
                Command<XDatabase>.Create(
                    async (db) => await (Application?.OnUpdateDataAsync() ?? Task.FromResult<object>(null)),
                    (db) => IsEditMode());

            Commands.CopyAsEmfCommand =
                Command.Create(
                    async () => await (Application?.OnCopyAsEmfAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ZoomResetCommand =
                Command.Create(
                    async () => await (Application?.OnZoomResetAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ZoomAutoFitCommand =
                Command.Create(
                    async () => await (Application?.OnZoomAutoFitAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.LoadWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnLoadWindowLayout() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.SaveWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnSaveWindowLayoutAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ResetWindowLayoutCommand =
                Command.Create(
                    async () => await (Application?.OnResetWindowLayoutAsync() ?? Task.FromResult<object>(null)),
                    () => true);

            Commands.ObjectBrowserCommand =
                Command.Create(
                    async () => await (Application?.OnShowObjectBrowserAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.DocumentViewerCommand =
                Command.Create(
                    async () => await (Application?.OnShowDocumentViewerAsync() ?? Task.FromResult<object>(null)),
                    () => IsEditMode());

            Commands.ChangeCurrentViewCommand =
                Command<ViewBase>.Create(
                    (view) => OnChangeCurrentView(view),
                    (view) => true);
        }
    }
}
