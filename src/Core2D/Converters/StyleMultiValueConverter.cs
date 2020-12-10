using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Converters
{
    public class StyleMultiValueConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2)
            {
                if (values[0] is ISet<BaseShapeViewModel> shapes && shapes.Count > 0)
                {
                    return shapes.FirstOrDefault().Style;
                }

                if (values[1] is ShapeStyleViewModel style)
                {
                    return style;
                }
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
