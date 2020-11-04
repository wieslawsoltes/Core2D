using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Core2D.Converters
{
    public class ArgbColorToBrushMultiConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count() == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (values[i].GetType() != typeof(byte))
                    {
                        return AvaloniaProperty.UnsetValue;
                    }
                }

                var color = Color.FromArgb(
                    (byte)values[0],
                    (byte)values[1],
                    (byte)values[2],
                    (byte)values[3]);

                return new SolidColorBrush(color);
            }

            return AvaloniaProperty.UnsetValue;
        }
    }
}
