using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor.History;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools.Decorators;
using Core2D.Input;
using Core2D.Layout;
using Core2D.Renderer;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using static System.Math;

namespace Core2D.Editor
{
    public class ProjectEditor : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private ShapeEditor _shapeEditor;
        private ProjectContainer _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private bool _isToolIdle;
        private IEditorTool _currentTool;
        private IPathTool _currentPathTool;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private AboutInfo _aboutInfo;
        private readonly Lazy<ImmutableArray<IEditorTool>> _tools;
        private readonly Lazy<ImmutableArray<IPathTool>> _pathTools;
        private readonly Lazy<IHitTest> _hitTest;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<DataFlow> _dataFlow;
        private readonly Lazy<IShapeRenderer> _pageRenderer;
        private readonly Lazy<IShapeRenderer> _documentRenderer;
        private readonly Lazy<IFileSystem> _fileIO;
        private readonly Lazy<IFactory> _factory;
        private readonly Lazy<IContainerFactory> _containerFactory;
        private readonly Lazy<IShapeFactory> _shapeFactory;
        private readonly Lazy<ITextClipboard> _textClipboard;
        private readonly Lazy<IJsonSerializer> _jsonSerializer;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ImmutableArray<ITextFieldReader<Database>>> _textFieldReaders;
        private readonly Lazy<ImmutableArray<ITextFieldWriter<Database>>> _textFieldWriters;
        private readonly Lazy<IImageImporter> _imageImporter;
        private readonly Lazy<IScriptRunner> _scriptRunner;
        private readonly Lazy<IProjectEditorPlatform> _platform;
        private readonly Lazy<IEditorCanvasPlatform> _canvasPlatform;
        private readonly Lazy<StyleEditor> _styleEditor;
        private readonly Lazy<IPathConverter> _pathConverter;
        private readonly Lazy<ISvgConverter> _svgConverter;

        public ProjectContainer Project
        {
            get => _project;
            set => RaiseAndSetIfChanged(ref _project, value);
        }

        public string ProjectPath
        {
            get => _projectPath;
            set => RaiseAndSetIfChanged(ref _projectPath, value);
        }

        public bool IsProjectDirty
        {
            get => _isProjectDirty;
            set => RaiseAndSetIfChanged(ref _isProjectDirty, value);
        }

        public ProjectObserver Observer
        {
            get => _observer;
            set => RaiseAndSetIfChanged(ref _observer, value);
        }

        public bool IsToolIdle
        {
            get => _isToolIdle;
            set => RaiseAndSetIfChanged(ref _isToolIdle, value);
        }

        public IEditorTool CurrentTool
        {
            get => _currentTool;
            set => RaiseAndSetIfChanged(ref _currentTool, value);
        }

        public IPathTool CurrentPathTool
        {
            get => _currentPathTool;
            set => RaiseAndSetIfChanged(ref _currentPathTool, value);
        }

        public ImmutableArray<RecentFile> RecentProjects
        {
            get => _recentProjects;
            set => RaiseAndSetIfChanged(ref _recentProjects, value);
        }

        public RecentFile CurrentRecentProject
        {
            get => _currentRecentProject;
            set => RaiseAndSetIfChanged(ref _currentRecentProject, value);
        }

        public AboutInfo AboutInfo
        {
            get => _aboutInfo;
            set => RaiseAndSetIfChanged(ref _aboutInfo, value);
        }

        public ImmutableArray<IEditorTool> Tools => _tools.Value;

        public ImmutableArray<IPathTool> PathTools => _pathTools.Value;

        public IHitTest HitTest => _hitTest.Value;

        public ILog Log => _log.Value;

        public DataFlow DataFlow => _dataFlow.Value;

        public IShapeRenderer PageRenderer => _pageRenderer.Value;

        public ShapeRendererState PageState => _pageRenderer.Value?.State;

        public IShapeRenderer DocumentRenderer => _documentRenderer.Value;

        public ShapeRendererState DocumentState => _documentRenderer.Value?.State;

        public IFileSystem FileIO => _fileIO.Value;

        public IFactory Factory => _factory.Value;

        public IContainerFactory ContainerFactory => _containerFactory.Value;

        public IShapeFactory ShapeFactory => _shapeFactory.Value;

        public ITextClipboard TextClipboard => _textClipboard.Value;

        public IJsonSerializer JsonSerializer => _jsonSerializer.Value;

        public ImmutableArray<IFileWriter> FileWriters => _fileWriters.Value;

        public ImmutableArray<ITextFieldReader<Database>> TextFieldReaders => _textFieldReaders.Value;

        public ImmutableArray<ITextFieldWriter<Database>> TextFieldWriters => _textFieldWriters.Value;

        public IImageImporter ImageImporter => _imageImporter.Value;

        public IScriptRunner ScriptRunner => _scriptRunner.Value;

        public IProjectEditorPlatform Platform => _platform.Value;

        public IEditorCanvasPlatform CanvasPlatform => _canvasPlatform.Value;

        public StyleEditor StyleEditor => _styleEditor.Value;

        public IPathConverter PathConverter => _pathConverter.Value;

        public ISvgConverter SvgConverter => _svgConverter.Value;

        private object ScriptState { get; set; } = default;

        private PageContainer PageToCopy { get; set; } = default;

        private DocumentContainer DocumentToCopy { get; set; } = default;

        private BaseShape HoveredShape { get; set; } = default;

