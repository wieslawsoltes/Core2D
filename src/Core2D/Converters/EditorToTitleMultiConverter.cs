using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Converters
{
    public class EditorToTitleMultiConverter : IMultiValueConverter
    {
        public static readonly string s_defaultTitle = "Core2D";

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count() == 2 && values.All(x => x != AvaloniaProperty.UnsetValue))
            {
                if (values[0] == null || values[0].GetType() != typeof(string))
                {
                    return s_defaultTitle;
                }

                if (values[1] == null || values[1].GetType() != typeof(bool))
                {
                    return s_defaultTitle;
                }

                string name = (string)values[0];
                bool isDirty = (bool)values[1];
                return string.Concat(name, isDirty ? "*" : "", " - ", s_defaultTitle);
            }

            return s_defaultTitle;
        }
    }
}
