// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Intersections.Line
{
    public class LineLineIntersection : PointIntersection
    {
        public override string Title => "Line-Line";

        public LineLineSettings Settings { get; set; }

        public override void Find(IToolContext context, BaseShape shape)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            if (!Settings.IsEnabled)
                return;

            var lines = context.CurrentContainer.Shapes.OfType<LineShape>();
            if (lines.Any())
            {
                var a0 = line.StartPoint.ToPoint2();
                var b0 = line.Point.ToPoint2();
                foreach (var l in lines)
                {
                    var a1 = l.StartPoint.ToPoint2();
                    var b1 = l.Point.ToPoint2();
                    Point2 clip;
                    var intersection = Line2.LineIntersectWithLine(a0, b0, a1, b1, out clip);
                    if (intersection)
                    {
                        var point = new PointShape(clip.X, clip.Y, context.PointShape);
                        Intersections.Add(point);
                        context.WorkingContainer.Shapes.Add(point);
                        context.Renderer.Selected.Add(point);
                    }
                }
            }
        }
    }
}
