#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters;

public class SelectedShapesIsEmptyValueConverter : IValueConverter
{
    public static SelectedShapesIsEmptyValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null || (value is ISet<BaseShapeViewModel> items && items.Count == 0);
    }

    public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}