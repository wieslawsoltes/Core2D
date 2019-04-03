// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.History;
using Core2D.Editor.Input;
using Core2D.Editor.Recent;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Dock.Model;
using Dock.Model.Controls;
using Spatial;
using static System.Math;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor.
    /// </summary>
    public class ProjectEditor : ObservableObject, IProjectEditor
    {
        private readonly IServiceProvider _serviceProvider;
        private IProjectContainer _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private bool _isToolIdle;
        private IEditorTool _currentTool;
        private IPathTool _currentPathTool;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private IDock _layout;
        private AboutInfo _aboutInfo;
        private readonly Lazy<ImmutableArray<IEditorTool>> _tools;
        private readonly Lazy<ImmutableArray<IPathTool>> _pathTools;
        private readonly Lazy<IHitTest> _hitTest;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<IDataFlow> _dataFlow;
        private readonly Lazy<IShapeRenderer[]> _renderers;
        private readonly Lazy<IFileSystem> _fileIO;
        private readonly Lazy<IFactory> _factory;
        private readonly Lazy<IContainerFactory> _containerFactory;
        private readonly Lazy<IShapeFactory> _shapeFactory;
        private readonly Lazy<ITextClipboard> _textClipboard;
        private readonly Lazy<IJsonSerializer> _jsonSerializer;
        private readonly Lazy<IXamlSerializer> _xamlSerializer;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ITextFieldReader<IDatabase>> _csvReader;
        private readonly Lazy<ITextFieldWriter<IDatabase>> _csvWriter;
        private readonly Lazy<IImageImporter> _imageImporter;
        private readonly Lazy<IScriptRunner> _scriptRunner;
        private readonly Lazy<IProjectEditorPlatform> _platform;
        private readonly Lazy<IEditorCanvasPlatform> _canvasPlatform;
        private readonly Lazy<IEditorLayoutPlatform> _layoutPlatform;

        /// <inheritdoc/>
        public IProjectContainer Project
        {
            get => _project;
            set => Update(ref _project, value);
        }

        /// <inheritdoc/>
        public string ProjectPath
        {
            get => _projectPath;
            set => Update(ref _projectPath, value);
        }

        /// <inheritdoc/>
        public bool IsProjectDirty
        {
            get => _isProjectDirty;
            set => Update(ref _isProjectDirty, value);
        }

        /// <inheritdoc/>
        public ProjectObserver Observer
        {
            get => _observer;
            set => Update(ref _observer, value);
        }

        /// <inheritdoc/>
        public bool IsToolIdle
        {
            get => _isToolIdle;
            set => Update(ref _isToolIdle, value);
        }

        /// <inheritdoc/>
        public IEditorTool CurrentTool
        {
            get => _currentTool;
            set => Update(ref _currentTool, value);
        }

        /// <inheritdoc/>
        public IPathTool CurrentPathTool
        {
            get => _currentPathTool;
            set => Update(ref _currentPathTool, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<RecentFile> RecentProjects
        {
            get => _recentProjects;
            set => Update(ref _recentProjects, value);
        }

        /// <inheritdoc/>
        public RecentFile CurrentRecentProject
        {
            get => _currentRecentProject;
            set => Update(ref _currentRecentProject, value);
        }

        /// <inheritdoc/>
        public IDock Layout
        {
            get => _layout;
            set => Update(ref _layout, value);
        }

        /// <inheritdoc/>
        public AboutInfo AboutInfo
        {
            get => _aboutInfo;
            set => Update(ref _aboutInfo, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<IEditorTool> Tools => _tools.Value;

        /// <inheritdoc/>
        public ImmutableArray<IPathTool> PathTools => _pathTools.Value;

        /// <inheritdoc/>
        public IHitTest HitTest => _hitTest.Value;

        /// <inheritdoc/>
        public ILog Log => _log.Value;

        /// <inheritdoc/>
        public IDataFlow DataFlow => _dataFlow.Value;

        /// <inheritdoc/>
        public IShapeRenderer[] Renderers => _renderers.Value;

        /// <inheritdoc/>
        public IFileSystem FileIO => _fileIO.Value;

        /// <inheritdoc/>
        public IFactory Factory => _factory.Value;

        /// <inheritdoc/>
        public IContainerFactory ContainerFactory => _containerFactory.Value;

        /// <inheritdoc/>
        public IShapeFactory ShapeFactory => _shapeFactory.Value;

        /// <inheritdoc/>
        public ITextClipboard TextClipboard => _textClipboard.Value;

        /// <inheritdoc/>
        public IJsonSerializer JsonSerializer => _jsonSerializer.Value;

        /// <inheritdoc/>
        public IXamlSerializer XamlSerializer => _xamlSerializer.Value;

        /// <inheritdoc/>
        public ImmutableArray<IFileWriter> FileWriters => _fileWriters.Value;

        /// <inheritdoc/>
        public ITextFieldReader<IDatabase> CsvReader => _csvReader.Value;

        /// <inheritdoc/>
        public ITextFieldWriter<IDatabase> CsvWriter => _csvWriter.Value;

        /// <inheritdoc/>
        public IImageImporter ImageImporter => _imageImporter.Value;

        /// <inheritdoc/>
        public IScriptRunner ScriptRunner => _scriptRunner.Value;

        /// <inheritdoc/>
        public IProjectEditorPlatform Platform => _platform.Value;

        /// <inheritdoc/>
        public IEditorCanvasPlatform CanvasPlatform => _canvasPlatform.Value;

        /// <inheritdoc/>
        public IEditorLayoutPlatform LayoutPlatform => _layoutPlatform.Value;

        private object ScriptState { get; set; } = default;

        private IPageContainer PageToCopy { get; set; } = default;

        private IDocumentContainer DocumentToCopy { get; set; } = default;

        private IBaseShape HoveredShape { get; set; } = default;

        /// <summary>
        /// Initialize new instance of <see cref="ProjectEditor"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ProjectEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default;
            _tools = _serviceProvider.GetServiceLazily<IEditorTool[], ImmutableArray<IEditorTool>>((tools) => tools.Where(tool => !tool.GetType().Name.StartsWith("PathTool")).ToImmutableArray());
            _pathTools = _serviceProvider.GetServiceLazily<IPathTool[], ImmutableArray<IPathTool>>((tools) => tools.ToImmutableArray());
            _hitTest = _serviceProvider.GetServiceLazily<IHitTest>(hitTests => hitTests.Register(_serviceProvider.GetService<IBounds[]>()));
            _log = _serviceProvider.GetServiceLazily<ILog>();
            _dataFlow = _serviceProvider.GetServiceLazily<IDataFlow>();
            _renderers = new Lazy<IShapeRenderer[]>(() => new[] { _serviceProvider.GetService<IShapeRenderer>(), _serviceProvider.GetService<IShapeRenderer>() });
            _fileIO = _serviceProvider.GetServiceLazily<IFileSystem>();
            _factory = _serviceProvider.GetServiceLazily<IFactory>();
            _containerFactory = _serviceProvider.GetServiceLazily<IContainerFactory>();
            _shapeFactory = _serviceProvider.GetServiceLazily<IShapeFactory>();
            _textClipboard = _serviceProvider.GetServiceLazily<ITextClipboard>();
            _jsonSerializer = _serviceProvider.GetServiceLazily<IJsonSerializer>();
            _xamlSerializer = _serviceProvider.GetServiceLazily<IXamlSerializer>();
            _fileWriters = _serviceProvider.GetServiceLazily<IFileWriter[], ImmutableArray<IFileWriter>>((writers) => writers.ToImmutableArray());
            _csvReader = _serviceProvider.GetServiceLazily<ITextFieldReader<IDatabase>>();
            _csvWriter = _serviceProvider.GetServiceLazily<ITextFieldWriter<IDatabase>>();
            _imageImporter = _serviceProvider.GetServiceLazily<IImageImporter>();
            _scriptRunner = _serviceProvider.GetServiceLazily<IScriptRunner>();
            _platform = _serviceProvider.GetServiceLazily<IProjectEditorPlatform>();
            _canvasPlatform = _serviceProvider.GetServiceLazily<IEditorCanvasPlatform>();
            _layoutPlatform = _serviceProvider.GetServiceLazily<IEditorLayoutPlatform>();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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

        /// <inheritdoc/>
        public (double sx, double sy) TryToSnap(InputArgs args)
        {
            if (Project != null && Project.Options.SnapToGrid == true)
                return (Snap(args.X, Project.Options.SnapX), Snap(args.Y, Project.Options.SnapY));
            else
                return (args.X, args.Y);
        }

        /// <inheritdoc/>
        public string GetName(object item)
        {
            if (item != null)
            {
                if (item is IObservableObject observable)
                {
                    return observable.Name;
                }
            }
            return string.Empty;
        }

        /// <inheritdoc/>
        public void OnNew(object item)
        {
            if (item is IPageContainer page)
            {
                OnNewPage(page);
            }
            else if (item is IDocumentContainer document)
            {
                OnNewPage(document);
            }
            else if (item is IProjectContainer project)
            {
                OnNewDocument();
            }
            else if (item is IProjectEditor)
            {
                OnNewProject();
            }
            else if (item == null)
            {
                if (Project == null)
                {
                    OnNewProject();
                }
                else
                {
                    if (Project.CurrentDocument == null)
                    {
                        OnNewDocument();
                    }
                    else
                    {
                        OnNewPage(Project.CurrentDocument);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnNewPage(IPageContainer selected)
        {
            var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
            if (document != null)
            {
                var page =
                    ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                    ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

                Project?.AddPage(document, page);
                Project?.SetCurrentContainer(page);
            }
        }

        /// <inheritdoc/>
        public void OnNewPage(IDocumentContainer selected)
        {
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

            Project?.AddPage(selected, page);
            Project?.SetCurrentContainer(page);
        }

        /// <inheritdoc/>
        public void OnNewDocument()
        {
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? Factory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

            Project?.AddDocument(document);
            Project?.SetCurrentDocument(document);
            Project?.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        /// <inheritdoc/>
        public void OnNewProject()
        {
            OnUnload();
            OnLoad(ContainerFactory?.GetProject() ?? Factory.CreateProjectContainer(), string.Empty);
            OnNavigate("EditorView");
            CanvasPlatform?.Invalidate?.Invoke();
        }

        /// <inheritdoc/>
        public void OnOpenProject(string path)
        {
            try
            {
                if (FileIO != null && JsonSerializer != null)
                {
                    if (!string.IsNullOrEmpty(path) && FileIO.Exists(path))
                    {
                        var project = Factory.OpenProjectContainer(path, FileIO, JsonSerializer);
                        if (project != null)
                        {
                            OnOpenProject(project, path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnOpenProject(IProjectContainer project, string path)
        {
            try
            {
                if (project != null)
                {
                    OnUnload();
                    OnLoad(project, path);
                    OnAddRecent(path, project.Name);
                    OnNavigate("EditorView");
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnCloseProject()
        {
            OnNavigate("DashboardView");
            Project?.History?.Reset();
            OnUnload();
        }

        /// <inheritdoc/>
        public void OnSaveProject(string path)
        {
            try
            {
                if (Project != null && FileIO != null && JsonSerializer != null)
                {
                    Factory.SaveProjectContainer(Project, path, FileIO, JsonSerializer);
                    OnAddRecent(path, Project.Name);

                    if (string.IsNullOrEmpty(ProjectPath))
                    {
                        ProjectPath = path;
                    }

                    IsProjectDirty = false;
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnImportData(IProjectContainer project, string path)
        {
            try
            {
                if (project != null)
                {
                    var db = CsvReader?.Read(path, FileIO);
                    if (db != null)
                    {
                        project.AddDatabase(db);
                        project.SetCurrentDatabase(db);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnImportData(string path)
        {
            OnImportData(Project, path);
        }

        /// <inheritdoc/>
        public void OnExportData(string path, IDatabase database)
        {
            try
            {
                CsvWriter?.Write(path, FileIO, database);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnUpdateData(string path, IDatabase database)
        {
            try
            {
                var db = CsvReader?.Read(path, FileIO);
                if (db != null)
                {
                    Project?.UpdateDatabase(database, db);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnImportObject(object item, bool restore)
        {
            if (item is IShapeStyle style)
            {
                Project?.AddStyle(Project?.CurrentStyleLibrary, style);
            }
            else if (item is IList<IShapeStyle> styleList)
            {
                Project.AddItems(Project?.CurrentStyleLibrary, styleList);
            }
            else if (item is IBaseShape)
            {
                if (item is IGroupShape group)
                {
                    if (restore)
                    {
                        var shapes = Enumerable.Repeat(group, 1);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project.AddGroup(Project?.CurrentGroupLibrary, group);
                }
                else
                {
                    Project?.AddShape(Project?.CurrentContainer?.CurrentLayer, item as IBaseShape);
                }
            }
            else if (item is IList<IGroupShape> groups)
            {
                if (restore)
                {
                    TryToRestoreStyles(groups);
                    TryToRestoreRecords(groups);
                }
                Project.AddItems(Project?.CurrentGroupLibrary, groups);
            }
            else if (item is ILibrary<IShapeStyle> sl)
            {
                Project.AddStyleLibrary(sl);
            }
            else if (item is IList<ILibrary<IShapeStyle>> sll)
            {
                Project.AddStyleLibraries(sll);
            }
            else if (item is ILibrary<IGroupShape> gl)
            {
                TryToRestoreStyles(gl.Items);
                TryToRestoreRecords(gl.Items);
                Project.AddGroupLibrary(gl);
            }
            else if (item is IList<ILibrary<IGroupShape>> gll)
            {
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);
                Project.AddGroupLibraries(gll);
            }
            else if (item is IContext context)
            {
                if (Renderers?[0]?.State?.SelectedShape != null || (Renderers?[0]?.State?.SelectedShapes?.Count > 0))
                {
                    OnApplyData(context);
                }
                else
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        container.Data = context;
                    }
                }
            }
            else if (item is IDatabase db)
            {
                Project?.AddDatabase(db);
                Project?.SetCurrentDatabase(db);
            }
            else if (item is ILayerContainer layer)
            {
                if (restore)
                {
                    TryToRestoreStyles(layer.Shapes);
                    TryToRestoreRecords(layer.Shapes);
                }
                Project?.AddLayer(Project?.CurrentContainer, layer);
            }
            else if (item is IPageContainer page)
            {
                if (page.Template == null)
                {
                    // Import as template.
                    if (restore)
                    {
                        var shapes = page.Layers.SelectMany(x => x.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddTemplate(page);
                }
                else
                {
                    // Import as page.
                    if (restore)
                    {
                        var shapes = Enumerable.Concat(
                            page.Layers.SelectMany(x => x.Shapes),
                            page.Template?.Layers.SelectMany(x => x.Shapes));
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddPage(Project?.CurrentDocument, page);
                }
            }
            else if (item is IList<IPageContainer> templates)
            {
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                Project.AddTemplates(templates);
            }
            else if (item is IDocumentContainer document)
            {
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        document.Pages.SelectMany(x => x.Layers).SelectMany(x => x.Shapes),
                        document.Pages.SelectMany(x => x.Template.Layers).SelectMany(x => x.Shapes));
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }
                Project?.AddDocument(document);
            }
            else if (item is IOptions options)
            {
                if (Project != null)
                {
                    Project.Options = options;
                }
            }
            else if (item is IProjectContainer project)
            {
                OnUnload();
                OnLoad(project, string.Empty);
            }
            else
            {
                throw new NotSupportedException("Not supported import object.");
            }
        }

        /// <inheritdoc/>
        public void OnImportXaml(string path)
        {
            try
            {
                var xaml = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    OnImportXamlString(xaml);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnImportXamlString(string xaml)
        {
            var item = XamlSerializer?.Deserialize<object>(xaml);
            if (item != null)
            {
                OnImportObject(item, false);
            }
        }

        /// <inheritdoc/>
        public void OnExportXaml(string path, object item)
        {
            try
            {
                var xaml = XamlSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    FileIO?.WriteUtf8Text(path, xaml);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnImportJson(string path)
        {
            try
            {
                var json = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    OnImportJsonString(json);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        private void OnImportJsonString(string json)
        {
            var item = JsonSerializer.Deserialize<object>(json);
            if (item != null)
            {
                OnImportObject(item, true);
            }
        }

        /// <inheritdoc/>
        public void OnExportJson(string path, object item)
        {
            try
            {
                var json = JsonSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    FileIO?.WriteUtf8Text(path, json);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnExport(string path, object item, IFileWriter writer)
        {
            try
            {
                writer?.Save(path, item, Project);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnExecuteCode(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    ScriptRunner?.Execute(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnExecuteRepl(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    ScriptState = ScriptRunner?.Execute(csharp, ScriptState);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnResetRepl()
        {
            ScriptState = null;
        }

        /// <inheritdoc/>
        public void OnExecuteScript(string path)
        {
            try
            {
                var csharp = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    OnExecuteCode(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnExecuteScript(string[] paths)
        {
            foreach (var path in paths)
            {
                OnExecuteScript(path);
            }
        }

        /// <inheritdoc/>
        public void OnUndo()
        {
            try
            {
                if (Project?.History.CanUndo() ?? false)
                {
                    Deselect();
                    Project?.History.Undo();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnRedo()
        {
            try
            {
                if (Project?.History.CanRedo() ?? false)
                {
                    Deselect();
                    Project?.History.Redo();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnCut(object item)
        {
            if (item is IPageContainer page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
                Project?.RemovePage(page);
                Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is IDocumentContainer document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is IProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    OnCopy(item);
                    OnDeleteSelected();
                }
            }
        }

        /// <inheritdoc/>
        public void OnCopy(object item)
        {
            if (item is IPageContainer page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
            }
            else if (item is IDocumentContainer document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
            }
            else if (item is IProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    if (Renderers?[0]?.State?.SelectedShape != null)
                    {
                        OnCopyShapes(Enumerable.Repeat(Renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (Renderers?[0]?.State?.SelectedShapes != null)
                    {
                        OnCopyShapes(Renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async void OnPaste(object item)
        {
            if (Project != null && item is IPageContainer page)
            {
                if (PageToCopy != null)
                {
                    var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                    if (document != null)
                    {
                        int index = document.Pages.IndexOf(page);
                        var clone = Clone(PageToCopy);
                        Project.ReplacePage(document, clone, index);
                        Project.SetCurrentContainer(clone);
                    }
                }
            }
            else if (Project != null && item is IDocumentContainer document)
            {
                if (PageToCopy != null)
                {
                    var clone = Clone(PageToCopy);
                    Project?.AddPage(document, clone);
                    Project.SetCurrentContainer(clone);
                }
                else if (DocumentToCopy != null)
                {
                    int index = Project.Documents.IndexOf(document);
                    var clone = Clone(DocumentToCopy);
                    Project.ReplaceDocument(clone, index);
                    Project.SetCurrentDocument(clone);
                    Project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
                }
            }
            else if (item is IProjectEditor || item == null)
            {
                if (await CanPaste())
                {
                    var text = await (TextClipboard?.GetText() ?? Task.FromResult(string.Empty));
                    if (!string.IsNullOrEmpty(text))
                    {
                        OnTryPaste(text);
                    }
                }
            }
            else if (item is string text)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    OnTryPaste(text);
                }
            }
        }

        /// <inheritdoc/>
        public void OnDelete(object item)
        {
            if (item is ILayerContainer layer)
            {
                Project?.RemoveLayer(layer);

                var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                if (layer.Owner is IPageContainer owner)
                {
                    owner.SetCurrentLayer(selected);
                }
            }
            if (item is IPageContainer page)
            {
                Project?.RemovePage(page);

                var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                Project?.SetCurrentContainer(selected);
            }
            else if (item is IDocumentContainer document)
            {
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is IProjectEditor || item == null)
            {
                OnDeleteSelected();
            }
        }

        /// <inheritdoc/>
        public void OnSelectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                Select(
                    Project?.CurrentContainer?.CurrentLayer,
                    ImmutableHashSet.CreateRange<IBaseShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnDeselectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnClearAll()
        {
            try
            {
                var container = Project?.CurrentContainer;
                if (container != null)
                {
                    foreach (var layer in container.Layers)
                    {
                        Project?.ClearLayer(layer);
                    }

                    container.WorkingLayer.Shapes = ImmutableArray.Create<IBaseShape>();
                    container.HelperLayer.Shapes = ImmutableArray.Create<IBaseShape>();

                    Project.CurrentContainer.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnCancel()
        {
            OnDeselectAll();
            OnResetTool();
        }

        /// <inheritdoc/>
        public void OnGroupSelected()
        {
            var group = Group(Renderers?[0]?.State?.SelectedShapes, ProjectEditorConfiguration.DefaulGroupName);
            if (group != null)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        /// <inheritdoc/>
        public void OnUngroupSelected()
        {
            var result = Ungroup(Renderers?[0]?.State?.SelectedShape, Renderers?[0]?.State?.SelectedShapes);
            if (result == true && Renderers?[0]?.State != null)
            {
                Renderers[0].State.SelectedShape = null;
                Renderers[0].State.SelectedShapes = null;
            }
        }

        /// <inheritdoc/>
        public void OnBringToFrontSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringToFront(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        /// <inheritdoc/>
        public void OnBringForwardSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringForward(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSendBackwardSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendBackward(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSendToBackSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendToBack(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        /// <inheritdoc/>
        public void OnMoveUpSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                0.0,
                Project.Options.SnapToGrid ? -Project.Options.SnapY : -1.0);
        }

        /// <inheritdoc/>
        public void OnMoveDownSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                0.0,
                Project.Options.SnapToGrid ? Project.Options.SnapY : 1.0);
        }

        /// <inheritdoc/>
        public void OnMoveLeftSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                Project.Options.SnapToGrid ? -Project.Options.SnapX : -1.0,
                0.0);
        }

        /// <inheritdoc/>
        public void OnMoveRightSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                Project.Options.SnapToGrid ? Project.Options.SnapX : 1.0,
                0.0);
        }

        /// <inheritdoc/>
        public void OnToolNone()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "None");
        }

        /// <inheritdoc/>
        public void OnToolSelection()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Selection");
        }

        /// <inheritdoc/>
        public void OnToolPoint()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Point");
        }

        /// <inheritdoc/>
        public void OnToolLine()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Line")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Line");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Line");
            }
        }

        /// <inheritdoc/>
        public void OnToolArc()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Arc")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Arc");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Arc");
            }
        }

        /// <inheritdoc/>
        public void OnToolCubicBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "CubicBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
        }

        /// <inheritdoc/>
        public void OnToolQuadraticBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "QuadraticBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
        }

        /// <inheritdoc/>
        public void OnToolPath()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Path");
        }

        /// <inheritdoc/>
        public void OnToolRectangle()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Rectangle");
        }

        /// <inheritdoc/>
        public void OnToolEllipse()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Ellipse");
        }

        /// <inheritdoc/>
        public void OnToolText()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Text");
        }

        /// <inheritdoc/>
        public void OnToolImage()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Image");
        }

        /// <inheritdoc/>
        public void OnToolMove()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Move")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Move");
            }
        }

        /// <inheritdoc/>
        public void OnResetTool()
        {
            CurrentTool?.Reset();
        }

        /// <inheritdoc/>
        public void OnToggleDefaultIsStroked()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsStroked = !Project.Options.DefaultIsStroked;
            }
        }

        /// <inheritdoc/>
        public void OnToggleDefaultIsFilled()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsFilled = !Project.Options.DefaultIsFilled;
            }
        }

        /// <inheritdoc/>
        public void OnToggleDefaultIsClosed()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsClosed = !Project.Options.DefaultIsClosed;
            }
        }

        /// <inheritdoc/>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsSmoothJoin = !Project.Options.DefaultIsSmoothJoin;
            }
        }

        /// <inheritdoc/>
        public void OnToggleSnapToGrid()
        {
            if (Project?.Options != null)
            {
                Project.Options.SnapToGrid = !Project.Options.SnapToGrid;
            }
        }

        /// <inheritdoc/>
        public void OnToggleTryToConnect()
        {
            if (Project?.Options != null)
            {
                Project.Options.TryToConnect = !Project.Options.TryToConnect;
            }
        }

        /// <inheritdoc/>
        public void OnToggleCloneStyle()
        {
            if (Project?.Options != null)
            {
                Project.Options.CloneStyle = !Project.Options.CloneStyle;
            }
        }

        /// <inheritdoc/>
        public void OnAddDatabase()
        {
            var db = Factory.CreateDatabase(ProjectEditorConfiguration.DefaultDatabaseName);
            Project.AddDatabase(db);
            Project.SetCurrentDatabase(db);
        }

        /// <inheritdoc/>
        public void OnRemoveDatabase(IDatabase db)
        {
            Project.RemoveDatabase(db);
            Project.SetCurrentDatabase(Project.Databases.FirstOrDefault());
        }

        /// <inheritdoc/>
        public void OnAddColumn(IDatabase db)
        {
            Project.AddColumn(db, Factory.CreateColumn(db, ProjectEditorConfiguration.DefaulColumnName));
        }

        /// <inheritdoc/>
        public void OnRemoveColumn(IColumn column)
        {
            Project.RemoveColumn(column);
        }

        /// <inheritdoc/>
        public void OnAddRecord(IDatabase db)
        {
            Project.AddRecord(db, Factory.CreateRecord(db, ProjectEditorConfiguration.DefaulValue));
        }

        /// <inheritdoc/>
        public void OnRemoveRecord(IRecord record)
        {
            Project.RemoveRecord(record);
        }

        /// <inheritdoc/>
        public void OnResetRecord(IContext data)
        {
            Project.ResetRecord(data);
        }

        /// <inheritdoc/>
        public void OnApplyRecord(IRecord record)
        {
            if (record != null)
            {
                // Selected shape.
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyRecord(Renderers[0].State.SelectedShape?.Data, record);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project.ApplyRecord(shape.Data, record);
                    }
                }

                // Current page.
                if (Renderers[0].State.SelectedShape == null && Renderers[0].State.SelectedShapes == null)
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        Project?.ApplyRecord(container.Data, record);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnAddProperty(IContext data)
        {
            Project.AddProperty(data, Factory.CreateProperty(data, ProjectEditorConfiguration.DefaulPropertyName, ProjectEditorConfiguration.DefaulValue));
        }

        /// <inheritdoc/>
        public void OnRemoveProperty(IProperty property)
        {
            Project.RemoveProperty(property);
        }

        /// <inheritdoc/>
        public void OnAddGroupLibrary()
        {
            var gl = Factory.CreateLibrary<IGroupShape>(ProjectEditorConfiguration.DefaulGroupLibraryName);
            Project.AddGroupLibrary(gl);
            Project.SetCurrentGroupLibrary(gl);
        }

        /// <inheritdoc/>
        public void OnRemoveGroupLibrary(ILibrary<IGroupShape> library)
        {
            Project.RemoveGroupLibrary(library);
            Project.SetCurrentGroupLibrary(Project?.GroupLibraries.FirstOrDefault());
        }

        /// <inheritdoc/>
        public void OnAddGroup(ILibrary<IGroupShape> library)
        {
            if (Project != null && library != null)
            {
                if (Renderers?[0]?.State?.SelectedShape is IGroupShape group)
                {
                    var clone = CloneShape(group);
                    if (clone != null)
                    {
                        Project?.AddGroup(library, clone);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnRemoveGroup(IGroupShape group)
        {
            if (Project != null && group != null)
            {
                var library = Project.RemoveGroup(group);
                library?.SetSelected(library?.Items.FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public void OnInsertGroup(IGroupShape group)
        {
            if (Project?.CurrentContainer != null)
            {
                OnDropShapeAsClone(group, 0.0, 0.0);
            }
        }

        /// <inheritdoc/>
        public void OnAddLayer(IPageContainer container)
        {
            Project.AddLayer(container, Factory.CreateLayerContainer(ProjectEditorConfiguration.DefaultLayerName, container));
        }

        /// <inheritdoc/>
        public void OnRemoveLayer(ILayerContainer layer)
        {
            Project.RemoveLayer(layer);
            if (layer.Owner is IPageContainer owner)
            {
                owner.SetCurrentLayer(owner.Layers.FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public void OnAddStyleLibrary()
        {
            var sl = Factory.CreateLibrary<IShapeStyle>(ProjectEditorConfiguration.DefaulStyleLibraryName);
            Project.AddStyleLibrary(sl);
            Project.SetCurrentStyleLibrary(sl);
        }

        /// <inheritdoc/>
        public void OnRemoveStyleLibrary(ILibrary<IShapeStyle> library)
        {
            Project.RemoveStyleLibrary(library);
            Project.SetCurrentStyleLibrary(Project?.StyleLibraries.FirstOrDefault());
        }

        /// <inheritdoc/>
        public void OnAddStyle(ILibrary<IShapeStyle> library)
        {
            Project.AddStyle(library, Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName));
        }

        /// <inheritdoc/>
        public void OnRemoveStyle(IShapeStyle style)
        {
            var library = Project.RemoveStyle(style);
            library?.SetSelected(library?.Items.FirstOrDefault());
        }

        /// <inheritdoc/>
        public void OnApplyStyle(IShapeStyle style)
        {
            if (style != null)
            {
                // Selected shape.
                if (Renderers[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyStyle(Renderers[0].State.SelectedShape, style);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project?.ApplyStyle(shape, style);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnApplyData(IContext data)
        {
            if (data != null)
            {
                // Selected shape.
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyData(Renderers[0].State.SelectedShape, data);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project?.ApplyData(shape, data);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnAddShape(IBaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.AddShape(layer, shape);
            }
        }

        /// <inheritdoc/>
        public void OnRemoveShape(IBaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.RemoveShape(layer, shape);
                Project.CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
            }
        }

        /// <inheritdoc/>
        public void OnAddTemplate()
        {
            if (Project != null)
            {
                var template = ContainerFactory.GetTemplate(Project, "Empty");
                if (template == null)
                {
                    template = Factory.CreateTemplateContainer(ProjectEditorConfiguration.DefaultTemplateName);
                }

                Project.AddTemplate(template);
            }
        }

        /// <inheritdoc/>
        public void OnRemoveTemplate(IPageContainer template)
        {
            if (template != null)
            {
                Project?.RemoveTemplate(template);
                Project?.SetCurrentTemplate(Project?.Templates.FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public void OnEditTemplate(IPageContainer template)
        {
            if (Project != null && template != null)
            {
                Project.SetCurrentContainer(template);
                Project.CurrentContainer?.Invalidate();
            }
        }

        /// <inheritdoc/>
        public void OnApplyTemplate(IPageContainer template)
        {
            var page = Project?.CurrentContainer;
            if (page != null && template != null && page != template)
            {
                Project.ApplyTemplate(page, template);
                Project.CurrentContainer.Invalidate();
            }
        }

        /// <inheritdoc/>
        public async Task<string> OnAddImageKey(string path)
        {
            if (Project != null)
            {
                if (path == null || string.IsNullOrEmpty(path))
                {
                    var key = await (ImageImporter.GetImageKeyAsync() ?? Task.FromResult(string.Empty));
                    if (key == null || string.IsNullOrEmpty(key))
                        return null;

                    return key;
                }
                else
                {
                    byte[] bytes;
                    using (var stream = FileIO?.Open(path))
                    {
                        bytes = FileIO?.ReadBinary(stream);
                    }
                    if (Project is IImageCache imageCache)
                    {
                        var key = imageCache.AddImageFromFile(path, bytes);
                        return key;
                    }
                    return null;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public void OnRemoveImageKey(string key)
        {
            if (key != null)
            {
                if (Project is IImageCache imageCache)
                {
                    imageCache.RemoveImage(key);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSelectedItemChanged(IObservableObject item)
        {
            if (Project != null)
            {
                Project.Selected = item;
            }
        }

        /// <inheritdoc/>
        public void OnAddPage(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                var page =
                    ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                    ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

                Project.AddPage(Project.CurrentDocument, page);
                Project.SetCurrentContainer(page);
            }
        }

        /// <inheritdoc/>
        public void OnInsertPageBefore(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is IPageContainer selected)
                {
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                        ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

                    Project.AddPageAt(Project.CurrentDocument, page, index);
                    Project.SetCurrentContainer(page);
                }
            }
        }

        /// <inheritdoc/>
        public void OnInsertPageAfter(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is IPageContainer selected)
                {
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                        ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

                    Project.AddPageAt(Project.CurrentDocument, page, index + 1);
                    Project.SetCurrentContainer(page);
                }
            }
        }

        /// <inheritdoc/>
        public void OnAddDocument(object item)
        {
            if (Project != null)
            {
                var document =
                    ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                    ?? Factory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

                Project.AddDocument(document);
                Project.SetCurrentDocument(document);
                Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
            }
        }

        /// <inheritdoc/>
        public void OnInsertDocumentBefore(object item)
        {
            if (Project != null)
            {
                if (item is IDocumentContainer selected)
                {
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                        ?? Factory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

                    Project.AddDocumentAt(document, index);
                    Project.SetCurrentDocument(document);
                    Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
        }

        /// <inheritdoc/>
        public void OnInsertDocumentAfter(object item)
        {
            if (Project != null)
            {
                if (item is IDocumentContainer selected)
                {
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                        ?? Factory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

                    Project.AddDocumentAt(document, index + 1);
                    Project.SetCurrentDocument(document);
                    Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
        }

        /// <inheritdoc/>
        private void SetRenderersImageCache(IImageCache cache)
        {
            if (Renderers != null)
            {
                foreach (var renderer in Renderers)
                {
                    renderer.ClearCache(isZooming: false);
                    renderer.State.ImageCache = cache;
                }
            }
        }

        /// <inheritdoc/>
        public void OnLoad(IProjectContainer project, string path = null)
        {
            if (project != null)
            {
                Deselect();
                if (project is IImageCache imageCache)
                {
                    SetRenderersImageCache(imageCache);
                }
                Project = project;
                Project.History = new StackHistory();
                ProjectPath = path;
                IsProjectDirty = false;
                Observer = new ProjectObserver(this);
            }
        }

        /// <inheritdoc/>
        public void OnUnload()
        {
            if (Observer != null)
            {
                Observer?.Dispose();
                Observer = null;
            }

            if (Project?.History != null)
            {
                Project.History.Reset();
                Project.History = null;
            }

            if (Project != null)
            {
                if (Project is IImageCache imageCache)
                {
                    imageCache.PurgeUnusedImages(Enumerable.Empty<string>().ToImmutableHashSet());
                }
                Deselect();
                SetRenderersImageCache(null);
                Project = null;
                ProjectPath = string.Empty;
                IsProjectDirty = false;
                GC.Collect();
            }
        }

        /// <inheritdoc/>
        public void OnInvalidateCache(bool isZooming)
        {
            try
            {
                if (Renderers != null)
                {
                    foreach (var renderer in Renderers)
                    {
                        renderer.ClearCache(isZooming);
                    }
                }

                Project?.CurrentContainer?.Invalidate();
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnAddRecent(string path, string name)
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

        /// <inheritdoc/>
        public void OnLoadRecent(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = FileIO.ReadUtf8Text(path);
                    var recent = JsonSerializer.Deserialize<Recents>(json);
                    if (recent != null)
                    {
                        var remove = recent.Files.Where(x => FileIO?.Exists(x.Path) == false).ToList();
                        var builder = recent.Files.ToBuilder();

                        foreach (var file in remove)
                        {
                            builder.Remove(file);
                        }

                        RecentProjects = builder.ToImmutable();

                        if (recent.Current != null
                            && (FileIO?.Exists(recent.Current.Path) ?? false))
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
                    Log?.LogException(ex);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSaveRecent(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var recent = Recents.Create(_recentProjects, _currentRecentProject);
                    var json = JsonSerializer.Serialize(recent);
                    FileIO.WriteUtf8Text(path, json);
                }
                catch (Exception ex)
                {
                    Log?.LogException(ex);
                }
            }
        }

        /// <inheritdoc/>
        public void OnLoadLayout(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = FileIO.ReadUtf8Text(path);
                    var layout = JsonSerializer.Deserialize<RootDock>(json);
                    if (layout != null)
                    {
                        Layout = layout;
                    }
                }
                catch (Exception ex)
                {
                    Log?.LogException(ex);
                }
            }
        }

        /// <inheritdoc/>
        public void OnSaveLayout(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_layout);
                    FileIO.WriteUtf8Text(path, json);
                }
                catch (Exception ex)
                {
                    Log?.LogException(ex);
                }
            }
        }

        /// <inheritdoc/>
        public void OnNavigate(object view)
        {
            if (Layout is IDock dock)
            {
                dock.Navigate(view);
            }
        }

        /// <inheritdoc/>
        public bool CanUndo()
        {
            return Project?.History?.CanUndo() ?? false;
        }

        /// <inheritdoc/>
        public bool CanRedo()
        {
            return Project?.History?.CanRedo() ?? false;
        }

        /// <inheritdoc/>
        public bool CanCopy()
        {
            return Renderers?[0]?.State?.SelectedShape != null
                || Renderers?[0]?.State?.SelectedShapes != null;
        }

        /// <inheritdoc/>
        public async Task<bool> CanPaste()
        {
            try
            {
                return await (TextClipboard?.ContainsText() ?? Task.FromResult(false));
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        /// <inheritdoc/>
        public void OnCopyShapes(IList<IBaseShape> shapes)
        {
            try
            {
                var json = JsonSerializer?.Serialize(shapes);
                if (!string.IsNullOrEmpty(json))
                {
                    TextClipboard?.SetText(json);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnTryPaste(string text)
        {
            try
            {
                var exception = default(Exception);

                // Try to deserialize Xaml.
                try
                {
                    if (XamlSerializer != null)
                    {
                        OnImportXamlString(text);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Try to deserialize Json.
                try
                {
                    var shapes = JsonSerializer?.Deserialize<IList<IBaseShape>>(text);
                    if (shapes?.Count() > 0)
                    {
                        OnPasteShapes(shapes);
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
                Log?.LogException(ex);
            }
        }

        private void ResetPointShapeToDefault(IEnumerable<IBaseShape> shapes)
        {
            foreach (var point in shapes?.SelectMany(s => s?.GetPoints()))
            {
                point.Shape = Project?.Options?.PointShape;
            }
        }

        /// <summary>
        /// Generated style dictionary with name as the key.
        /// </summary>
        /// <returns>The style dictionary with name as the key.</returns>
        private IDictionary<string, IShapeStyle> GenerateStyleDictionaryByName()
        {
            return Project?.StyleLibraries
                .Where(sl => sl?.Items != null && sl?.Items.Length > 0)
                .SelectMany(sl => sl.Items)
                .Distinct(new ShapeStyleByNameComparer())
                .ToDictionary(s => s.Name);
        }

        /// <summary>
        /// Try to restore shape styles.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreStyles(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                if (Project?.StyleLibraries == null)
                    return;

                var styles = GenerateStyleDictionaryByName();

                // Reset point shape to defaults.
                ResetPointShapeToDefault(shapes);

                // Try to restore shape styles.
                foreach (var shape in ProjectContainer.GetAllShapes(shapes))
                {
                    if (shape?.Style == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(shape.Style.Name))
                    {
                        if (styles.TryGetValue(shape.Style.Name, out var style))
                        {
                            // Use existing style.
                            shape.Style = style;
                        }
                        else
                        {
                            // Create Imported style library.
                            if (Project?.CurrentStyleLibrary == null)
                            {
                                var sl = Factory.CreateLibrary<IShapeStyle>(ProjectEditorConfiguration.ImportedStyleLibraryName);
                                Project.AddStyleLibrary(sl);
                                Project.SetCurrentStyleLibrary(sl);
                            }

                            // Add missing style.
                            Project?.AddStyle(Project?.CurrentStyleLibrary, shape.Style);

                            // Recreate styles dictionary.
                            styles = GenerateStyleDictionaryByName();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <summary>
        /// Generates record dictionary with id as the key.
        /// </summary>
        /// <returns>The record dictionary with id as the key.</returns>
        private IDictionary<string, IRecord> GenerateRecordDictionaryById()
        {
            return Project?.Databases
                .Where(d => d?.Records != null && d?.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
        }

        /// <summary>
        /// Try to restore shape records.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreRecords(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                if (Project?.Databases == null)
                    return;

                var records = GenerateRecordDictionaryById();

                // Try to restore shape record.
                foreach (var shape in ProjectContainer.GetAllShapes(shapes))
                {
                    if (shape?.Data?.Record == null)
                        continue;

                    if (records.TryGetValue((string)shape.Data.Record.Id, out var record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (Project?.CurrentDatabase == null && shape.Data.Record.Owner is IDatabase owner)
                        {
                            var db = Factory.CreateDatabase(
                                ProjectEditorConfiguration.ImportedDatabaseName,
                                (ImmutableArray<IColumn>)owner.Columns);
                            Project.AddDatabase(db);
                            Project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Data.Record.Owner = Project.CurrentDatabase;
                        Project?.AddRecord(Project?.CurrentDatabase, (IRecord)shape.Data.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordDictionaryById();
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <summary>
        /// Restore shape project references afer closing.
        /// </summary>
        /// <param name="shape">The shape to restore.</param>
        private void RestoreShape(IBaseShape shape)
        {
            var shapes = Enumerable.Repeat(shape, 1).ToList();
            TryToRestoreStyles(shapes);
            TryToRestoreRecords(shapes);
        }

        /// <inheritdoc/>
        public void OnPasteShapes(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                Project.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);

                OnSelect(shapes);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public void OnSelect(IEnumerable<IBaseShape> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(Project?.CurrentContainer?.CurrentLayer, ImmutableHashSet.CreateRange<IBaseShape>(shapes));
            }
        }

        /// <inheritdoc/>
        public T CloneShape<T>(T shape) where T : IBaseShape
        {
            try
            {
                if (JsonSerializer is IJsonSerializer serializer)
                {
                    var json = serializer.Serialize(shape);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var clone = serializer.Deserialize<T>(json);
                        if (clone != null)
                        {
                            RestoreShape(clone);
                            return clone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        /// <inheritdoc/>
        public ILayerContainer Clone(ILayerContainer container)
        {
            try
            {
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<ILayerContainer>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Shapes;
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        /// <inheritdoc/>
        public IPageContainer Clone(IPageContainer container)
        {
            try
            {
                var template = container?.Template;
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<IPageContainer>(json);
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
                Log?.LogException(ex);
            }

            return default;
        }

        /// <inheritdoc/>
        public IDocumentContainer Clone(IDocumentContainer document)
        {
            try
            {
                var templates = document?.Pages.Select(c => c?.Template)?.ToArray();
                var json = JsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<IDocumentContainer>(json);
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
                Log?.LogException(ex);
            }

            return default;
        }

        /// <inheritdoc/>
        public bool OnDropFiles(string[] files)
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

                        if (string.Compare(ext, ProjectEditorConfiguration.ProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnOpenProject(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ProjectEditorConfiguration.CsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ProjectEditorConfiguration.JsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportJson(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ProjectEditorConfiguration.XamlExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportXaml(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ProjectEditorConfiguration.ScriptExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnExecuteScript(path);
                            result = true;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool OnDropShape(IBaseShape shape, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    var target = Renderers[0].State.SelectedShape;
                    if (target is IPointShape point)
                    {
                        if (bExecute == true)
                        {
                            point.Shape = shape;
                        }
                        return true;
                    }
                }
                else if (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var target in Renderers[0].State.SelectedShapes)
                    {
                        if (target is IPointShape point)
                        {
                            if (bExecute == true)
                            {
                                point.Shape = shape;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var point = HitTest.TryToGetPoint(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (point != null)
                        {
                            if (bExecute == true)
                            {
                                point.Shape = shape;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (bExecute == true)
                            {
                                OnDropShapeAsClone(shape, x, y);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        /// <inheritdoc/>
        public void OnDropShapeAsClone<T>(T shape, double x, double y) where T : IBaseShape
        {
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            try
            {
                var clone = CloneShape(shape);
                if (clone != null)
                {
                    Deselect(Project?.CurrentContainer?.CurrentLayer);
                    clone.Move(null, sx, sy);

                    Project.AddShape(Project?.CurrentContainer?.CurrentLayer, clone);

                    // Select(Project?.CurrentContainer?.CurrentLayer, clone);

                    if (Project.Options.TryToConnect)
                    {
                        if (clone is IGroupShape group)
                        {
                            TryToConnectLines(
                                ProjectContainer.GetAllShapes<ILineShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes),
                                group.Connectors,
                                Project.Options.HitThreshold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        /// <inheritdoc/>
        public bool OnDropRecord(IRecord record, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    if (bExecute)
                    {
                        OnApplyRecord(record);
                    }
                    return true;
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            if (bExecute)
                            {
                                Project?.ApplyRecord(result.Data, record);
                            }
                            return true;
                        }
                        else
                        {
                            if (bExecute)
                            {
                                OnDropRecordAsGroup(record, x, y);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        /// <inheritdoc/>
        public void OnDropRecordAsGroup(IRecord record, double x, double y)
        {
            var selected = Project.CurrentStyleLibrary.Selected;
            var style = Project.Options.CloneStyle ? (IShapeStyle)selected.Copy(null) : selected;
            var point = Project?.Options?.PointShape;
            var layer = Project?.CurrentContainer?.CurrentLayer;
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            var g = Factory.CreateGroupShape(ProjectEditorConfiguration.DefaulGroupName);

            g.Data.Record = record;

            var length = record.Values.Length;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            var db = record.Owner as IDatabase;

            for (int i = 0; i < length; i++)
            {
                var column = db.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + db.Columns[i].Name + "}";
                    var text = Factory.CreateTextShape(px, py, px + width, py + height, style, point, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = Factory.CreateRectangleShape(sx, sy, sx + width, sy + (length * height), style, point);
            g.AddShape(rectangle);

            var pt = Factory.CreatePointShape(sx + (width / 2), sy, point);
            var pb = Factory.CreatePointShape(sx + (width / 2), sy + (length * height), point);
            var pl = Factory.CreatePointShape(sx, sy + ((length * height) / 2), point);
            var pr = Factory.CreatePointShape(sx + width, sy + ((length * height) / 2), point);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            Project.AddShape(layer, g);
        }

        /// <inheritdoc/>
        public bool OnDropStyle(IShapeStyle style, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    if (bExecute == true)
                    {
                        OnApplyStyle(style);
                    }
                    return true;
                }
                else
                {
                    var layer = Project.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            if (bExecute == true)
                            {
                                Project.ApplyStyle(result, style);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        /// <inheritdoc/>
        public bool OnDropTemplate(IPageContainer template, double x, double y, bool bExecute = true)
        {
            try
            {
                var page = Project?.CurrentContainer;
                if (page != null && template != null && page != template)
                {
                    if (bExecute)
                    {
                        OnApplyTemplate(template);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        /// <inheritdoc/>
        public void OnDeleteSelected()
        {
            if (Project?.CurrentContainer?.CurrentLayer == null || Renderers?[0]?.State == null)
                return;

            if (Renderers[0].State.SelectedShape != null)
            {
                var layer = Project.CurrentContainer.CurrentLayer;

                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(Renderers[0].State.SelectedShape);
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Renderers[0].State.SelectedShape = default;
                layer.Invalidate();
            }

            if (Renderers[0].State.SelectedShapes != null && Renderers[0].State.SelectedShapes.Count > 0)
            {
                var layer = Project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in Renderers[0].State.SelectedShapes)
                {
                    builder.Remove(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Renderers[0].State.SelectedShapes = default;
                layer.Invalidate();
            }
        }

        /// <inheritdoc/>
        public void Select(IBaseShape shape)
        {
            if (Renderers?[0]?.State != null)
            {
                Renderers[0].State.SelectedShape = shape;

                if (Renderers[0].State.SelectedShapes != null)
                {
                    Renderers[0].State.SelectedShapes = default;
                }
            }
        }

        /// <inheritdoc/>
        public void Select(ISet<IBaseShape> shapes)
        {
            if (Renderers?[0]?.State != null)
            {
                if (Renderers[0].State.SelectedShape != null)
                {
                    Renderers[0].State.SelectedShape = default;
                }

                Renderers[0].State.SelectedShapes = shapes;
            }
        }

        /// <inheritdoc/>
        public void Deselect()
        {
            if (Renderers?[0].State?.SelectedShape != null)
            {
                Renderers[0].State.SelectedShape = default;
            }

            if (Renderers?[0].State?.SelectedShapes != null)
            {
                Renderers[0].State.SelectedShapes = default;
            }
        }

        /// <inheritdoc/>
        public void Select(ILayerContainer layer, IBaseShape shape)
        {
            Select(shape);

            if (layer.Owner is IPageContainer owner)
            {
                owner.CurrentShape = shape;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                CanvasPlatform?.Invalidate?.Invoke();
            }
        }

        /// <inheritdoc/>
        public void Select(ILayerContainer layer, ISet<IBaseShape> shapes)
        {
            Select(shapes);

            if (layer.Owner is IPageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                CanvasPlatform?.Invalidate?.Invoke();
            }
        }

        /// <inheritdoc/>
        public void Deselect(ILayerContainer layer)
        {
            Deselect();

            if (layer.Owner is IPageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                if (CanvasPlatform?.Invalidate != null)
                {
                    CanvasPlatform?.Invalidate();
                }
            }
        }

        /// <inheritdoc/>
        public bool TryToSelectShape(ILayerContainer layer, double x, double y)
        {
            if (layer != null)
            {
                var point = HitTest.TryToGetPoint(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (point != null)
                {
                    Select(layer, point);
                    return true;
                }

                var shape = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (shape != null)
                {
                    Select(layer, shape);
                    return true;
                }

                Deselect(layer);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryToSelectShapes(ILayerContainer layer, IRectangleShape rectangle)
        {
            if (layer != null)
            {
                var rect = Rect2.FromPoints(
                    rectangle.TopLeft.X,
                    rectangle.TopLeft.Y,
                    rectangle.BottomRight.X,
                    rectangle.BottomRight.Y);
                var result = HitTest.TryToGetShapes(layer.Shapes, rect, Project.Options.HitThreshold);
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
                            Select(layer, result.ToImmutableHashSet());
                        }
                        return true;
                    }
                }

                Deselect(layer);
            }

            return false;
        }

        /// <inheritdoc/>
        public void Hover(ILayerContainer layer, IBaseShape shape)
        {
            if (layer != null)
            {
                Select(layer, shape);
                HoveredShape = shape;
            }
        }

        /// <inheritdoc/>
        public void Dehover(ILayerContainer layer)
        {
            if (layer != null && HoveredShape != null)
            {
                HoveredShape = default;
                Deselect(layer);
            }
        }

        /// <inheritdoc/>
        public bool TryToHoverShape(double x, double y)
        {
            if (Project?.CurrentContainer?.CurrentLayer == null)
                return false;

            if (Renderers?[0]?.State?.SelectedShapes == null
                && !(Renderers?[0]?.State?.SelectedShape != null && HoveredShape != Renderers?[0]?.State?.SelectedShape))
            {
                var point = HitTest.TryToGetPoint(Project.CurrentContainer?.CurrentLayer?.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (point != null)
                {
                    Hover(Project.CurrentContainer?.CurrentLayer, point);
                    return true;
                }
                else
                {
                    var shape = HitTest.TryToGetShape(Project.CurrentContainer?.CurrentLayer?.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                    if (shape != null)
                    {
                        Hover(Project.CurrentContainer?.CurrentLayer, shape);
                        return true;
                    }
                    else
                    {
                        if (Renderers[0].State.SelectedShape != null && Renderers[0].State.SelectedShape == HoveredShape)
                        {
                            Dehover(Project.CurrentContainer?.CurrentLayer);
                        }
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public IPointShape TryToGetConnectionPoint(double x, double y)
        {
            if (Project.Options.TryToConnect)
            {
                return HitTest.TryToGetPoint(
                    Project.CurrentContainer.CurrentLayer.Shapes,
                    new Point2(x, y),
                    Project.Options.HitThreshold);
            }
            return null;
        }

        private void SwapLineStart(ILineShape line, IPointShape point)
        {
            if (line?.Start != null && point != null)
            {
                var previous = line.Start;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.Start = p);
                line.Start = next;
            }
        }

        private void SwapLineEnd(ILineShape line, IPointShape point)
        {
            if (line?.End != null && point != null)
            {
                var previous = line.End;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }
        }

        /// <inheritdoc/>
        public bool TryToSplitLine(double x, double y, IPointShape point, bool select = false)
        {
            if (Project?.CurrentContainer == null || Project?.Options == null)
                return false;

            var result = HitTest.TryToGetShape(
                Project.CurrentContainer.CurrentLayer.Shapes,
                new Point2(x, y),
                Project.Options.HitThreshold);

            if (result is ILineShape line)
            {
                if (!Project.Options.SnapToGrid)
                {
                    var a = new Point2(line.Start.X, line.Start.Y);
                    var b = new Point2(line.End.X, line.End.Y);
                    var target = new Point2(x, y);
                    var nearest = target.NearestOnLine(a, b);
                    point.X = nearest.X;
                    point.Y = nearest.Y;
                }

                var split = Factory.CreateLineShape(
                    x, y,
                    line.Style,
                    Project.Options.PointShape,
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

                Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

                if (select)
                {
                    Select(Project.CurrentContainer.CurrentLayer, point);
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryToSplitLine(ILineShape line, IPointShape p0, IPointShape p1)
        {
            if (Project?.Options == null)
                return false;

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            ILineShape split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = Factory.CreateLineShape(
                    p0,
                    line.End,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = Factory.CreateLineShape(
                    p1,
                    line.End,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        /// <inheritdoc/>
        public bool TryToConnectLines(IEnumerable<ILineShape> lines, ImmutableArray<IPointShape> connectors, double threshold)
        {
            if (connectors.Length > 0)
            {
                var lineToPoints = new Dictionary<ILineShape, IList<IPointShape>>();

                // Find possible connector to line connections.
                foreach (var connector in connectors)
                {
                    ILineShape result = null;
                    foreach (var line in lines)
                    {
                        if (HitTest.Contains(line, new Point2(connector.X, connector.Y), threshold))
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
                            lineToPoints.Add(result, new List<IPointShape>());
                            lineToPoints[result].Add(connector);
                        }
                    }
                }

                // Try to split lines using connectors.
                bool success = false;
                foreach (var kv in lineToPoints)
                {
                    var line = kv.Key;
                    var points = kv.Value;
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

        private IGroupShape Group(ILayerContainer layer, ISet<IBaseShape> shapes, string name)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();
                var group = Factory.CreateGroupShape(name);
                group.Group(shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                return group;
            }

            return null;
        }

        private void Ungroup(ILayerContainer layer, ISet<IBaseShape> shapes)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is IGroupShape group)
                    {
                        group.Ungroup(source);
                    }
                }

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        private void Ungroup(ILayerContainer layer, IGroupShape group)
        {
            if (layer != null && group != null)
            {
                var source = layer.Shapes.ToBuilder();

                group.Ungroup(source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <inheritdoc/>
        public IGroupShape Group(ISet<IBaseShape> shapes, string name)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                return Group(layer, shapes, name);
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Ungroup(IBaseShape shape, ISet<IBaseShape> shapes)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                if (shape != null && shape is IGroupShape group)
                {
                    Ungroup(layer, group);
                    return true;
                }

                if (shapes != null)
                {
                    Ungroup(layer, shapes);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        private void Swap(IBaseShape shape, int sourceIndex, int targetIndex)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer?.Shapes != null)
            {
                if (sourceIndex < targetIndex)
                {
                    Project.SwapShape(layer, shape, targetIndex + 1, sourceIndex);
                }
                else
                {
                    if (layer.Shapes.Length + 1 > sourceIndex + 1)
                    {
                        Project.SwapShape(layer, shape, targetIndex, sourceIndex + 1);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void BringToFront(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
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

        /// <inheritdoc/>
        public void BringForward(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
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

        /// <inheritdoc/>
        public void SendBackward(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
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

        /// <inheritdoc/>
        public void SendToBack(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
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

        /// <inheritdoc/>
        public void MoveShapesBy(IEnumerable<IBaseShape> shapes, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(null, dx, dy);
                }
            }
        }

        private void MoveShapesByWithHistory(IEnumerable<IBaseShape> shapes, double dx, double dy)
        {
            MoveShapesBy(shapes, dx, dy);

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            Project?.History?.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
        }

        /// <inheritdoc/>
        public void MoveBy(IBaseShape shape, ISet<IBaseShape> shapes, double dx, double dy)
        {
            if (shape != null)
            {
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var distinct = Enumerable.Repeat(shape, 1).SelectMany(s => s.GetPoints()).Distinct().ToList();
                                MoveShapesByWithHistory(distinct, dx, dy);
                            }
                        }
                        break;
                    case MoveMode.Shape:
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
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var distinct = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked)).SelectMany(s => s.GetPoints()).Distinct().ToList();
                            MoveShapesByWithHistory(distinct, dx, dy);
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                            MoveShapesByWithHistory(items, dx, dy);
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public void MoveItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = library.Items[sourceIndex];
                var builder = library.Items.ToBuilder();
                builder.Insert(targetIndex + 1, item);
                builder.RemoveAt(sourceIndex);

                var previous = library.Items;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (library.Items.Length + 1 > removeIndex)
                {
                    var item = library.Items[sourceIndex];
                    var builder = library.Items.ToBuilder();
                    builder.Insert(targetIndex, item);
                    builder.RemoveAt(removeIndex);

                    var previous = library.Items;
                    var next = builder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
            }
        }

        /// <inheritdoc/>
        public void SwapItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex)
        {
            var item1 = library.Items[sourceIndex];
            var item2 = library.Items[targetIndex];
            var builder = library.Items.ToBuilder();
            builder[targetIndex] = item1;
            builder[sourceIndex] = item2;

            var previous = library.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
            library.Items = next;
        }
    }
}
