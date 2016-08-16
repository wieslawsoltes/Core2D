// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Core2D.Data.Database;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Interfaces;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools;
using Core2D.Editor.Views;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor model.
    /// </summary>
    public partial class ProjectEditor : ObservableObject
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
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private ImmutableArray<ViewBase> _views;
        private ViewBase _currentView;
        private DashboardView _dashboardView;
        private EditorView _editorView;
        private readonly Lazy<IEditorApplication> _application;
        private readonly Lazy<ILog> _log;
        private readonly Lazy<CommandManager> _commandManager;
        private readonly Lazy<ShapeRenderer[]> _renderers;
        private readonly Lazy<IFileSystem> _fileIO;
        private readonly Lazy<IProjectFactory> _projectFactory;
        private readonly Lazy<ITextClipboard> _textClipboard;
        private readonly Lazy<IJsonSerializer> _jsonSerializer;
        private readonly Lazy<IXamlSerializer> _xamlSerializer;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ITextFieldReader<XDatabase>> _csvReader;
        private readonly Lazy<ITextFieldWriter<XDatabase>> _csvWriter;

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
        public Func<Task<string>> GetImageKey => Application.OnGetImageKeyAsync;

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
        /// Gets current editor application.
        /// </summary>
        public IEditorApplication Application => _application.Value;

        /// <summary>
        /// Gets current log.
        /// </summary>
        public ILog Log => _log.Value;

        /// <summary>
        /// Gets current command manager.
        /// </summary>
        public CommandManager CommandManager => _commandManager.Value;

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
        /// Initialize new instance of <see cref="ProjectEditor"/> class.
        /// </summary>
        public ProjectEditor()
        {
            _currentTool = Tool.Selection;
            _currentPathTool = PathTool.Line;

            _tools = new Dictionary<Tool, ToolBase>
            {
                [Tool.None] = new ToolNone(),
                [Tool.Selection] = new ToolSelection(),
                [Tool.Point] = new ToolPoint(),
                [Tool.Line] = new ToolLine(),
                [Tool.Arc] = new ToolArc(),
                [Tool.CubicBezier] = new ToolCubicBezier(),
                [Tool.QuadraticBezier] = new ToolQuadraticBezier(),
                [Tool.Path] = new ToolPath(),
                [Tool.Rectangle] = new ToolRectangle(),
                [Tool.Ellipse] = new ToolEllipse(),
                [Tool.Text] = new ToolText(),
                [Tool.Image] = new ToolImage()
            }
            .ToImmutableDictionary();

            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default(RecentFile);

            _dashboardView = new DashboardView { Name = "Dashboard", Context = this };
            _editorView = new EditorView { Name = "Editor", Context = this };

            _views = new List<ViewBase> { _dashboardView, _editorView }.ToImmutableArray();
            _currentView = _dashboardView;

            _application = ServiceLocator.Instance.ResolveLazily<IEditorApplication>();
            _log = ServiceLocator.Instance.ResolveLazily<ILog>();
            _commandManager = ServiceLocator.Instance.ResolveLazily<CommandManager>();
            _renderers = ServiceLocator.Instance.ResolveLazily<ShapeRenderer[]>();
            _fileIO = ServiceLocator.Instance.ResolveLazily<IFileSystem>();
            _projectFactory = ServiceLocator.Instance.ResolveLazily<IProjectFactory>();
            _textClipboard = ServiceLocator.Instance.ResolveLazily<ITextClipboard>();
            _jsonSerializer = ServiceLocator.Instance.ResolveLazily<IJsonSerializer>();
            _xamlSerializer = ServiceLocator.Instance.ResolveLazily<IXamlSerializer>();
            _fileWriters = ServiceLocator.Instance.ResolveLazily<ImmutableArray<IFileWriter>>();
            _csvReader = ServiceLocator.Instance.ResolveLazily<ITextFieldReader<XDatabase>>();
            _csvWriter = ServiceLocator.Instance.ResolveLazily<ITextFieldWriter<XDatabase>>();

            InitializeCommands();
            CommandManager.RegisterCommands();
        }
    }
}
