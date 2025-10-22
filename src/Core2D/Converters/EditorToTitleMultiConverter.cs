// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Converters;

public class EditorToTitleMultiConverter : IMultiValueConverter
{
    public static EditorToTitleMultiConverter Instance = new();

    public static readonly string s_defaultTitle = "Core2D";

    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is { } && values.Count() == 2 && values.All(x => x != AvaloniaProperty.UnsetValue))
        {
            if (values[0] is null || values[0]?.GetType() != typeof(string))
            {
                return s_defaultTitle;
            }

            if (values[1] is null || values[1]?.GetType() != typeof(bool))
            {
                return s_defaultTitle;
            }

            var name = (string?)values[0];
            var isDirty = (bool?)values[1];
            return string.Concat(name, (isDirty ?? false) ? "*" : "", " - ", s_defaultTitle);
        }

        return s_defaultTitle;
    }
}
