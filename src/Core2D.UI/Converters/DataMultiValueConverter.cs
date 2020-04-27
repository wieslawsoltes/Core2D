using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Shapes;

namespace Core2D.UI.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class DataMultiValueConverter : IMultiValueConverter
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
                    var contexts = new List<IContext>();
                    foreach (var shape in shapes)
                    {
                        contexts.Add(shape.Data);
                    }
                    return contexts;
                }

                if (values[1] is IPageContainer container)
                {
                    var contexts = new List<IContext>();
                    contexts.Add(container.Data);
                    return contexts;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
