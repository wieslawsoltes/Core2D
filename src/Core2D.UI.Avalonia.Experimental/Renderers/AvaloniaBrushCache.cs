// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia.Media;
using Core2D.Style;

namespace Core2D.UI.Avalonia.Renderers
{
    public struct AvaloniaBrushCache : IDisposable
    {
        public readonly Brush Stroke;
        public readonly Pen StrokePen;
        public readonly Brush Fill;

        public AvaloniaBrushCache(Brush stroke, Pen strokePen, Brush fill)
        {
            this.Stroke = stroke;
            this.StrokePen = strokePen;
            this.Fill = fill;
        }

        public void Dispose()
        {
        }

        public static Color FromDrawColor(ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static AvaloniaBrushCache FromDrawStyle(ShapeStyle style)
        {
            Brush stroke = null;
            Pen strokePen = null;
            Brush fill = null;

            if (style.Stroke != null)
            {
                stroke = new SolidColorBrush(FromDrawColor(style.Stroke));
                strokePen = new Pen(stroke, style.Thickness);
            }

            if (style.Fill != null)
            {
                fill = new SolidColorBrush(FromDrawColor(style.Fill));
            }

            return new AvaloniaBrushCache(stroke, strokePen, fill);
        }
    }
}
