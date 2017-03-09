// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data.Database;
using Core2D.Editor.Bounds;
using Core2D.Editor.Bounds.Shapes;
using Core2D.Editor.Factories;
using Core2D.Editor.Recent;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor model.
    /// </summary>
    public partial class ProjectEditor : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private XProject _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private Action _invalidate;
        private Action _resetZoom;
        private Action _extentZoom;
        private Action _loadLayout;
        private Action _saveLayout;
        private Action _resetLayout;
        private bool _cancelAvailable;
        private ToolBase _currentTool;
        private PathToolBase _currentPathTool;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private IView _currentView;
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
        private readonly Lazy<ITextFieldReader<XDatabase>> _csvReader;
        private readonly Lazy<ITextFieldWriter<XDatabase>> _csvWriter;
        private readonly Lazy<IImageImporter> _imageImporter;
        private readonly Lazy<ProjectEditorCommands> _editorCommands;

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
        /// Gets or sets current editor tool.
        /// </summary>
        public ToolBase CurrentTool
        {
            get { return _currentTool; }
            set { Update(ref _currentTool, value); }
        }

        /// <summary>
        /// Gets or sets current editor path tool.
        /// </summary>
        public PathToolBase CurrentPathTool
        {
            get { return _currentPathTool; }
            set { Update(ref _currentPathTool, value); }
        }

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
        /// Gets or sets current view.
        /// </summary>
        public IView CurrentView
        {
            get { return _currentView; }
            set { Update(ref _currentView, value); }
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
        public ITextFieldReader<XDatabase> CsvReader => _csvReader.Value;

        /// <summary>
        /// Gets Csv file writer.
        /// </summary>
        public ITextFieldWriter<XDatabase> CsvWriter => _csvWriter.Value;

        /// <summary>
        /// Gets image key importer.
        /// </summary>
        public IImageImporter ImageImporter => _imageImporter.Value;

        /// <summary>
        /// Gets project editor commands.
        /// </summary>
        public ProjectEditorCommands EditorCommands => _editorCommands.Value;

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
            _csvReader = _serviceProvider.GetServiceLazily<ITextFieldReader<XDatabase>>();
            _csvWriter = _serviceProvider.GetServiceLazily<ITextFieldWriter<XDatabase>>();
            _imageImporter = _serviceProvider.GetServiceLazily<IImageImporter>();
            _editorCommands = _serviceProvider.GetServiceLazily<ProjectEditorCommands>();
        }
    }
}
