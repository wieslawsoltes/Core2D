// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Core2D.Avalonia.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class EditorToStatsMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Default stats result.
        /// </summary>
        public static readonly string DefaultStats = "";

        /// <summary>
        /// Gets an instance of a <see cref="EditorToStatsMultiConverter"/>.
        /// </summary>
        public static readonly EditorToStatsMultiConverter Instance = new EditorToStatsMultiConverter();

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
            if (values != null && values.Count() == 6 && values.All(x => x != AvaloniaProperty.UnsetValue))
            {
                int documentsLength = values[0] != null && values[0].GetType() == typeof(int) ? (int)values[0] : 0;
                int pagesLength = values[1] != null && values[1].GetType() == typeof(int) ? (int)values[1] : 0;
                int layersLength = values[2] != null && values[2].GetType() == typeof(int) ? (int)values[2] : 0;
                int shapesLength = values[3] != null && values[3].GetType() == typeof(int) ? (int)values[3] : 0;
                BaseShape selectedShape = values[4] != null && values[4].GetType() == typeof(BaseShape) ? (BaseShape)values[4] : null;
                ImmutableHashSet<BaseShape> selectedShapes = values[5] != null && values[5].GetType() == typeof(ImmutableHashSet<BaseShape>) ? (ImmutableHashSet<BaseShape>)values[5] : null;
                return string.Format(
                    "Documents: {0} - Pages: {1} - Layers: {2} - Shapes: {3} - Selected: {4}",
                    documentsLength,
                    pagesLength,
                    layersLength,
                    shapesLength,
                    selectedShape != null ? 1 : (selectedShapes != null) ? selectedShapes.Count : 0);
            }

            return DefaultStats;
        }
    }
}
