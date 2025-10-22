// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Dock.Model.Mvvm.Controls;

namespace Core2D.ViewModels.Docking.Documents;

public class BlockDocumentViewModel : Document
{
    private BlockShapeViewModel? _group;
    private PageContainerViewModel? _container;

    public BlockShapeViewModel? Group
    {
        get => _group;
        set => SetProperty(ref _group, value);
    }

    public PageContainerViewModel? Container
    {
        get => _container;
        set => SetProperty(ref _container, value);
    }
}
