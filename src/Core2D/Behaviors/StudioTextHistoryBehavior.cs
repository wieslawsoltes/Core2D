// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Style;

namespace Core2D.Behaviors;

/// <summary>Records explicit name/dash model edits without retaining controls in the undo stack.</summary>
public sealed class StudioTextHistoryBehavior : Behavior<StudioTextField>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } field) field.Committed += OnCommitted;
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } field) field.Committed -= OnCommitted;
        base.OnDetaching();
    }
    private void OnCommitted(object? sender, StudioTextCommittedEventArgs e)
    {
        if (AssociatedObject is not { } field || StudioEditContext.GetHistory(field) is not { } history) return;
        Action<string?>? update = null;
        if (field is StudioNameField && field.DataContext is ViewModelBase target && target.Name == e.Next)
            update = value => target.Name = value ?? string.Empty;
        else if (field is StudioDashField && field.DataContext is StrokeStyleViewModel stroke && stroke.Dashes == e.Next)
            update = value => stroke.Dashes = value;
        if (update is not null) history.Snapshot(e.Previous, e.Next, update);
    }
}
