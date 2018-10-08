// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Intersections
{
    public static class LineHelper
    {
        public static IList<LineShape> SplitByIntersections(IToolContext context, IEnumerable<PointIntersection> intersections, LineShape target)
        {
            var points = intersections.SelectMany(i => i.Intersections).ToList();
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = points
                .Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                .Select(p => new PointShape(p.X, p.Y, context.PointShape)).ToList();

            var lines = new List<LineShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var line = new LineShape(unique[i], unique[i + 1]);
                line.Style = context.CurrentStyle;

                context.CurrentContainer.Shapes.Add(line);
                lines.Add(line);
            }

            return lines;
        }
    }
}
