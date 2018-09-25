// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.Recent;
using Core2D.Interfaces;
using Core2D.Renderer;
using Dock.Model;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor model.
    /// </summary>
    public partial class ProjectEditor : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private IProjectContainer _project;
        private string _projectPath;
        private bool _isProjectDirty;
        private ProjectObserver _observer;
        private bool _isToolIdle;
        private ToolBase _currentTool;
        private PathToolBase _currentPathTool;
        private ImmutableArray<RecentFile> _recentProjects;
        private RecentFile _currentRecentProject;
        private IDock _layout;
        private AboutInfo _aboutInfo;
        private readonly Lazy<ImmutableArray<ToolBase>> _tools;
        private readonly Lazy<ImmutableArray<PathToolBase>> _pathTools;
        private readonly Lazy<HitTest> _hitTest;
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

        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        public IProjectContainer Project
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
        /// Gets or sets current layout configuration.
        /// </summary>
        public IDock Layout
        {
            get => _layout;
            set => Update(ref _layout, value);
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
        /// Gets current log.
        /// </summary>
        public ILog Log => _log.Value;

        /// <summary>
        /// Gets current data flow.
        /// </summary>
        public IDataFlow DataFlow => _dataFlow.Value;

        /// <summary>
        /// Gets current renderer's.
        /// </summary>
        public IShapeRenderer[] Renderers => _renderers.Value;

        /// <summary>
        /// Gets current file system.
        /// </summary>
        public IFileSystem FileIO => _fileIO.Value;

        /// <summary>
        /// Gets factory.
        /// </summary>
        public IFactory Factory => _factory.Value;

        /// <summary>
        /// Gets container factory.
        /// </summary>
        public IContainerFactory ContainerFactory => _containerFactory.Value;

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
        public ITextFieldReader<IDatabase> CsvReader => _csvReader.Value;

        /// <summary>
        /// Gets Csv file writer.
        /// </summary>
        public ITextFieldWriter<IDatabase> CsvWriter => _csvWriter.Value;

        /// <summary>
        /// Gets image key importer.
        /// </summary>
        public IImageImporter ImageImporter => _imageImporter.Value;

        /// <summary>
        /// Gets code script runner.
        /// </summary>
        public IScriptRunner ScriptRunner => _scriptRunner.Value;

        /// <summary>
        /// Gets project editor platform.
        /// </summary>
        public IProjectEditorPlatform Platform => _platform.Value;

        /// <summary>
        /// Gets editor canvas platform.
        /// </summary>
        public IEditorCanvasPlatform CanvasPlatform => _canvasPlatform.Value;

        /// <summary>
        /// Gets editor layout platform.
        /// </summary>
        public IEditorLayoutPlatform LayoutPlatform => _layoutPlatform.Value;

        /// <summary>
        /// Initialize new instance of <see cref="ProjectEditor"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ProjectEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _recentProjects = ImmutableArray.Create<RecentFile>();
            _currentRecentProject = default;
            _tools = _serviceProvider.GetServiceLazily<ToolBase[], ImmutableArray<ToolBase>>((tools) => tools.Where(tool => !tool.GetType().Name.StartsWith("PathTool")).ToImmutableArray());
            _pathTools = _serviceProvider.GetServiceLazily<PathToolBase[], ImmutableArray<PathToolBase>>((tools) => tools.ToImmutableArray());
            _hitTest = _serviceProvider.GetServiceLazily<HitTest>(hitTests => hitTests.Register(_serviceProvider.GetService<HitTestBase[]>()));
            _log = _serviceProvider.GetServiceLazily<ILog>();
            _dataFlow = new Lazy<IDataFlow>(() => _serviceProvider.GetService<IDataFlow>());
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
    }
}
