// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public static class LineGrid
    {
        /// <summary>
        /// 
        /// </summary>
        public struct Point
        {
            /// <summary>
            /// 
            /// </summary>
            public double X;
            /// <summary>
            /// 
            /// </summary>
            public double Y;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct Size
        {
            /// <summary>
            /// 
            /// </summary>
            public double Width;
            /// <summary>
            /// 
            /// </summary>
            public double Height;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public Size(double width, double height)
            {
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// 
            /// </summary>
            public Point Origin;
            /// <summary>
            /// 
            /// </summary>
            public Size GridSize;
            /// <summary>
            /// 
            /// </summary>
            public Size CellSize;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="originX"></param>
            /// <param name="originY"></param>
            /// <param name="gridWidth"></param>
            /// <param name="gridHeight"></param>
            /// <param name="cellWidth"></param>
            /// <param name="cellHeight"></param>
            /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="settings"></param>
        /// <param name="point"></param>
        /// <returns></returns>
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
