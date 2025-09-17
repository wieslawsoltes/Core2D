#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editors;
using Core2D.ViewModels.Shapes;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel : ViewModelBase, IDialogPresenter
{
    public ProjectEditorViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _dialogs = new ObservableCollection<DialogViewModel>();
        _tools = serviceProvider is null 
            ? new Lazy<ImmutableArray<IEditorTool>>(() => new ImmutableArray<IEditorTool>()) 
            : serviceProvider.GetServiceLazily<IEditorTool[], ImmutableArray<IEditorTool>>(tools =>
            {
                if (tools is null)
                {
                    return new ImmutableArray<IEditorTool>();
                }
                return tools.ToImmutableArray();
            });
        _pathTools = serviceProvider is null 
            ? new Lazy<ImmutableArray<IPathTool>>(() => new ImmutableArray<IPathTool>()) 
            : serviceProvider.GetServiceLazily<IPathTool[], ImmutableArray<IPathTool>>(pathTools =>
            {
                if (pathTools is null)
                {
                    return new ImmutableArray<IPathTool>();
                }
                return pathTools.ToImmutableArray();
            });
        _dataFlow = serviceProvider.GetServiceLazily<DataFlow>();
        _renderer = serviceProvider.GetServiceLazily<IShapeRenderer>();
        _libraryRenderer = serviceProvider.GetServiceLazily<IShapeRenderer>();
        _selectionService = serviceProvider.GetServiceLazily<ISelectionService>();
        _shapeService = serviceProvider.GetServiceLazily<IShapeService>();
        _clipboardService = serviceProvider.GetServiceLazily<IClipboardService>();
        _fileWriters = serviceProvider is null 
            ? new Lazy<ImmutableArray<IFileWriter>>(() => new ImmutableArray<IFileWriter>()) 
            : serviceProvider.GetServiceLazily<IFileWriter[], ImmutableArray<IFileWriter>>(writers =>
            {
                if (writers is null)
                {
                    return new ImmutableArray<IFileWriter>();
                }
                return writers.ToImmutableArray();
            });  
        _textFieldReaders = serviceProvider is null 
            ? new Lazy<ImmutableArray<ITextFieldReader<DatabaseViewModel>>>(() => new ImmutableArray<ITextFieldReader<DatabaseViewModel>>()) 
            : serviceProvider.GetServiceLazily<ITextFieldReader<DatabaseViewModel>[], ImmutableArray<ITextFieldReader<DatabaseViewModel>>>(readers =>
            {
                if (readers is null)
                {
                    return new ImmutableArray<ITextFieldReader<DatabaseViewModel>>();
                }
                return readers.ToImmutableArray();
            });
        _textFieldWriters = serviceProvider is null 
            ? new Lazy<ImmutableArray<ITextFieldWriter<DatabaseViewModel>>>(() => new ImmutableArray<ITextFieldWriter<DatabaseViewModel>>()) 
            : serviceProvider.GetServiceLazily<ITextFieldWriter<DatabaseViewModel>[], ImmutableArray<ITextFieldWriter<DatabaseViewModel>>>(writers =>
            {
                if (writers is null)
                {
                    return new ImmutableArray<ITextFieldWriter<DatabaseViewModel>>();
                }
                return writers.ToImmutableArray();
            });
        _platform = serviceProvider.GetServiceLazily<IProjectEditorPlatform>();
        _canvasPlatform = serviceProvider.GetServiceLazily<IEditorCanvasPlatform>();
        _styleEditor = serviceProvider.GetServiceLazily<StyleEditorViewModel>();

        PropertyChanged += OnEditorPropertyChanged;
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public void OnToggleDockableVisibility(object? param)
    {
        if (param is not string id)
        {
            return;
        }

        if (DockFactory is not FactoryBase dockFactory)
        {
            return;
        }

        if (RootDock is not IRootDock rootDock)
        {
            return;
        }

        void ActivateDockable(IDockable dockable)
        {
            dockFactory.SetActiveDockable(dockable);
            if (dockable.Owner is IDock owner)
            {
                dockFactory.SetFocusedDockable(owner, dockable);
            }
            dockFactory.ActivateWindow(dockable);
        }

        var hiddenDockable = rootDock.HiddenDockables?.FirstOrDefault(x => x.Id == id);
        if (hiddenDockable is { })
        {
            dockFactory.RestoreDockable(hiddenDockable);
            ActivateDockable(hiddenDockable);
            return;
        }

        var dockable = dockFactory.FindDockable(rootDock, x => x.Id == id);
        if (dockable is { })
        {
            if (dockable.Owner is IDock owner && owner.VisibleDockables?.Contains(dockable) == true)
            {
                dockFactory.HideDockable(dockable);
            }
            else
            {
                dockFactory.RestoreDockable(dockable);
                ActivateDockable(dockable);
            }

            return;
        }

        var restored = dockFactory.RestoreDockable(id);
        if (restored is { })
        {
            ActivateDockable(restored);
        }
    }

    public void ShowDialog(DialogViewModel? dialog)
    {
        if (dialog is { })
        {
            _dialogs?.Add(dialog);
        }
    }

    public void CloseDialog(DialogViewModel? dialog)
    {
        if (dialog is { })
        {
            _dialogs?.Remove(dialog);
        }
    }

    public DialogViewModel CreateTextBindingDialog(TextShapeViewModel text)
    {
        var textBindingEditor = new TextBindingEditorViewModel(ServiceProvider)
        {
            Editor = this,
            Text = text
        };

        return new DialogViewModel(ServiceProvider, this)
        {
            Title = "Text Binding",
            IsOverlayVisible = false,
            IsTitleBarVisible = true,
            IsCloseButtonVisible = true,
            ViewModel = textBindingEditor
        };
    }
}
