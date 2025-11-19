// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            ? new Lazy<ImmutableArray<IEditorTool>>(() => ImmutableArray<IEditorTool>.Empty) 
            : serviceProvider.GetServiceLazily<IEnumerable<IEditorTool>, ImmutableArray<IEditorTool>>(tools =>
            {
                if (tools is null)
                {
                    return ImmutableArray<IEditorTool>.Empty;
                }
                return tools.ToImmutableArray();
            });
        _pathTools = serviceProvider is null 
            ? new Lazy<ImmutableArray<IPathTool>>(() => ImmutableArray<IPathTool>.Empty) 
            : serviceProvider.GetServiceLazily<IEnumerable<IPathTool>, ImmutableArray<IPathTool>>(pathTools =>
            {
                if (pathTools is null)
                {
                    return ImmutableArray<IPathTool>.Empty;
                }
                return pathTools.ToImmutableArray();
            });
        _dataFlow = serviceProvider.GetServiceLazily<DataFlow>();
        _renderer = serviceProvider.GetServiceLazily<IShapeRenderer>();
        _libraryRenderer = serviceProvider.GetServiceLazily<IShapeRenderer>();
        _rendererSelectionService = serviceProvider.GetServiceLazily<IRendererSelectionService>(HookRendererSelection);
        _selectionService = serviceProvider.GetServiceLazily<ISelectionService>();
        _shapeService = serviceProvider.GetServiceLazily<IShapeService>();
        _graphLayoutService = serviceProvider.GetServiceLazily<IGraphLayoutService>();
        _waveFunctionCollapseService = serviceProvider.GetServiceLazily<IWaveFunctionCollapseService>();
        _clipboardService = serviceProvider.GetServiceLazily<IClipboardService>();
        _fileWriters = serviceProvider is null 
            ? new Lazy<ImmutableArray<IFileWriter>>(() => ImmutableArray<IFileWriter>.Empty) 
            : serviceProvider.GetServiceLazily<IEnumerable<IFileWriter>, ImmutableArray<IFileWriter>>(writers =>
            {
                if (writers is null)
                {
                    return ImmutableArray<IFileWriter>.Empty;
                }
                return writers.ToImmutableArray();
            });  
        _textFieldReaders = serviceProvider is null 
            ? new Lazy<ImmutableArray<ITextFieldReader<DatabaseViewModel>>>(() => ImmutableArray<ITextFieldReader<DatabaseViewModel>>.Empty) 
            : serviceProvider.GetServiceLazily<IEnumerable<ITextFieldReader<DatabaseViewModel>>, ImmutableArray<ITextFieldReader<DatabaseViewModel>>>(readers =>
            {
                if (readers is null)
                {
                    return ImmutableArray<ITextFieldReader<DatabaseViewModel>>.Empty;
                }
                return readers.ToImmutableArray();
            });
        _textFieldWriters = serviceProvider is null 
            ? new Lazy<ImmutableArray<ITextFieldWriter<DatabaseViewModel>>>(() => ImmutableArray<ITextFieldWriter<DatabaseViewModel>>.Empty) 
            : serviceProvider.GetServiceLazily<IEnumerable<ITextFieldWriter<DatabaseViewModel>>, ImmutableArray<ITextFieldWriter<DatabaseViewModel>>>(writers =>
            {
                if (writers is null)
                {
                    return ImmutableArray<ITextFieldWriter<DatabaseViewModel>>.Empty;
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

public partial class ProjectEditorViewModel
{
    private void HookRendererSelection(IRendererSelectionService service)
    {
        if (service is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnRendererSelectionChanged;
        }
    }

    private void OnRendererSelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName)
            || e.PropertyName == nameof(IRendererSelectionService.Renderer))
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Renderer)));
            RaisePropertyChanged(new PropertyChangedEventArgs(nameof(LibraryRenderer)));
            RaisePropertyChanged(new PropertyChangedEventArgs(nameof(PageState)));
        }
    }
}