        public ProjectEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _shapeEditor = new ShapeEditor(_serviceProvider);
            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default;
            _tools = _serviceProvider.GetServiceLazily<IEditorTool[], ImmutableArray<IEditorTool>>((tools) => tools.Where(tool => !tool.GetType().Name.StartsWith("PathTool")).ToImmutableArray());
            _pathTools = _serviceProvider.GetServiceLazily<IPathTool[], ImmutableArray<IPathTool>>((tools) => tools.ToImmutableArray());
            _hitTest = _serviceProvider.GetServiceLazily<IHitTest>(hitTests => hitTests.Register(_serviceProvider.GetService<IBounds[]>()));
            _log = _serviceProvider.GetServiceLazily<ILog>();
            _dataFlow = _serviceProvider.GetServiceLazily<DataFlow>();
            _pageRenderer = _serviceProvider.GetServiceLazily<IShapeRenderer>();
            _documentRenderer = _serviceProvider.GetServiceLazily<IShapeRenderer>();
            _fileIO = _serviceProvider.GetServiceLazily<IFileSystem>();
            _factory = _serviceProvider.GetServiceLazily<IFactory>();
            _containerFactory = _serviceProvider.GetServiceLazily<IContainerFactory>();
            _shapeFactory = _serviceProvider.GetServiceLazily<IShapeFactory>();
            _textClipboard = _serviceProvider.GetServiceLazily<ITextClipboard>();
            _jsonSerializer = _serviceProvider.GetServiceLazily<IJsonSerializer>();
            _fileWriters = _serviceProvider.GetServiceLazily<IFileWriter[], ImmutableArray<IFileWriter>>((writers) => writers.ToImmutableArray());
            _textFieldReaders = _serviceProvider.GetServiceLazily<ITextFieldReader<Database>[], ImmutableArray<ITextFieldReader<Database>>>((readers) => readers.ToImmutableArray());
            _textFieldWriters = _serviceProvider.GetServiceLazily<ITextFieldWriter<Database>[], ImmutableArray<ITextFieldWriter<Database>>>((writers) => writers.ToImmutableArray());
            _imageImporter = _serviceProvider.GetServiceLazily<IImageImporter>();
            _scriptRunner = _serviceProvider.GetServiceLazily<IScriptRunner>();
            _platform = _serviceProvider.GetServiceLazily<IProjectEditorPlatform>();
            _canvasPlatform = _serviceProvider.GetServiceLazily<IEditorCanvasPlatform>();
            _styleEditor = _serviceProvider.GetServiceLazily<StyleEditor>();
            _pathConverter = _serviceProvider.GetServiceLazily<IPathConverter>();
            _svgConverter = _serviceProvider.GetServiceLazily<ISvgConverter>();
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public (decimal sx, decimal sy) TryToSnap(InputArgs args)
        {
            if (Project != null && Project.Options.SnapToGrid == true)
            {
                return (
                    PointUtil.Snap((decimal)args.X, (decimal)Project.Options.SnapX),
                    PointUtil.Snap((decimal)args.Y, (decimal)Project.Options.SnapY));
            }
            else
            {
                return ((decimal)args.X, (decimal)args.Y);
            }
        }

        public string GetName(object item)
        {
            if (item != null)
            {
                if (item is ObservableObject observable)
                {
                    return observable.Name;
                }
            }
            return string.Empty;
        }

