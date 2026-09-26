// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;
using Core2D.Model.Editor;

namespace Core2D.ViewModels.Editor;

public partial class DialogViewModel : ViewModelBase
{
    private readonly IDialogPresenter _dialogPresenter;

    [AutoNotify] private string? _title;
    [AutoNotify] private bool _isOverlayVisible;
    [AutoNotify] private bool _isTitleBarVisible;
    [AutoNotify] private bool _isCloseButtonVisible;
    [AutoNotify] private ViewModelBase? _viewModel;

    public DialogViewModel(IServiceProvider? serviceProvider, IDialogPresenter dialogPresenter) : base(serviceProvider)
    {
        _dialogPresenter = dialogPresenter;
        CloseCommand = ReactiveCommand.Create(Close, outputScheduler: ImmediateScheduler.Instance);
    }

    /// <summary>Closes the dialog through its original presenter.</summary>
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public void Show()
    {
        _dialogPresenter.ShowDialog(this);
    }

    public void Close()
    {
        _dialogPresenter.CloseDialog(this);
    }
}
