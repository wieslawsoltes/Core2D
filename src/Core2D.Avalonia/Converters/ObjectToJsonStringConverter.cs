// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Markup;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Converts a binding value <see cref="object"/> <see cref="Type"/> to a json <see cref="string"/>.
    /// </summary>
    public class ObjectToJsonStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets json serializer.
        /// </summary>
        internal static Lazy<IJsonSerializer> JsonSerializer { get; set; }

        /// <summary>
        /// Gets an instance of a <see cref="ObjectToJsonStringConverter"/>.
        /// </summary>
        public static readonly ObjectToJsonStringConverter Instance = new ObjectToJsonStringConverter();

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
            if (value != null && value != AvaloniaProperty.UnsetValue)
            {
                try
                {
                    return JsonSerializer?.Value?.Serialize(value);
                }
                catch (Exception) { }
            }
            return AvaloniaProperty.UnsetValue;
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
