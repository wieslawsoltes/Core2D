// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;

namespace Core2D.ViewModels.Docking.Documents;

public class PageViewModel : Document
{
    private PageContainerViewModel? _page;

    public PageContainerViewModel? Page
    {
        get => _page;
        set => this.RaiseAndSetIfChanged(ref _page, value);
    }
}
