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
        private readonly Lazy<DataFlow?>? _dataFlow;
        private readonly Lazy<IShapeRenderer?>? _renderer;
        private readonly Lazy<ISelectionService?>? _selectionService;
        private readonly Lazy<IShapeService?>? _shapeService;
        private readonly Lazy<IClipboardService?>? _clipboardService;
        private readonly Lazy<ImmutableArray<IFileWriter>> _fileWriters;
        private readonly Lazy<ImmutableArray<ITextFieldReader<DatabaseViewModel>>> _textFieldReaders;
        private readonly Lazy<ImmutableArray<ITextFieldWriter<DatabaseViewModel>>> _textFieldWriters;
        private readonly Lazy<IProjectEditorPlatform?>? _platform;
        private readonly Lazy<IEditorCanvasPlatform?>? _canvasPlatform;
        private readonly Lazy<StyleEditorViewModel?>? _styleEditor;

        public ImmutableArray<IEditorTool> Tools => _tools.Value;

        public ImmutableArray<IPathTool> PathTools => _pathTools.Value;

        public DataFlow? DataFlow => _dataFlow?.Value;

        public IShapeRenderer? Renderer => _renderer?.Value;

        public ShapeRendererStateViewModel? PageState => _renderer?.Value?.State;

        public ISelectionService? SelectionService => _selectionService?.Value;

        public IShapeService? ShapeService => _shapeService?.Value;
        
        public IClipboardService? ClipboardService => _clipboardService?.Value;

        public ImmutableArray<IFileWriter> FileWriters => _fileWriters.Value;

        public ImmutableArray<ITextFieldReader<DatabaseViewModel>> TextFieldReaders => _textFieldReaders.Value;

        public ImmutableArray<ITextFieldWriter<DatabaseViewModel>> TextFieldWriters => _textFieldWriters.Value;

        public IProjectEditorPlatform? Platform => _platform?.Value;

        public IEditorCanvasPlatform? CanvasPlatform => _canvasPlatform?.Value;

        public StyleEditorViewModel? StyleEditor => _styleEditor?.Value;
    }
}
