using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters
{
    public class BaseShapeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BaseShapeViewModel shape)
            {
                var key = value.GetType().Name.Replace("Shape", "");

                if (Application.Current.Styles.TryGetResource(key, out var resource))
                {
                    return resource;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
