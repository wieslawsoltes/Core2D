// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Perspex;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
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
            if (values != null && values.Count() == 6 && values.All(x => x != PerspexProperty.UnsetValue))
            {
                var documentsLength = (int)values[0];
                var pagesLength = (int)values[1];
                var layersLength = (int)values[2];
                var shapesLength = (int)values[3];
                var selectedShape = (BaseShape)values[4];
                var selectedShapes = (ImmutableHashSet<BaseShape>)values[5];

                return string.Format("Documents: {0} - Pages: {1} - Layers: {2} - Shapes: {3} - Selected: {4}",
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
