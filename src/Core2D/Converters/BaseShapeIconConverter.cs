﻿#nullable enable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters;

public class BaseShapeIconConverter : IValueConverter
{
    public static BaseShapeIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BaseShapeViewModel shape)
        {
            var key = value.GetType().Name.Replace("ShapeViewModel", "");

            if (Application.Current is { } application)
            {
                if (application.Styles.TryGetResource(key, out var resource))
                {
                    return resource;
                }
            }
        }
        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
