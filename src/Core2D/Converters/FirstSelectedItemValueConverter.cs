using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters
{
    public class FirstSelectedItemValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ISet<BaseShapeViewModel> items && items.Count == 1)
            {
                return items.First();
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
