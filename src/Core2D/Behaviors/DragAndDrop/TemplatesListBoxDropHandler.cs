// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia.Controls;
using Avalonia.Input;

namespace Core2D.Behaviors.DragAndDrop;

public class TemplatesListBoxDropHandler : ListBoxDropHandler
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            // TODO:
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            // TODO:
        }
        return false;
    }
}
