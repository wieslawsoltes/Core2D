// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    public static class LineGrid
    {
        public struct Point
        {
            public double X;
            public double Y;
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public struct Size
        {
            public double Width;
            public double Height;
            public Size(double width, double height)
            {
                Width = width;
                Height = height;
            }
        }

        public struct Settings
        {
            public Point Origin;
            public Size GridSize;
            public Size CellSize;
            public static Settings Create(
                double originX,
                double originY,
                double gridWidth,
                double gridHeight,
                double cellWidth,
                double cellHeight)
            {
                return new Settings()
                {
                    Origin = new Point(originX, originY),
                    GridSize = new Size(gridWidth, gridHeight),
                    CellSize = new Size(cellWidth, cellHeight)
                };
            }
        }

        public static IList<BaseShape> Create(ShapeStyle style, Settings settings, BaseShape point)
        {
            double sx = settings.Origin.X + settings.CellSize.Width;
            double sy = settings.Origin.Y + settings.CellSize.Height;
            double ex = settings.Origin.X + settings.GridSize.Width;
            double ey = settings.Origin.Y + settings.GridSize.Height;

            var shapes = new List<BaseShape>();

            for (double x = sx; x < ex; x += settings.CellSize.Width)
            {
                var line = XLine.Create(
                    x,
                    settings.Origin.Y,
                    x,
                    ey,
                    style, point);
                line.State &= ~ShapeState.Printable;
                shapes.Add(line);
            }

            for (double y = sy; y < ey; y += settings.CellSize.Height)
            {
                var line = XLine.Create(
                    settings.Origin.X,
                    y,
                    ex,
                    y,
                    style, point);
                line.State &= ~ShapeState.Printable;
                shapes.Add(line);
            }

            return shapes;
        }
    }
}
