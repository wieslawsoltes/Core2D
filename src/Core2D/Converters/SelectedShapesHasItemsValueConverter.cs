// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters;

public class SelectedShapesHasItemsValueConverter : IValueConverter
{
    public static SelectedShapesHasItemsValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ISet<BaseShapeViewModel> items && items.Count > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
