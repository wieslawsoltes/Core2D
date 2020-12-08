using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core2D.Style;

namespace Core2D.Converters
{
    public class FontStyleFlagsConverter : IValueConverter
    {
        private FontStyleFlags _target;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mask = (FontStyleFlags)parameter;
            this._target = (FontStyleFlags)value;
            return ((mask & this._target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this._target ^= (FontStyleFlags)parameter;
            return this._target;
        }
    }
}
