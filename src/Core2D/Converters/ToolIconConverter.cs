#nullable disable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Model.Editor;

namespace Core2D.Converters
{
    public class ToolIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITool tool)
            {
                var key = $"{tool.Title}";

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
