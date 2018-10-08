// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Intersections.Line
{
    public class RectangleLineIntersection : PointIntersection
    {
        public override string Title => "Rectangle-Line";

        public RectangleLineSettings Settings { get; set; }

        public override void Find(IToolContext context, BaseShape shape)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            if (!Settings.IsEnabled)
                return;

            var rectangles = context.CurrentContainer.Shapes.OfType<RectangleShape>();
            if (rectangles.Any())
            {
                foreach (var rectangle in rectangles)
                {
                    var rect = Rect2.FromPoints(rectangle.TopLeft.ToPoint2(), rectangle.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    var intersections = Line2.LineIntersectsWithRect(p1, p2, rect, out double x0clip, out double y0clip, out double x1clip, out double y1clip);
                    if (intersections)
                    {
                        var point1 = new PointShape(x0clip, y0clip, context.PointShape);
                        Intersections.Add(point1);
                        context.WorkingContainer.Shapes.Add(point1);
                        context.Renderer.Selected.Add(point1);

                        var point2 = new PointShape(x1clip, y1clip, context.PointShape);
                        Intersections.Add(point2);
                        context.WorkingContainer.Shapes.Add(point2);
                        context.Renderer.Selected.Add(point2);
                    }
                }
            }
        }
    }
}
