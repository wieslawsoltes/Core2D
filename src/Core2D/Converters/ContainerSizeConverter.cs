// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Containers;

namespace Core2D.Converters;

/// <summary>
/// Extracts width or height from the current container or its template.
/// </summary>
public sealed class ContainerSizeConverter : IValueConverter
{
    public static ContainerSizeConverter Width { get; } = new() { UseHeight = false };

    public static ContainerSizeConverter Height { get; } = new() { UseHeight = true };

    /// <summary>
    /// Gets or sets whether height should be returned instead of width.
    /// </summary>
    public bool UseHeight { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return 0d;
        }

        var container = value as FrameContainerViewModel;
        if (container is null)
        {
            return 0d;
        }

        var length = container switch
        {
            TemplateContainerViewModel template => UseHeight ? template.Height : template.Width,
            PageContainerViewModel { Template: { } template } => UseHeight ? template.Height : template.Width,
            _ => 0d
        };

        return length;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
