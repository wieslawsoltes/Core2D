using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.UI.Converters
{
    /// <summary>
    /// Converts multi-binding inputs to a final value.
    /// </summary>
    public class EditorToStatsMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Default stats result.
        /// </summary>
        public static readonly string s_defaultStats = "";

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
            if (values != null && values.Count() == 5 && values.All(x => x != AvaloniaProperty.UnsetValue))
            {
                int nDocuments = 0;
                int nPages = 0;
                int nLayers = 0;
                int nShapes = 0;
                int nSelectedShapes = 0;

                if (values[0] is ImmutableArray<DocumentContainer> documents)
                {
                    nDocuments = documents.Length;
                }

                if (values[1] is ImmutableArray<PageContainer> pages)
                {
                    nPages = pages.Length;
                }

                if (values[2] is ImmutableArray<LayerContainer> layers)
                {
                    nLayers = layers.Length;
                }

                if (values[3] is ImmutableArray<BaseShape> shapes)
                {
                    nShapes = shapes.Length;
                }

                if (values[4] is HashSet<BaseShape> selectedShapes)
                {
                    nSelectedShapes = selectedShapes.Count;
                }

                return $"Documents: {nDocuments} Pages: {nPages} Layers: {nLayers} Shapes: {nShapes} Selected: {nSelectedShapes}";
            }
            return s_defaultStats;
        }
    }
}
