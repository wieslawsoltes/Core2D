#nullable disable
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.Recent;
using Core2D.ViewModels.Editors;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Docking;
using Dock.Model.Controls;
namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel : ViewModelBase, IDialogPresenter
    {
        public ProjectEditorViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _shapeEditor = new ShapeEditor(serviceProvider);
            _recentProjects = ImmutableArray.Create<RecentFileViewModel>();
            _currentRecentProject = default;
            _dialogs = new ObservableCollection<DialogViewModel>();
            _tools = serviceProvider.GetServiceLazily<IEditorTool[], ImmutableArray<IEditorTool>>((tools) => tools.ToImmutableArray());
            _pathTools = serviceProvider.GetServiceLazily<IPathTool[], ImmutableArray<IPathTool>>((tools) => tools.ToImmutableArray());
            _hitTest = serviceProvider.GetServiceLazily<IHitTest>(hitTests => hitTests.Register(serviceProvider.GetService<IBounds[]>()));
            _log = serviceProvider.GetServiceLazily<ILog>();
            _dataFlow = serviceProvider.GetServiceLazily<DataFlow>();
            _renderer = serviceProvider.GetServiceLazily<IShapeRenderer>();
            _fileSystem = serviceProvider.GetServiceLazily<IFileSystem>();
            _factory = serviceProvider.GetServiceLazily<IViewModelFactory>();
            _containerFactory = serviceProvider.GetServiceLazily<IContainerFactory>();
            _shapeFactory = serviceProvider.GetServiceLazily<IShapeFactory>();
            _textClipboard = serviceProvider.GetServiceLazily<ITextClipboard>();
            _jsonSerializer = serviceProvider.GetServiceLazily<IJsonSerializer>();
            _fileWriters = serviceProvider.GetServiceLazily<IFileWriter[], ImmutableArray<IFileWriter>>((writers) => writers.ToImmutableArray());
            _textFieldReaders = serviceProvider.GetServiceLazily<ITextFieldReader<DatabaseViewModel>[], ImmutableArray<ITextFieldReader<DatabaseViewModel>>>((readers) => readers.ToImmutableArray());
            _textFieldWriters = serviceProvider.GetServiceLazily<ITextFieldWriter<DatabaseViewModel>[], ImmutableArray<ITextFieldWriter<DatabaseViewModel>>>((writers) => writers.ToImmutableArray());
            _imageImporter = serviceProvider.GetServiceLazily<IImageImporter>();
            _scriptRunner = serviceProvider.GetServiceLazily<IScriptRunner>();
            _platform = serviceProvider.GetServiceLazily<IProjectEditorPlatform>();
            _canvasPlatform = serviceProvider.GetServiceLazily<IEditorCanvasPlatform>();
            _styleEditor = serviceProvider.GetServiceLazily<StyleEditorViewModel>();
            _pathConverter = serviceProvider.GetServiceLazily<IPathConverter>();
            _svgConverter = serviceProvider.GetServiceLazily<ISvgConverter>();

            _dockFactory = new DockFactory(this);

            _dockFactory.DockableClosed += (sender, args) =>
            {
                Debug.WriteLine($"DockableClosed {args.Dockable?.Id}");
            };

            _dockFactory.DockableRemoved += (sender, args) =>
            {
                Debug.WriteLine($"DockableRemoved {args.Dockable?.Id}");
            };
        }

        public void CreateLayout()
        {
            _rootDock = _dockFactory.CreateLayout();

            if (_rootDock is { })
            {
                _dockFactory?.InitLayout(_rootDock);
            }

            _dockFactory.GetDockable<IDocumentDock>("Pages")?.CreateDocument?.Execute(null);

            NavigateTo("Dashboard");
        }

        public void LoadLayout(IRootDock layout)
        {
            _rootDock = layout;

            if (_rootDock is { })
            {
                _dockFactory?.InitLayout(_rootDock);
            }

            NavigateTo("Dashboard");
        }

        public void NavigateTo(string id)
        {
            _rootDock?.Navigate.Execute(id);
        }

        public void OnToggleDockableVisibility(string id)
        {
            // TODO:
        }

        public void ShowDialog(DialogViewModel dialog)
        {
            _dialogs.Add(dialog);
        }

        public void CloseDialog(DialogViewModel dialog)
        {
            _dialogs.Remove(dialog);
        }

        public DialogViewModel CreateTextBindingDialog(TextShapeViewModel text)
        {
            var textBindingEditor = new TextBindingEditorViewModel(_serviceProvider)
            {
                Editor = this,
                Text = text
            };

            return new DialogViewModel(_serviceProvider, this)
            {
                Title = "Text Binding",
                IsOverlayVisible = false,
                IsTitleBarVisible = true,
                IsCloseButtonVisible = true,
                ViewModel = textBindingEditor
            };
        }
    }
}
