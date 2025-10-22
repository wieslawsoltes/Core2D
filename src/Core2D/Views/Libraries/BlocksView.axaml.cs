// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Interactivity;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.Views.Libraries;

public partial class BlocksView : UserControl
{
    public BlocksView()
    {
        InitializeComponent();
    }

    private void OnGroupDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ProjectContainerViewModel project && sender is Control control && control.DataContext is BlockShapeViewModel group)
        {
            project.OnEditGroup(group);
            e.Handled = true;
        }
    }
}