        public void OnNew(object item)
        {
            if (item is LayerContainer layer)
            {
                if (layer.Owner is PageContainer page)
                {
                    OnAddLayer(page);
                }
            }
            else if (item is PageContainer page)
            {
                OnNewPage(page);
            }
            else if (item is DocumentContainer document)
            {
                OnNewPage(document);
            }
            else if (item is ProjectContainer)
            {
                OnNewDocument();
            }
            else if (item is ProjectEditor)
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

        public void OnNewPage(PageContainer selected)
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

        public void OnNewPage(DocumentContainer selected)
        {
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? Factory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

            Project?.AddPage(selected, page);
            Project?.SetCurrentContainer(page);
        }

        public void OnNewDocument()
        {
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? Factory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

            Project?.AddDocument(document);
            Project?.SetCurrentDocument(document);
            Project?.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        public void OnNewProject()
        {
            OnUnload();
            OnLoad(ContainerFactory?.GetProject() ?? Factory.CreateProjectContainer(), string.Empty);
            CanvasPlatform?.ResetZoom?.Invoke();
            CanvasPlatform?.InvalidateControl?.Invoke();
        }

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

        public void OnOpenProject(ProjectContainer project, string path)
        {
            try
            {
                if (project != null)
                {
                    OnUnload();
                    OnLoad(project, path);
                    OnAddRecent(path, project.Name);
                    CanvasPlatform?.ResetZoom?.Invoke();
                    CanvasPlatform?.InvalidateControl?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnCloseProject()
        {
            Project?.History?.Reset();
            OnUnload();
        }

        public void OnSaveProject(string path)
        {
            try
            {
                if (Project != null && FileIO != null && JsonSerializer != null)
                {
                    var isDecoratorVisible = PageState.Decorator?.IsVisible == true;
                    if (isDecoratorVisible)
                    {
                        OnHideDecorator();
                    }

                    Factory.SaveProjectContainer(Project, path, FileIO, JsonSerializer);
                    OnAddRecent(path, Project.Name);

                    if (string.IsNullOrEmpty(ProjectPath))
                    {
                        ProjectPath = path;
                    }

                    IsProjectDirty = false;

                    if (isDecoratorVisible)
                    {
                        OnShowDecorator();
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnImportData(ProjectContainer project, string path, ITextFieldReader<Database> reader)
        {
            try
            {
                if (project != null)
                {
                    using var stream = FileIO.Open(path);
                    var db = reader?.Read(stream);
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

        public void OnExportData(string path, Database database, ITextFieldWriter<Database> writer)
        {
            try
            {
                using var stream = FileIO.Create(path);
                writer?.Write(stream, database);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnUpdateData(string path, Database database, ITextFieldReader<Database> reader)
        {
            try
            {
                using var stream = FileIO.Open(path);
                var db = reader?.Read(stream);
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

        public void OnImportObject(object item, bool restore)
        {
            if (item is ShapeStyle style)
            {
                Project?.AddStyle(Project?.CurrentStyleLibrary, style);
            }
            else if (item is IList<ShapeStyle> styleList)
            {
                Project.AddItems(Project?.CurrentStyleLibrary, styleList);
            }
            else if (item is BaseShape)
            {
                if (item is GroupShape group)
                {
                    if (restore)
                    {
                        var shapes = Enumerable.Repeat(group, 1);
                        TryToRestoreRecords(shapes);
                    }
                    Project.AddGroup(Project?.CurrentGroupLibrary, group);
                }
                else
                {
                    Project?.AddShape(Project?.CurrentContainer?.CurrentLayer, item as BaseShape);
                }
            }
            else if (item is IList<GroupShape> groups)
            {
                if (restore)
                {
                    TryToRestoreRecords(groups);
                }
                Project.AddItems(Project?.CurrentGroupLibrary, groups);
            }
            else if (item is Library<ShapeStyle> sl)
            {
                Project.AddStyleLibrary(sl);
            }
            else if (item is IList<Library<ShapeStyle>> sll)
            {
                Project.AddStyleLibraries(sll);
            }
            else if (item is Library<GroupShape> gl)
            {
                TryToRestoreRecords(gl.Items);
                Project.AddGroupLibrary(gl);
            }
            else if (item is IList<Library<GroupShape>> gll)
            {
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreRecords(shapes);
                Project.AddGroupLibraries(gll);
            }
            else if (item is Database db)
            {
                Project?.AddDatabase(db);
                Project?.SetCurrentDatabase(db);
            }
            else if (item is LayerContainer layer)
            {
                if (restore)
                {
                    TryToRestoreRecords(layer.Shapes);
                }
                Project?.AddLayer(Project?.CurrentContainer, layer);
            }
            else if (item is PageContainer page)
            {
                if (page.Template == null)
                {
                    // Import as template.
                    if (restore)
                    {
                        var shapes = page.Layers.SelectMany(x => x.Shapes);
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
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddPage(Project?.CurrentDocument, page);
                }
            }
            else if (item is IList<PageContainer> templates)
            {
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                Project.AddTemplates(templates);
            }
            else if (item is IList<Script> scripts)
            {
                // Import as scripts.
                Project.AddScripts(scripts);
            }
            else if (item is Script script)
            {
                Project?.AddScript(script);
            }
            else if (item is DocumentContainer document)
            {
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        document.Pages.SelectMany(x => x.Layers).SelectMany(x => x.Shapes),
                        document.Pages.SelectMany(x => x.Template.Layers).SelectMany(x => x.Shapes));
                    TryToRestoreRecords(shapes);
                }
                Project?.AddDocument(document);
            }
            else if (item is Options options)
            {
                if (Project != null)
                {
                    Project.Options = options;
                }
            }
            else if (item is ProjectContainer project)
            {
                OnUnload();
                OnLoad(project, string.Empty);
            }
            else
            {
                throw new NotSupportedException("Not supported import object.");
            }
        }

        public void OnImportJson(string path)
        {
            try
            {
                var json = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var item = JsonSerializer.Deserialize<object>(json);
                    if (item != null)
                    {
                        OnImportObject(item, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnImportSvg(string path)
        {
            if (SvgConverter == null)
            {
                return;
            }
            var shapes = SvgConverter.Convert(path, out _, out _);
            if (shapes != null)
            {
                OnPasteShapes(shapes);
            }
        }

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

        public void OnExport(string path, object item, IFileWriter writer)
        {
            try
            {
                using var stream = FileIO.Create(path);
                writer?.Save(stream, item, Project);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteCode(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await ScriptRunner?.Execute(csharp, null);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteRepl(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    ScriptState = await ScriptRunner?.Execute(csharp, ScriptState);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnResetRepl()
        {
            ScriptState = null;
        }

        public async Task OnExecuteScriptFile(string path)
        {
            try
            {
                var csharp = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await OnExecuteCode(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteScriptFile(string[] paths)
        {
            foreach (var path in paths)
            {
                await OnExecuteScriptFile(path);
            }
        }

        public async Task OnExecuteScript(Script script)
        {
            try
            {
                var csharp = script?.Code;
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await OnExecuteRepl(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

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

        public void OnCut(object item)
        {
            if (item is PageContainer page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
                Project?.RemovePage(page);
                Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is DocumentContainer document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    OnCopy(item);
                    OnDeleteSelected();
                }
            }
        }

        public void OnCopy(object item)
        {
            if (item is PageContainer page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
            }
            else if (item is DocumentContainer document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
            }
            else if (item is ProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    if (PageState?.SelectedShapes != null)
                    {
                        OnCopyShapes(PageState.SelectedShapes.ToList());
                    }
                }
            }
        }

        public async void OnPaste(object item)
        {
            if (Project != null && item is PageContainer page)
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
            else if (Project != null && item is DocumentContainer document)
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
            else if (item is ProjectEditor || item == null)
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

        public void OnDelete(object item)
        {
            if (item is LayerContainer layer)
            {
                Project?.RemoveLayer(layer);

                var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                if (layer.Owner is PageContainer owner)
                {
                    owner.SetCurrentLayer(selected);
                }
            }
            if (item is PageContainer page)
            {
                Project?.RemovePage(page);

                var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                Project?.SetCurrentContainer(selected);
            }
            else if (item is DocumentContainer document)
            {
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
            {
                OnDeleteSelected();
            }
        }

        public void OnShowDecorator()
        {
            if (PageState == null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            var shapes = PageState.SelectedShapes?.ToList();
            if (shapes == null || shapes.Count <= 0)
            {
                return;
            }

            if (PageState.Decorator == null)
            {
                PageState.Decorator = new BoxDecorator(_serviceProvider);
            }

            PageState.Decorator.Layer = Project.CurrentContainer.WorkingLayer;
            PageState.Decorator.Shapes = shapes;
            PageState.Decorator.Update(true);
            PageState.Decorator.Show();
        }

        public void OnUpdateDecorator()
        {
            if (PageState == null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            if (PageState.Decorator != null)
            {
                PageState.Decorator.Update(false);
            }
        }

        public void OnHideDecorator()
        {
            if (PageState == null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            if (PageState.Decorator != null)
            {
                PageState.Decorator.Hide();
            }
        }

        public void OnShowOrHideDecorator()
        {
            if (PageState == null)
            {
                return;
            }

            if (PageState.DrawDecorators == false)
            {
                return;
            }

            if (PageState.SelectedShapes?.Count == 1 && PageState.SelectedShapes?.FirstOrDefault() is PointShape)
            {
                OnHideDecorator();
                return;
            }

            if (PageState.SelectedShapes?.Count == 1 && PageState.SelectedShapes?.FirstOrDefault() is LineShape)
            {
                OnHideDecorator();
                return;
            }

            if (PageState.SelectedShapes != null)
            {
                OnShowDecorator();
            }
            else
            {
                OnHideDecorator();
            }
        }

        public void OnSelectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                Select(
                    Project?.CurrentContainer?.CurrentLayer,
                    new HashSet<BaseShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnDeselectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                OnUpdateDecorator();
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

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

                    container.WorkingLayer.Shapes = ImmutableArray.Create<BaseShape>();
                    container.HelperLayer.Shapes = ImmutableArray.Create<BaseShape>();

                    Project.CurrentContainer.InvalidateLayer();
                    OnHideDecorator();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnCancel()
        {
            OnDeselectAll();
            OnResetTool();
        }

        public void OnDuplicateSelected()
        {
            try
            {
                if (PageState?.SelectedShapes == null)
                {
                    return;
                }

                var json = JsonSerializer?.Serialize(PageState.SelectedShapes.ToList());
                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                var shapes = JsonSerializer?.Deserialize<IList<BaseShape>>(json);
                if (shapes?.Count() <= 0)
                {
                    return;
                }

                OnPasteShapes(shapes);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnGroupSelected()
        {
            var group = Group(PageState?.SelectedShapes, ProjectEditorConfiguration.DefaulGroupName);
            if (group != null)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        public void OnUngroupSelected()
        {
            var result = Ungroup(PageState?.SelectedShapes);
            if (result == true && PageState != null)
            {
                PageState.SelectedShapes = null;
                OnHideDecorator();
            }
        }

        public void OnRotateSelected(string degrees)
        {
            if (!double.TryParse(degrees, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                BoxLayout.Rotate(sources, (decimal)value, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnFlipHorizontalSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                BoxLayout.Flip(sources, FlipMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnFlipVerticalSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                BoxLayout.Flip(sources, FlipMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnMoveUpSelected()
        {
            MoveBy(
                PageState?.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapY : -1m);
        }

        public void OnMoveDownSelected()
        {
            MoveBy(
                PageState?.SelectedShapes,
                0m,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapY : 1m);
        }

        public void OnMoveLeftSelected()
        {
            MoveBy(
                PageState?.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)-Project.Options.SnapX : -1m,
                0m);
        }

        public void OnMoveRightSelected()
        {
            MoveBy(
                PageState?.SelectedShapes,
                Project.Options.SnapToGrid ? (decimal)Project.Options.SnapX : 1m,
                0m);
        }

        public void OnStackHorizontallySelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnStackVerticallySelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Stack(items, StackMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnDistributeHorizontallySelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Horizontal, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnDistributeVerticallySelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Distribute(items, DistributeMode.Vertical, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignLeftSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Left, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignCenteredSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Centered, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignRightSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Right, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignTopSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Top, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignCenterSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Center, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnAlignBottomSelected()
        {
            var shapes = PageState?.SelectedShapes;
            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                BoxLayout.Align(items, AlignMode.Bottom, Project?.History);
                OnUpdateDecorator();
            }
        }

        public void OnBringToFrontSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        public void OnBringForwardSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        public void OnSendBackwardSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        public void OnSendToBackSelected()
        {
            var sources = PageState?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        public void OnCreatePath()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            var source = PageState?.SelectedShapes?.FirstOrDefault();

            if (sources != null && sources.Count == 1)
            {
                var path = PathConverter.ToPathShape(source);
                if (path != null)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources != null && sources.Count > 1)
            {
                var path = PathConverter.ToPathShape(sources);
                if (path == null)
                {
                    return;
                }

                var shapesBuilder = layer.Shapes.ToBuilder();

                foreach (var shape in sources)
                {
                    shapesBuilder.Remove(shape);
                }
                shapesBuilder.Add(path);

                var previous = layer.Shapes;
                var next = shapesBuilder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Select(layer, path);
            }
        }

        public void OnCreateStrokePath()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            var source = PageState?.SelectedShapes?.FirstOrDefault();

            if (sources != null && sources.Count == 1)
            {
                var path = PathConverter.ToStrokePathShape(source);
                if (path != null)
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources != null && sources.Count > 1)
            {
                var paths = new List<PathShape>();
                var shapes = new List<BaseShape>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToStrokePathShape(s);
                    if (path != null)
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShape>(paths));
                }
            }
        }

        public void OnCreateFillPath()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            var source = PageState?.SelectedShapes?.FirstOrDefault();

            if (sources != null && sources.Count == 1)
            {
                var path = PathConverter.ToFillPathShape(source);
                if (path != null)
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources != null && sources.Count > 1)
            {
                var paths = new List<PathShape>();
                var shapes = new List<BaseShape>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToFillPathShape(s);
                    if (path != null)
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShape>(paths));
                }
            }
        }

        public void OnCreateWindingPath()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            var source = PageState?.SelectedShapes?.FirstOrDefault();

            if (sources != null && sources.Count == 1)
            {
                var path = PathConverter.ToWindingPathShape(source);
                if (path != null)
                {
                    path.IsStroked = false;
                    path.IsFilled = true;

                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources != null && sources.Count > 1)
            {
                var paths = new List<PathShape>();
                var shapes = new List<BaseShape>();

                foreach (var s in sources)
                {
                    var path = PathConverter.ToWindingPathShape(s);
                    if (path != null)
                    {
                        path.IsStroked = false;
                        path.IsFilled = true;

                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShape>(paths));
                }
            }
        }

        public void OnPathSimplify()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            var source = PageState?.SelectedShapes?.FirstOrDefault();

            if (sources != null && sources.Count == 1)
            {
                var path = PathConverter.Simplify(source);
                if (path != null)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    var index = shapesBuilder.IndexOf(source);
                    shapesBuilder[index] = path;

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, path);
                }
            }

            if (sources != null && sources.Count > 1)
            {
                var paths = new List<PathShape>();
                var shapes = new List<BaseShape>();

                foreach (var s in sources)
                {
                    var path = PathConverter.Simplify(s);
                    if (path != null)
                    {
                        paths.Add(path);
                        shapes.Add(s);
                    }
                }

                if (paths.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        var index = shapesBuilder.IndexOf(shapes[i]);
                        shapesBuilder[index] = paths[i];
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShape>(paths));
                }
            }
        }

        public void OnPathBreak()
        {
            if (PathConverter == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;

            if (sources != null && sources.Count >= 1)
            {
                var result = new List<BaseShape>();
                var remove = new List<BaseShape>();

                foreach (var s in sources)
                {
                    _shapeEditor.BreakShape(s, result, remove);
                }

                if (result.Count > 0)
                {
                    var shapesBuilder = layer.Shapes.ToBuilder();

                    for (int i = 0; i < remove.Count; i++)
                    {
                        shapesBuilder.Remove(remove[i]);
                    }

                    for (int i = 0; i < result.Count; i++)
                    {
                        shapesBuilder.Add(result[i]);
                    }

                    var previous = layer.Shapes;
                    var next = shapesBuilder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    Select(layer, new HashSet<BaseShape>(result));
                }
            }
        }

        public void OnPathOp(string op)
        {
            if (!Enum.TryParse<PathOp>(op, true, out var pathOp))
            {
                return;
            }

            if (PathConverter == null)
            {
                return;
            }

            var sources = PageState?.SelectedShapes;
            if (sources == null)
            {
                return;
            }

            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer == null)
            {
                return;
            }

            var path = PathConverter.Op(sources, pathOp);
            if (path == null)
            {
                return;
            }

            var shapesBuilder = layer.Shapes.ToBuilder();
            foreach (var shape in sources)
            {
                shapesBuilder.Remove(shape);
            }
            shapesBuilder.Add(path);

            var previous = layer.Shapes;
            var next = shapesBuilder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;

            Select(layer, path);
        }

        public void OnToolNone()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "None");
        }

        public void OnToolSelection()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Selection");
        }

        public void OnToolPoint()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Point");
        }

        public void OnToolLine()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Line")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Line");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Line");
            }
        }

        public void OnToolArc()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Arc")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Arc");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Arc");
            }
        }

        public void OnToolCubicBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "CubicBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
        }

        public void OnToolQuadraticBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "QuadraticBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
        }

        public void OnToolPath()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Path");
        }

        public void OnToolRectangle()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Rectangle");
        }

        public void OnToolEllipse()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Ellipse");
        }

        public void OnToolText()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Text");
        }

        public void OnToolImage()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Image");
        }

        public void OnToolMove()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Move")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Move");
            }
        }

        public void OnResetTool()
        {
            CurrentTool?.Reset();
        }

        public void OnToggleDefaultIsStroked()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsStroked = !Project.Options.DefaultIsStroked;
            }
        }

        public void OnToggleDefaultIsFilled()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsFilled = !Project.Options.DefaultIsFilled;
            }
        }

        public void OnToggleDefaultIsClosed()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsClosed = !Project.Options.DefaultIsClosed;
            }
        }

        public void OnToggleSnapToGrid()
        {
            if (Project?.Options != null)
            {
                Project.Options.SnapToGrid = !Project.Options.SnapToGrid;
            }
        }

        public void OnToggleTryToConnect()
        {
            if (Project?.Options != null)
            {
                Project.Options.TryToConnect = !Project.Options.TryToConnect;
            }
        }

        public void OnAddDatabase()
        {
            var db = Factory.CreateDatabase(ProjectEditorConfiguration.DefaultDatabaseName);
            Project.AddDatabase(db);
            Project.SetCurrentDatabase(db);
        }

        public void OnRemoveDatabase(Database db)
        {
            Project.RemoveDatabase(db);
            Project.SetCurrentDatabase(Project.Databases.FirstOrDefault());
        }

        public void OnAddColumn(Database db)
        {
            Project.AddColumn(db, Factory.CreateColumn(db, ProjectEditorConfiguration.DefaulColumnName));
        }

        public void OnRemoveColumn(Column column)
        {
            Project.RemoveColumn(column);
        }

        public void OnAddRecord(Database db)
        {
            Project.AddRecord(db, Factory.CreateRecord(db, ProjectEditorConfiguration.DefaulValue));
        }

        public void OnRemoveRecord(Record record)
        {
            Project.RemoveRecord(record);
        }

        public void OnResetRecord(IDataObject data)
        {
            Project.ResetRecord(data);
        }

        public void OnApplyRecord(Record record)
        {
            if (record != null)
            {
                if (PageState?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in PageState.SelectedShapes)
                    {
                        Project.ApplyRecord(shape, record);
                    }
                }

                if (PageState.SelectedShapes == null)
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        Project?.ApplyRecord(container, record);
                    }
                }
            }
        }

        public void OnAddProperty(ObservableObject owner)
        {
            if (owner is IDataObject data)
            {
                Project.AddProperty(data, Factory.CreateProperty(owner, ProjectEditorConfiguration.DefaulPropertyName, ProjectEditorConfiguration.DefaulValue));
            }
        }

        public void OnRemoveProperty(Property property)
        {
            Project.RemoveProperty(property);
        }

        public void OnAddGroupLibrary()
        {
            var gl = Factory.CreateLibrary<GroupShape>(ProjectEditorConfiguration.DefaulGroupLibraryName);
            Project.AddGroupLibrary(gl);
            Project.SetCurrentGroupLibrary(gl);
        }

        public void OnRemoveGroupLibrary(Library<GroupShape> library)
        {
            Project.RemoveGroupLibrary(library);
            Project.SetCurrentGroupLibrary(Project?.GroupLibraries.FirstOrDefault());
        }

        public void OnAddGroup(Library<GroupShape> library)
        {
            if (Project != null && library != null)
            {
                if (PageState.SelectedShapes?.Count == 1 && PageState.SelectedShapes?.FirstOrDefault() is GroupShape group)
                {
                    var clone = CloneShape(group);
                    if (clone != null)
                    {
                        Project?.AddGroup(library, clone);
                    }
                }
            }
        }

        public void OnRemoveGroup(GroupShape group)
        {
            if (Project != null && group != null)
            {
                var library = Project.RemoveGroup(group);
                library?.SetSelected(library?.Items.FirstOrDefault());
            }
        }

        public void OnInsertGroup(GroupShape group)
        {
            if (Project?.CurrentContainer != null)
            {
                OnDropShapeAsClone(group, 0.0, 0.0);
            }
        }

        public void OnAddLayer(PageContainer container)
        {
            if (container != null)
            {
                Project.AddLayer(container, Factory.CreateLayerContainer(ProjectEditorConfiguration.DefaultLayerName, container));
            }
        }

        public void OnRemoveLayer(LayerContainer layer)
        {
            if (layer != null)
            {
                Project.RemoveLayer(layer);
                if (layer.Owner is PageContainer owner)
                {
                    owner.SetCurrentLayer(owner.Layers.FirstOrDefault());
                }
            }
        }

        public void OnAddStyleLibrary()
        {
            var sl = Factory.CreateLibrary<ShapeStyle>(ProjectEditorConfiguration.DefaulStyleLibraryName);
            Project.AddStyleLibrary(sl);
            Project.SetCurrentStyleLibrary(sl);
        }

        public void OnRemoveStyleLibrary(Library<ShapeStyle> library)
        {
            Project.RemoveStyleLibrary(library);
            Project.SetCurrentStyleLibrary(Project?.StyleLibraries.FirstOrDefault());
        }

        public void OnAddStyle(Library<ShapeStyle> library)
        {
            if (PageState?.SelectedShapes != null)
            {
                foreach (var shape in PageState.SelectedShapes)
                {
                    if (shape.Style != null)
                    {
                        var style = (ShapeStyle)shape.Style.Copy(null);
                        Project.AddStyle(library, style);
                    }
                }
            }
            else
            {
                var style = Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                Project.AddStyle(library, style);
            }
        }

        public void OnRemoveStyle(ShapeStyle style)
        {
            var library = Project.RemoveStyle(style);
            library?.SetSelected(library?.Items.FirstOrDefault());
        }

        public void OnApplyStyle(ShapeStyle style)
        {
            if (style != null)
            {
                if (PageState?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in PageState.SelectedShapes)
                    {
                        Project?.ApplyStyle(shape, style);
                    }
                }
            }
        }

        public void OnAddShape(BaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.AddShape(layer, shape);
            }
        }

        public void OnRemoveShape(BaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.RemoveShape(layer, shape);
                Project.CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
            }
        }

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

