// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

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
        /// Initializes default <see cref="ProjectEditor"/>.
        /// </summary>
        /// <returns>The instance of the <see cref="ProjectEditor"/> class.</returns>
        public ProjectEditor Defaults()
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

            return this;
        }
    }
}
