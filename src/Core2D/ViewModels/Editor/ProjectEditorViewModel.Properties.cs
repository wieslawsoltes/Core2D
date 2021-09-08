#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.Recent;
using Core2D.ViewModels.Renderer;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        [AutoNotify] private IRootDock? _rootDock;
        [AutoNotify] private IFactory? _dockFactory;
        [AutoNotify] private ProjectContainerViewModel? _project;
        [AutoNotify] private string? _projectPath;
        [AutoNotify] private bool _isProjectDirty;
        [AutoNotify] private IDisposable? _observer;
        [AutoNotify] private bool _isToolIdle;
        [AutoNotify] private IEditorTool? _currentTool;
        [AutoNotify] private IPathTool? _currentPathTool;
        [AutoNotify] private ImmutableArray<RecentFileViewModel> _recentProjects;
        [AutoNotify] private RecentFileViewModel? _currentRecentProject;
        [AutoNotify] private AboutInfoViewModel? _aboutInfo;
        [AutoNotify] private IList<DialogViewModel>? _dialogs;
        private readonly Lazy<ImmutableArray<IEditorTool>> _tools;
        private readonly Lazy<ImmutableArray<IPathTool>> _pathTools;
        private readonly Lazy<IHitTest>? _hitTest;
        private readonly Lazy<ILog>? _log;
        private readonly Lazy<DataFlow>? _dataFlow;
        private readonly Lazy<IShapeRenderer>? _renderer;
        private readonly Lazy<IFileSystem>? _fileSystem;
        private readonly Lazy<IViewModelFactory>? _factory;
        private readonly Lazy<IContainerFactory>? _containerFactory;
        private readonly Lazy<IShapeFactory>? _shapeFactory;
        private readonly Lazy<ITextClipboard>? _textClipboard;
        private readonly Lazy<IJsonSerializer>? _jsonSerializer;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ImmutableArray<ITextFieldReader<DatabaseViewModel>>> _textFieldReaders;
        private readonly Lazy<ImmutableArray<ITextFieldWriter<DatabaseViewModel>>> _textFieldWriters;
        private readonly Lazy<IImageImporter>? _imageImporter;
        private readonly Lazy<IScriptRunner>? _scriptRunner;
        private readonly Lazy<IProjectEditorPlatform>? _platform;
        private readonly Lazy<IEditorCanvasPlatform>? _canvasPlatform;
        private readonly Lazy<StyleEditorViewModel>? _styleEditor;
        private readonly Lazy<IPathConverter>? _pathConverter;
        private readonly Lazy<ISvgConverter>? _svgConverter;

        public ImmutableArray<IEditorTool> Tools => _tools.Value;

        public ImmutableArray<IPathTool> PathTools => _pathTools.Value;

        public IHitTest? HitTest => _hitTest?.Value;

        public ILog? Log => _log?.Value;

        public DataFlow? DataFlow => _dataFlow?.Value;

        public IShapeRenderer? Renderer => _renderer?.Value;

        public ShapeRendererStateViewModel? PageState => _renderer?.Value.State;

        public ISelection? Selection => _project;

        public IFileSystem? FileSystem => _fileSystem?.Value;

        public IViewModelFactory? ViewModelFactory => _factory?.Value;

        public IContainerFactory? ContainerFactory => _containerFactory?.Value;

        public IShapeFactory? ShapeFactory => _shapeFactory?.Value;

        public ITextClipboard? TextClipboard => _textClipboard?.Value;

        public IJsonSerializer? JsonSerializer => _jsonSerializer?.Value;

        public ImmutableArray<IFileWriter> FileWriters => _fileWriters.Value;

        public ImmutableArray<ITextFieldReader<DatabaseViewModel>> TextFieldReaders => _textFieldReaders.Value;

        public ImmutableArray<ITextFieldWriter<DatabaseViewModel>> TextFieldWriters => _textFieldWriters.Value;

        public IImageImporter? ImageImporter => _imageImporter?.Value;

        public IScriptRunner? ScriptRunner => _scriptRunner?.Value;

        public IProjectEditorPlatform? Platform => _platform?.Value;

        public IEditorCanvasPlatform? CanvasPlatform => _canvasPlatform?.Value;

        public StyleEditorViewModel? StyleEditor => _styleEditor?.Value;

        public IPathConverter? PathConverter => _pathConverter?.Value;

        public ISvgConverter? SvgConverter => _svgConverter?.Value;

        private object? ScriptState { get; set; }
    }
}
