﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core2D.Shapes;

namespace Core2D.Converters
{
    public class IsGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.GetType() == typeof(GroupShape);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}