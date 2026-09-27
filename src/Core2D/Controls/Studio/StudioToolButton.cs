// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A drawing-tool selector retaining RadioButton grouping, keyboard input and automation.</summary>
public class StudioToolButton : RadioButton
{
    /// <summary>Defines the themeable tool image.</summary>
    public static readonly StyledProperty<IImage?> IconProperty =
        AvaloniaProperty.Register<StudioToolButton, IImage?>(nameof(Icon));

    /// <summary>Gets or sets the tool image. Existing Core2D drawing resources can be reused.</summary>
    public IImage? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
