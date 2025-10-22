// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia.Controls;
using Avalonia.Input;
using Core2D.ViewModels.Style;

namespace Core2D.Behaviors.DragAndDrop;

public class StylesListBoxDropHandler : ListBoxDropHandler
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return ValidateLibrary<ShapeStyleViewModel>(listBox, e, sourceContext, targetContext, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return ValidateLibrary<ShapeStyleViewModel>(listBox, e, sourceContext, targetContext, true);
        }
        return false;
    }
}
