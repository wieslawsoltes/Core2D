using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.UI.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class StyleMultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts multi-binding inputs to a final value.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2)
            {
                if (values[0] is ISet<IBaseShape> shapes)
                {
                    var styles = new List<IShapeStyle>();
                    foreach (var shape in shapes)
                    {
                        styles.Add(shape.Style);
                    }
                    return styles;
                }

                if (values[1] is IShapeStyle style)
                {
                    var styles = new List<IShapeStyle>();
                    styles.Add(style);
                    return styles;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
