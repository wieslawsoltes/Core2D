// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Core2D.Model.History;

namespace Core2D.Controls.Studio;

/// <summary>Supplies document-scoped edit services through the logical tree without service location.</summary>
public sealed class StudioEditContext : AvaloniaObject
{
    /// <summary>Defines the inherited document history service.</summary>
    public static readonly AttachedProperty<IHistory?> HistoryProperty =
        AvaloniaProperty.RegisterAttached<StudioEditContext, Control, IHistory?>("History", inherits: true);

    /// <summary>Gets the document history service.</summary>
    public static IHistory? GetHistory(Control control) => control.GetValue(HistoryProperty);
    /// <summary>Sets the document history service.</summary>
    public static void SetHistory(Control control, IHistory? value) => control.SetValue(HistoryProperty, value);
}
