// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A native text label using explicit model display contracts, not reflected property paths.</summary>
public class StudioItemLabel : TextBlock
{
    /// <summary>Defines the original item whose name, title or image key is displayed.</summary>
    public static readonly StyledProperty<object?> ItemProperty =
        AvaloniaProperty.Register<StudioItemLabel, object?>(nameof(Item));

    /// <summary>Gets or sets the original item.</summary>
    public object? Item { get => GetValue(ItemProperty); set => SetValue(ItemProperty, value); }

    /// <inheritdoc />
    protected override System.Type StyleKeyOverride => typeof(TextBlock);

    /// <summary>Initializes attachment-scoped item observation.</summary>
    public StudioItemLabel() => Interaction.GetBehaviors(this).Add(new StudioItemLabelBehavior());
}
