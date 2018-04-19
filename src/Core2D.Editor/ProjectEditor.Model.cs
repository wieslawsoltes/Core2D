// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Editor.Recent;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Containers;
using Core2D.Renderer;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines project editor platform contract.
    /// </summary>
    public interface IProjectEditorPlatform
    {
        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        void OnOpen(string path);

        /// <summary>
        /// Save project.
        /// </summary>
        void OnSave();

        /// <summary>
        /// Save project as.
        /// </summary>
        void OnSaveAs();

        /// <summary>
        /// Import json.
        /// </summary>
        /// <param name="path">The project json path.</param>
        void OnImportJson(string path);

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="path">The object file path.</param>
        void OnImportObject(string path);

        /// <summary>
        /// Import xaml.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        void OnImportXaml(string path);

        /// <summary>
        /// Export json.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportJson(object item);

        /// <summary>
        /// Export object.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportObject(object item);

        /// <summary>
        /// Export xaml.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportXaml(object item);

        /// <summary>
        /// Export project, document or page.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExport(object item);

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="path">The script file path.</param>
        void OnExecuteScript(string path);

        /// <summary>
        /// Close application view.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Copy page or selected shapes to clipboard as Emf.
        /// </summary>
        /// <param name="item">The object to copy as EMF.</param>
        void OnCopyAsEmf(object item);

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="project">The target project.</param>
        void OnImportData(ProjectContainer project);

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="db">The database to export.</param>
        void OnExportData(Database db);

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="db">The database to update.</param>
        void OnUpdateData(Database db);

        /// <summary>
        /// Show document viewer.
        /// </summary>
        void OnDocumentViewer();

        /// <summary>
        /// Show object browser.
        /// </summary>
        void OnObjectBrowser();

        /// <summary>
        /// Show about dialog.
        /// </summary>
        void OnAboutDialog();

        /// <summary>
        /// Auto-fit view to the available extents.
        /// </summary>
        void OnZoomAutoFit();

        /// <summary>
        /// Reset view size to defaults.
        /// </summary>
        void OnZoomReset();
    }

    /// <summary>
    /// Project editor model.
    /// </summary>
    public partial class ProjectEditor : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private ProjectContainer _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private Action _invalidate;
        private Action _resetZoom;
        private Action _extentZoom;
        private bool _isToolIdle;
        private ToolBase _currentTool;
        private PathToolBase _currentPathTool;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private IView _currentView;
        private AboutInfo _aboutInfo;
        private readonly Lazy<ImmutableArray<ToolBase>> _tools;
        private readonly Lazy<ImmutableArray<PathToolBase>> _pathTools;
        private readonly Lazy<HitTest> _hitTest;
        private readonly Lazy<ImmutableArray<IView>> _views;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<ShapeRenderer[]> _renderers;
        private readonly Lazy<IFileSystem> _fileIO;
        private readonly Lazy<IProjectFactory> _projectFactory;
        private readonly Lazy<IShapeFactory> _shapeFactory;
        private readonly Lazy<ITextClipboard> _textClipboard;
        private readonly Lazy<IJsonSerializer> _jsonSerializer;
        private readonly Lazy<IXamlSerializer> _xamlSerializer;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ITextFieldReader<Database>> _csvReader;
        private readonly Lazy<ITextFieldWriter<Database>> _csvWriter;
        private readonly Lazy<IImageImporter> _imageImporter;
        private readonly Lazy<IProjectEditorPlatform> _platform;

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        public ProjectContainer Project
        {
            get => _project;
            set => Update(ref _project, value);
        }

        /// <summary>
        /// Gets or sets current project path.
        /// </summary>
        public string ProjectPath
        {
            get => _projectPath;
            set => Update(ref _projectPath, value);
        }

        /// <summary>
        /// Gets or sets flag indicating that current project was modified.
        /// </summary>
        public bool IsProjectDirty
        {
            get => _isProjectDirty;
            set => Update(ref _isProjectDirty, value);
        }

        /// <summary>
        /// Gets or sets current project collections and objects observer.
        /// </summary>
        public ProjectObserver Observer
        {
            get => _observer;
            set => Update(ref _observer, value);
        }

        /// <summary>
        /// Gets or sets invalidate action.
        /// </summary>
        /// <remarks>Invalidate current container control.</remarks>
        public Action Invalidate
        {
            get => _invalidate;
            set => Update(ref _invalidate, value);
        }

        /// <summary>
        /// Gets or sets reset zoom action.
        /// </summary>
        /// <remarks>Reset view size to defaults.</remarks>
        public Action ResetZoom
        {
            get => _resetZoom;
            set => Update(ref _resetZoom, value);
        }

        /// <summary>
        /// Gets or sets extent zoom action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        public Action AutoFitZoom
        {
            get => _extentZoom;
            set => Update(ref _extentZoom, value);
        }

        /// <summary>
        /// Gets or sets flag indicating that current tool is in idle mode.
        /// </summary>
        public bool IsToolIdle
        {
            get => _isToolIdle;
            set => Update(ref _isToolIdle, value);
        }

        /// <summary>
        /// Gets or sets current editor tool.
        /// </summary>
        public ToolBase CurrentTool
        {
            get => _currentTool;
            set => Update(ref _currentTool, value);
        }

        /// <summary>
        /// Gets or sets current editor path tool.
        /// </summary>
        public PathToolBase CurrentPathTool
        {
            get => _currentPathTool;
            set => Update(ref _currentPathTool, value);
        }

        /// <summary>
        /// Gets or sets recent projects collection.
        /// </summary>
        public ImmutableArray<RecentFile> RecentProjects
        {
            get => _recentProjects;
            set => Update(ref _recentProjects, value);
        }

        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        public RecentFile CurrentRecentProject
        {
            get => _currentRecentProject;
            set => Update(ref _currentRecentProject, value);
        }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        public IView CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <summary>
        /// Gets or sets about info.
        /// </summary>
        public AboutInfo AboutInfo
        {
            get => _aboutInfo;
            set => Update(ref _aboutInfo, value);
        }

        /// <summary>
        /// Gets or sets editor tools.
        /// </summary>
        public ImmutableArray<ToolBase> Tools => _tools.Value;

        /// <summary>
        /// Gets or sets editor path tools.
        /// </summary>
        public ImmutableArray<PathToolBase> PathTools => _pathTools.Value;

        /// <summary>
        /// Gets or sets current editor hit test.
        /// </summary>
        public HitTest HitTest => _hitTest.Value;

        /// <summary>
        /// Gets or sets registered views.
        /// </summary>
        public ImmutableArray<IView> Views => _views.Value;

        /// <summary>
        /// Gets current log.
        /// </summary>
        public ILog Log => _log.Value;

        /// <summary>
        /// Gets current renderer's.
        /// </summary>
        public ShapeRenderer[] Renderers => _renderers.Value;

        /// <summary>
        /// Gets current file system.
        /// </summary>
        public IFileSystem FileIO => _fileIO.Value;

        /// <summary>
        /// Gets project factory.
        /// </summary>
        public IProjectFactory ProjectFactory => _projectFactory.Value;

        /// <summary>
        /// Gets shape factory.
        /// </summary>
        public IShapeFactory ShapeFactory => _shapeFactory.Value;

        /// <summary>
        /// Gets text clipboard.
        /// </summary>
        public ITextClipboard TextClipboard => _textClipboard.Value;

        /// <summary>
        /// Gets Json serializer.
        /// </summary>
        public IJsonSerializer JsonSerializer => _jsonSerializer.Value;

        /// <summary>
        /// Gets Xaml serializer.
        /// </summary>
        public IXamlSerializer XamlSerializer => _xamlSerializer.Value;

        /// <summary>
        /// Gets available file writers.
        /// </summary>
        public ImmutableArray<IFileWriter> FileWriters => _fileWriters.Value;

        /// <summary>
        /// Gets Csv file reader.
        /// </summary>
        public ITextFieldReader<Database> CsvReader => _csvReader.Value;

        /// <summary>
        /// Gets Csv file writer.
        /// </summary>
        public ITextFieldWriter<Database> CsvWriter => _csvWriter.Value;

        /// <summary>
        /// Gets image key importer.
        /// </summary>
        public IImageImporter ImageImporter => _imageImporter.Value;

        /// <summary>
        /// Gets project editor platform.
        /// </summary>
        public IProjectEditorPlatform Platform => _platform.Value;

        /// <summary>
        /// Initialize new instance of <see cref="ProjectEditor"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ProjectEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default(RecentFile);
            _tools = _serviceProvider.GetServiceLazily<ToolBase[], ImmutableArray<ToolBase>>((tools) => tools.Where(tool => !tool.GetType().Name.StartsWith("PathTool")).ToImmutableArray());
            _pathTools = _serviceProvider.GetServiceLazily<PathToolBase[], ImmutableArray<PathToolBase>>((tools) => tools.ToImmutableArray());
            _hitTest = _serviceProvider.GetServiceLazily<HitTest>(hitTests => hitTests.Register(_serviceProvider.GetService<HitTestBase[]>()));
            _views = _serviceProvider.GetServiceLazily<IView[], ImmutableArray<IView>>((views) => views.ToImmutableArray());
            _log = _serviceProvider.GetServiceLazily<ILog>();
            _renderers = new Lazy<ShapeRenderer[]>(() => new[] { _serviceProvider.GetService<ShapeRenderer>(), _serviceProvider.GetService<ShapeRenderer>() });
            _fileIO = _serviceProvider.GetServiceLazily<IFileSystem>();
            _projectFactory = _serviceProvider.GetServiceLazily<IProjectFactory>();
            _shapeFactory = _serviceProvider.GetServiceLazily<IShapeFactory>();
            _textClipboard = _serviceProvider.GetServiceLazily<ITextClipboard>();
            _jsonSerializer = _serviceProvider.GetServiceLazily<IJsonSerializer>();
            _xamlSerializer = _serviceProvider.GetServiceLazily<IXamlSerializer>();
            _fileWriters = _serviceProvider.GetServiceLazily<IFileWriter[], ImmutableArray<IFileWriter>>((writers) => writers.ToImmutableArray());
            _csvReader = _serviceProvider.GetServiceLazily<ITextFieldReader<Database>>();
            _csvWriter = _serviceProvider.GetServiceLazily<ITextFieldWriter<Database>>();
            _imageImporter = _serviceProvider.GetServiceLazily<IImageImporter>();
            _platform = _serviceProvider.GetServiceLazily<IProjectEditorPlatform>();
        }
    }
}
