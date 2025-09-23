// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Core2D.ViewModels.Containers;

namespace Core2D.Controls.Editor;

public partial class PageView : UserControl
{
    public static readonly StyledProperty<FrameContainerViewModel?> ContainerProperty =
        AvaloniaProperty.Register<PageView, FrameContainerViewModel?>(nameof(Container));

    public FrameContainerViewModel? Container
    {
        get => GetValue(ContainerProperty);
        set => SetValue(ContainerProperty, value);
    }

    public PageView()
    {
        InitializeComponent();
    }
}
