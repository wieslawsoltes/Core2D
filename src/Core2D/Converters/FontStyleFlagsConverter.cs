// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Model.Style;

namespace Core2D.Converters;

public class FontStyleFlagsConverter : IValueConverter
{
    public static FontStyleFlagsConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FontStyleFlags flags && parameter is FontStyleFlags flag)
        {
            return flags.HasFlag(flag);
        }
        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
