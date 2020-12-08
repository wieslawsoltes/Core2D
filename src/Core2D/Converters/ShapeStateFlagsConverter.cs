using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Renderer;

namespace Core2D.Converters
{
    public class ShapeStateFlagsConverter : IValueConverter
    {
        private ShapeStateFlags _target;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mask = (ShapeStateFlags)parameter;
            this._target = (ShapeStateFlags)value;
            return ((mask & this._target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this._target ^= (ShapeStateFlags)parameter;
            return this._target;
        }
    }
}