        public void OnRemoveTemplate(PageContainer template)
        {
            if (template != null)
            {
                Project?.RemoveTemplate(template);
                Project?.SetCurrentTemplate(Project?.Templates.FirstOrDefault());
            }
        }

        public void OnEditTemplate(PageContainer template)
        {
            if (Project != null && template != null)
            {
                Project.SetCurrentContainer(template);
                Project.CurrentContainer?.InvalidateLayer();
            }
        }

        public void OnAddScript()
        {
            if (Project != null)
            {
                var script = Factory.CreateScript(ProjectEditorConfiguration.DefaultScriptName);
                Project.AddScript(script);
            }
        }

        public void OnRemoveScript(Script script)
        {
            if (script != null)
            {
                Project?.RemoveScript(script);
                Project?.SetCurrentScript(Project?.Scripts.FirstOrDefault());
            }
        }

        public void OnApplyTemplate(PageContainer template)
        {
            var page = Project?.CurrentContainer;
            if (page != null && template != null && page != template)
            {
                Project.ApplyTemplate(page, template);
                Project.CurrentContainer.InvalidateLayer();
            }
        }

        public string OnGetImageKey(string path)
        {
            using var stream = FileIO.Open(path);
            var bytes = FileIO.ReadBinary(stream);
            if (Project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                return key;
            }
            return default;
        }

