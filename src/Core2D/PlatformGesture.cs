// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Core2D;

/// <summary>
/// Converts a gesture string that uses the <c>Primary</c> modifier placeholder
/// into a platform-specific <see cref="KeyGesture"/>.
/// </summary>
public sealed class PlatformGesture : MarkupExtension
{
    public PlatformGesture()
    {
    }

    public PlatformGesture(string text)
    {
        Text = text;
    }

    public string? Text { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            throw new InvalidOperationException("Text must be provided for PlatformGesture.");
        }

        var primary = OperatingSystem.IsMacOS() ? "Meta" : "Ctrl";
        var gesture = Text.Replace("Primary", primary, StringComparison.OrdinalIgnoreCase);

        return KeyGesture.Parse(gesture);
    }
}
