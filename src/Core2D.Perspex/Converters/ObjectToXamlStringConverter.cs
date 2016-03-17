// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Perspex;
using Perspex.Markup;
using Serializer.Xaml;
using System;
using System.Globalization;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Converts a binding value <see cref="object"/> <see cref="Type"/> to a xaml <see cref="string"/>.
    /// </summary>
    public sealed class ObjectToXamlStringConverter : IValueConverter
    {
        private static ITextSerializer XamlSerializer = new PortableXamlSerializer();

        /// <summary>
        /// Gets an instance of a <see cref="ObjectToXamlStringConverter"/>.
        /// </summary>
        public static readonly ObjectToXamlStringConverter Instance = new ObjectToXamlStringConverter();

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
            if (value != null && value != PerspexProperty.UnsetValue)
            {
                try
                {
                    return XamlSerializer?.Serialize(value);
                }
                catch (Exception) { }
            }
            return PerspexProperty.UnsetValue;
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
