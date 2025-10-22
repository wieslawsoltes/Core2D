// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Core2D.Behaviors.DragAndDrop;

public abstract class DefaultDropHandler : AvaloniaObject, IDropHandler
{
    public static Point GetPosition(Control? relativeTo, DragEventArgs e)
    {
        relativeTo ??= e.Source as Control;
        var point = relativeTo is { } ? e.GetPosition(relativeTo) : new Point();
        return point;
    }

    public virtual void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Validate(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    public virtual void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Validate(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    public virtual void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Execute(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    public virtual void Leave(object? sender, RoutedEventArgs e)
    {
        Cancel(sender, e);
    }

    public virtual bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return false;
    }

    public virtual bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return false;
    }

    public virtual void Cancel(object? sender, RoutedEventArgs e)
    {
    }
}
