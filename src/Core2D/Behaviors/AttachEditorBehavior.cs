// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors;

public class AttachEditorBehavior : Behavior<Control>
{
    private AttachEditor? _input;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is { })
        {
            _input = new AttachEditor(AssociatedObject);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is { })
        {
            _input?.Detach();
        }
    }
}
