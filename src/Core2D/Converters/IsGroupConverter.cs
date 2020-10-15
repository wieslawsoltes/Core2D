using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core2D.Shapes;

namespace Core2D.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="object"/> to <see cref="bool"/> True if value is not equal to null and is of type <see cref="GroupShape"/> otherwise return False.
    /// </summary>
    public class IsGroupConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.GetType() == typeof(GroupShape);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