        public async Task<string> OnAddImageKey(string path)
        {
            if (Project != null)
            {
                if (path == null || string.IsNullOrEmpty(path))
                {
                    var key = await (ImageImporter.GetImageKeyAsync() ?? Task.FromResult(string.Empty));
                    if (key == null || string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

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

        public void OnSelectedItemChanged(ObservableObject item)
        {
            if (Project != null)
            {
                Project.Selected = item;
            }
        }

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

        public void OnInsertPageBefore(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is PageContainer selected)
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

        public void OnInsertPageAfter(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is PageContainer selected)
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

        public void OnInsertDocumentBefore(object item)
        {
            if (Project != null)
            {
                if (item is DocumentContainer selected)
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

        public void OnInsertDocumentAfter(object item)
        {
            if (Project != null)
            {
                if (item is DocumentContainer selected)
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

        private void SetRenderersImageCache(IImageCache cache)
        {
            if (PageRenderer != null)
            {
                PageRenderer.ClearCache();
                PageRenderer.State.ImageCache = cache;
            }

            if (DocumentRenderer != null)
            {
                DocumentRenderer.ClearCache();
                DocumentRenderer.State.ImageCache = cache;
            }
        }

        public void OnLoad(ProjectContainer project, string path = null)
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
                    imageCache.PurgeUnusedImages(new HashSet<string>());
                }
                Deselect();
                SetRenderersImageCache(null);
                Project = null;
                ProjectPath = string.Empty;
                IsProjectDirty = false;
                GC.Collect();
            }
        }

        public void OnInvalidateCache()
        {
            try
            {
                if (PageRenderer != null)
                {
                    PageRenderer.ClearCache();
                }

                if (DocumentRenderer != null)
                {
                    DocumentRenderer.ClearCache();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

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

        public bool CanUndo()
        {
            return Project?.History?.CanUndo() ?? false;
        }

        public bool CanRedo()
        {
            return Project?.History?.CanRedo() ?? false;
        }

        public bool CanCopy()
        {
            return PageState?.SelectedShapes != null;
        }

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

        public void OnCopyShapes(IList<BaseShape> shapes)
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

        public void OnTryPaste(string text)
        {
            try
            {
                var exception = default(Exception);

                try
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        var pathShape = PathConverter?.FromSvgPathData(text, isStroked: false, isFilled: true);
                        if (pathShape != null)
                        {
                            OnPasteShapes(Enumerable.Repeat<BaseShape>(pathShape, 1));
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                try
                {
                    var shapes = JsonSerializer?.Deserialize<IList<BaseShape>>(text);
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

        private IDictionary<string, Record> GenerateRecordDictionaryById()
        {
            return Project?.Databases
                .Where(d => d?.Records != null && d?.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
        }

        private void TryToRestoreRecords(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (Project?.Databases == null)
                {
                    return;
                }

                var records = GenerateRecordDictionaryById();

                // Try to restore shape record.
                foreach (var shape in ProjectContainer.GetAllShapes(shapes))
                {
                    if (shape?.Record == null)
                    {
                        continue;
                    }

                    if (records.TryGetValue(shape.Record.Id, out var record))
                    {
                        // Use existing record.
                        shape.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (Project?.CurrentDatabase == null && shape.Record.Owner is Database owner)
                        {
                            var db = Factory.CreateDatabase(
                                ProjectEditorConfiguration.ImportedDatabaseName,
                                owner.Columns);
                            Project.AddDatabase(db);
                            Project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Record.Owner = Project.CurrentDatabase;
                        Project?.AddRecord(Project?.CurrentDatabase, shape.Record);

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

        private void RestoreShape(BaseShape shape)
        {
            var shapes = Enumerable.Repeat(shape, 1).ToList();
            TryToRestoreRecords(shapes);
        }

        public void OnPasteShapes(IEnumerable<BaseShape> shapes)
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                TryToRestoreRecords(shapes);
                Project.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);
                OnSelect(shapes);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnSelect(IEnumerable<BaseShape> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(Project?.CurrentContainer?.CurrentLayer, new HashSet<BaseShape>(shapes));
            }
        }

        public T CloneShape<T>(T shape) where T : BaseShape
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

        public LayerContainer Clone(LayerContainer container)
        {
            try
            {
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<LayerContainer>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Shapes;
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

        public PageContainer Clone(PageContainer container)
        {
            try
            {
                var template = container?.Template;
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<PageContainer>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
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

        public DocumentContainer Clone(DocumentContainer document)
        {
            try
            {
                var templates = document?.Pages.Select(c => c?.Template)?.ToArray();
                var json = JsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<DocumentContainer>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Pages.Length; i++)
                        {
                            var container = clone.Pages[i];
                            var shapes = container.Layers.SelectMany(l => l.Shapes);
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

        public async Task<bool> OnDropFiles(string[] files, double x, double y)
        {
            try
            {
                if (files?.Length < 1)
                {
                    return false;
                }

                bool result = false;

                foreach (var path in files)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }

                    string ext = System.IO.Path.GetExtension(path);

                    if (string.Compare(ext, ProjectEditorConfiguration.ProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnOpenProject(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.CsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var reader = TextFieldReaders.FirstOrDefault(x => x.Extension == "csv");
                        if (reader != null)
                        {
                            OnImportData(Project, path, reader);
                            result = true;
                        }
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.XlsxExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var reader = TextFieldReaders.FirstOrDefault(x => x.Extension == "xlsx");
                        if (reader != null)
                        {
                            OnImportData(Project, path, reader);
                            result = true;
                        }
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.JsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnImportJson(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.ScriptExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        await OnExecuteScriptFile(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.SvgExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnImportSvg(path);
                        result = true;
                    }
                    else if (ProjectEditorConfiguration.ImageExtensions.Any(x => string.Compare(ext, x, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        var key = OnGetImageKey(path);
                        if (key != null && !string.IsNullOrEmpty(key))
                        {
                            OnDropImageKey(key, x, y);
                        }
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return false;
        }

        public void OnDropImageKey(string key, double x, double y)
        {
            var selected = Project.CurrentStyleLibrary?.Selected != null ?
                Project.CurrentStyleLibrary.Selected :
                Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var style = (ShapeStyle)selected.Copy(null);
            var layer = Project?.CurrentContainer?.CurrentLayer;
            decimal sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            decimal sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

            var image = Factory.CreateImageShape((double)sx, (double)sy, style, key);
            image.BottomRight.X = (double)(sx + 320m);
            image.BottomRight.Y = (double)(sy + 180m);

            Project.AddShape(layer, image);
        }

        public bool OnDropShape(BaseShape shape, double x, double y, bool bExecute = true)
        {
            try
            {
                var layer = Project?.CurrentContainer?.CurrentLayer;
                if (layer != null)
                {
                    if (bExecute == true)
                    {
                        OnDropShapeAsClone(shape, x, y);
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

        public void OnDropShapeAsClone<T>(T shape, double x, double y) where T : BaseShape
        {
            decimal sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            decimal sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

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
                        if (clone is GroupShape group)
                        {
                            var shapes = ProjectContainer.GetAllShapes<LineShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes);
                            TryToConnectLines(shapes, group.Connectors);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public bool OnDropRecord(Record record, double x, double y, bool bExecute = true)
        {
            try
            {
                if (PageState?.SelectedShapes?.Count > 0)
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
                        var shapes = layer.Shapes.Reverse();
                        double radius = Project.Options.HitThreshold / PageState.ZoomX;
                        var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                        if (result != null)
                        {
                            if (bExecute)
                            {
                                Project?.ApplyRecord(result, record);
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

        public void OnDropRecordAsGroup(Record record, double x, double y)
        {
            var selected = Project.CurrentStyleLibrary?.Selected != null ?
                Project.CurrentStyleLibrary.Selected :
                Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var style = (ShapeStyle)selected.Copy(null);
            var layer = Project?.CurrentContainer?.CurrentLayer;
            decimal sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            decimal sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

            var g = Factory.CreateGroupShape(ProjectEditorConfiguration.DefaulGroupName);

            g.Record = record;

            var length = record.Values.Length;
            double px = (double)sx;
            double py = (double)sy;
            double width = 150;
            double height = 15;

            var db = record.Owner as Database;

            for (int i = 0; i < length; i++)
            {
                var column = db.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + db.Columns[i].Name + "}";
                    var text = Factory.CreateTextShape(px, py, px + width, py + height, style, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = Factory.CreateRectangleShape((double)sx, (double)sy, (double)sx + width, (double)sy + (length * height), style);
            g.AddShape(rectangle);

            var pt = Factory.CreatePointShape((double)sx + (width / 2), (double)sy);
            var pb = Factory.CreatePointShape((double)sx + (width / 2), (double)sy + (length * height));
            var pl = Factory.CreatePointShape((double)sx, (double)sy + ((length * height) / 2));
            var pr = Factory.CreatePointShape((double)sx + width, (double)sy + ((length * height) / 2));

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            Project.AddShape(layer, g);
        }

        public bool OnDropStyle(ShapeStyle style, double x, double y, bool bExecute = true)
        {
            try
            {
                if (PageState?.SelectedShapes?.Count > 0)
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
                        var shapes = layer.Shapes.Reverse();
                        double radius = Project.Options.HitThreshold / PageState.ZoomX;
                        var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
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

        public bool OnDropTemplate(PageContainer template, double x, double y, bool bExecute = true)
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

        public void OnDeleteSelected()
        {
            if (Project?.CurrentContainer?.CurrentLayer == null || PageState == null)
            {
                return;
            }

            if (PageState.SelectedShapes?.Count > 0)
            {
                var layer = Project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in PageState.SelectedShapes)
                {
                    builder.Remove(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                PageState.SelectedShapes = default;
                layer.InvalidateLayer();

                OnHideDecorator();
            }
        }

        public void Deselect()
        {
            if (PageState?.SelectedShapes != null)
            {
                PageState.SelectedShapes = default;
            }

            OnHideDecorator();
        }

        public void Select(LayerContainer layer, BaseShape shape)
        {
            if (PageState != null)
            {
                PageState.SelectedShapes = new HashSet<BaseShape>() { shape };

                if (PageState.DrawPoints == true)
                {
                    OnHideDecorator();
                }
                else
                {
                    if (shape is PointShape || shape is LineShape)
                    {
                        OnHideDecorator();
                    }
                    else
                    {
                        OnShowDecorator();
                    }
                }
            }

            if (layer.Owner is PageContainer owner)
            {
                owner.CurrentShape = shape;
            }

            if (layer != null)
            {
                layer.InvalidateLayer();
            }
            else
            {
                CanvasPlatform?.InvalidateControl?.Invoke();
            }
        }

        public void Select(LayerContainer layer, ISet<BaseShape> shapes)
        {
            if (PageState != null)
            {
                PageState.SelectedShapes = shapes;

                OnShowDecorator();
            }

            if (layer.Owner is PageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.InvalidateLayer();
            }
            else
            {
                CanvasPlatform?.InvalidateControl?.Invoke();
            }
        }

        public void Deselect(LayerContainer layer)
        {
            Deselect();

            if (layer.Owner is PageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.InvalidateLayer();
            }
            else
            {
                if (CanvasPlatform?.InvalidateControl != null)
                {
                    CanvasPlatform?.InvalidateControl();
                }
            }
        }

        public bool TryToSelectShape(LayerContainer layer, double x, double y, bool deselect = true)
        {
            if (layer != null)
            {
                var shapes = layer.Shapes.Reverse();
                double radius = Project.Options.HitThreshold / PageState.ZoomX;

                var point = HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
                if (point != null)
                {
                    Select(layer, point);
                    return true;
                }

                var shape = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                if (shape != null)
                {
                    Select(layer, shape);
                    return true;
                }

                if (deselect)
                {
                    Deselect(layer);
                }
            }

            return false;
        }

        public bool TryToSelectShapes(LayerContainer layer, RectangleShape rectangle, bool deselect = true, bool includeSelected = false)
        {
            if (layer != null)
            {
                var rect = Rect2.FromPoints(
                    rectangle.TopLeft.X,
                    rectangle.TopLeft.Y,
                    rectangle.BottomRight.X,
                    rectangle.BottomRight.Y);
                var shapes = layer.Shapes;
                double radius = Project.Options.HitThreshold / PageState.ZoomX;
                var result = HitTest.TryToGetShapes(shapes, rect, radius, PageState.ZoomX);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        if (includeSelected)
                        {
                            if (PageState != null)
                            {
                                if (PageState.SelectedShapes != null)
                                {
                                    foreach (var shape in PageState.SelectedShapes)
                                    {
                                        if (result.Contains(shape))
                                        {
                                            result.Remove(shape);
                                        }
                                        else
                                        {
                                            result.Add(shape);
                                        }
                                    }
                                }
                            }

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
                        else
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
                }

                if (deselect)
                {
                    Deselect(layer);
                }
            }

            return false;
        }

        public void Hover(LayerContainer layer, BaseShape shape)
        {
            if (layer != null)
            {
                Select(layer, shape);
                HoveredShape = shape;
            }
        }

        public void Dehover(LayerContainer layer)
        {
            if (layer != null && HoveredShape != null)
            {
                HoveredShape = default;
                Deselect(layer);
            }
        }

        public bool TryToHoverShape(double x, double y)
        {
            if (Project?.CurrentContainer?.CurrentLayer == null)
            {
                return false;
            }

            if (PageState.SelectedShapes?.Count > 1)
            {
                return false;
            }

            if (!(PageState.SelectedShapes?.Count == 1 && HoveredShape != PageState.SelectedShapes?.FirstOrDefault()))
            {
                var shapes = Project.CurrentContainer?.CurrentLayer?.Shapes.Reverse();

                double radius = Project.Options.HitThreshold / PageState.ZoomX;
                var point = HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
                if (point != null)
                {
                    Hover(Project.CurrentContainer?.CurrentLayer, point);
                    return true;
                }
                else
                {
                    var shape = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                    if (shape != null)
                    {
                        Hover(Project.CurrentContainer?.CurrentLayer, shape);
                        return true;
                    }
                    else
                    {
                        if (PageState.SelectedShapes?.Count == 1 && HoveredShape == PageState.SelectedShapes?.FirstOrDefault())
                        {
                            Dehover(Project.CurrentContainer?.CurrentLayer);
                        }
                    }
                }
            }

            return false;
        }

        public PointShape TryToGetConnectionPoint(double x, double y)
        {
            if (Project.Options.TryToConnect)
            {
                var shapes = Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
                double radius = Project.Options.HitThreshold / PageState.ZoomX;
                return HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, PageState.ZoomX);
            }
            return null;
        }

        private void SwapLineStart(LineShape line, PointShape point)
        {
            if (line?.Start != null && point != null)
            {
                var previous = line.Start;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.Start = p);
                line.Start = next;
            }
        }

        private void SwapLineEnd(LineShape line, PointShape point)
        {
            if (line?.End != null && point != null)
            {
                var previous = line.End;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }
        }

        public bool TryToSplitLine(double x, double y, PointShape point, bool select = false)
        {
            if (Project?.CurrentContainer == null || Project?.Options == null)
            {
                return false;
            }

            var shapes = Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
            double radius = Project.Options.HitThreshold / PageState.ZoomX;
            var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);

            if (result is LineShape line)
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
                    (ShapeStyle)line.Style.Copy(null),
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

        public bool TryToSplitLine(LineShape line, PointShape p0, PointShape p1)
        {
            if (Project?.Options == null)
            {
                return false;
            }

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
            {
                return false;
            }

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
            {
                return false;
            }

            LineShape split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = Factory.CreateLineShape(
                    p0,
                    line.End,
                    (ShapeStyle)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = Factory.CreateLineShape(
                    p1,
                    line.End,
                    (ShapeStyle)line.Style.Copy(null),
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        public bool TryToConnectLines(IEnumerable<LineShape> lines, ImmutableArray<PointShape> connectors)
        {
            if (connectors.Length > 0)
            {
                var lineToPoints = new Dictionary<LineShape, IList<PointShape>>();

                double threshold = Project.Options.HitThreshold / PageState.ZoomX;
                double scale = PageState.ZoomX;

                // Find possible connector to line connections.
                foreach (var connector in connectors)
                {
                    LineShape result = null;
                    foreach (var line in lines)
                    {
                        double radius = Project.Options.HitThreshold / PageState.ZoomX;
                        if (HitTest.Contains(line, new Point2(connector.X, connector.Y), threshold, scale))
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
                            lineToPoints.Add(result, new List<PointShape>());
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

        private GroupShape Group(LayerContainer layer, ISet<BaseShape> shapes, string name)
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

        private void Ungroup(LayerContainer layer, ISet<BaseShape> shapes)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is GroupShape group)
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

        public GroupShape Group(ISet<BaseShape> shapes, string name)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                return Group(layer, shapes, name);
            }

            return null;
        }

        public bool Ungroup(ISet<BaseShape> shapes)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shapes != null)
            {
                Ungroup(layer, shapes);
                return true;
            }

            return false;
        }

        private void Swap(BaseShape shape, int sourceIndex, int targetIndex)
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

        public void BringToFront(BaseShape source)
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

        public void BringForward(BaseShape source)
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

        public void SendBackward(BaseShape source)
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

        public void SendToBack(BaseShape source)
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

        public void MoveShapesBy(IEnumerable<BaseShape> shapes, decimal dx, decimal dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(null, dx, dy);
                }
            }
            OnUpdateDecorator();
        }

        private void MoveShapesByWithHistory(IEnumerable<BaseShape> shapes, decimal dx, decimal dy)
        {
            MoveShapesBy(shapes, dx, dy);
            OnUpdateDecorator();

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            Project?.History?.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
        }

        public void MoveBy(ISet<BaseShape> shapes, decimal dx, decimal dy)
        {
            if (shapes != null)
            {
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var points = new List<PointShape>();

                            foreach (var shape in shapes)
                            {
                                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                                {
                                    shape.GetPoints(points);
                                }
                            }

                            var distinct = points.Distinct().ToList();
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

        public void MoveItem<T>(Library<T> library, int sourceIndex, int targetIndex)
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

        public void SwapItem<T>(Library<T> library, int sourceIndex, int targetIndex)
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

        public void InsertItem<T>(Library<T> library, T item, int index)
        {
            var builder = library.Items.ToBuilder();
            builder.Insert(index, item);

            var previous = library.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
            library.Items = next;
        }
    }
}
