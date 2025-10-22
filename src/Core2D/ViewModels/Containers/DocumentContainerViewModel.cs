// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Containers;

public partial class DocumentContainerViewModel : BaseContainerViewModel
{
    [AutoNotify] private ImmutableArray<PageContainerViewModel> _pages;

    public DocumentContainerViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        AddPage = new RelayCommand<object?>(x => GetProject()?.OnAddPage(x));

        InsertDocumentBefore = new RelayCommand<object?>(x => GetProject()?.OnInsertDocumentBefore(x));

        InsertDocumentAfter = new RelayCommand<object?>(x => GetProject()?.OnInsertDocumentAfter(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand AddPage { get; }
        
    [IgnoreDataMember]
    public ICommand InsertDocumentBefore { get; }

    [IgnoreDataMember]
    public ICommand InsertDocumentAfter { get; }
        
    public override object Copy(IDictionary<object, object>? shared)
    {
        var pages = _pages.CopyShared(shared).ToImmutable();

        var copy = new DocumentContainerViewModel(ServiceProvider)
        {
            Name = Name,
            IsVisible = IsVisible,
            IsExpanded = IsExpanded,
            Pages = pages
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        foreach (var page in _pages)
        {
            isDirty |= page.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        foreach (var page in _pages)
        {
            page.Invalidate();
        }
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposablePages = default(CompositeDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveList(_pages, ref disposablePages, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pages))
            {
                ObserveList(_pages, ref disposablePages, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
