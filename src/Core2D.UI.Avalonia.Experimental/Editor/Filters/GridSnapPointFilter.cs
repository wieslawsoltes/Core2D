// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using Core2D.Shapes;

namespace Core2D.Editor.Filters
{
    public class GridSnapPointFilter : PointFilter
    {
        public override string Title => "Grid-Snap";

        public GridSnapSettings Settings { get; set; }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Mode != GridSnapMode.None)
            {
                bool haveSnapToGrid = false;

                if (Settings.Mode.HasFlag(GridSnapMode.Horizontal))
                {
                    x = SnapGrid(x, Settings.GridSizeX);
                    haveSnapToGrid = true;
                }

                if (Settings.Mode.HasFlag(GridSnapMode.Vertical))
                {
                    y = SnapGrid(y, Settings.GridSizeY);
                    haveSnapToGrid = true;
                }

                if (Settings.EnableGuides && haveSnapToGrid)
                {
                    PointGuides(context, x, y);
                }

                if (haveSnapToGrid)
                {
                    Log.Info($"Grid Snap {Settings.Mode}");
                }

                return haveSnapToGrid;
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.CurrentContainer.Width, y, null),
                Style = Settings.GuideStyle
            };

            var vertical = new LineShape()
            {
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.CurrentContainer.Height, null),
                Style = Settings.GuideStyle
            };

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.WorkingContainer.Shapes.Add(horizontal);
            context.WorkingContainer.Shapes.Add(vertical);
        }

        public static double SnapGrid(double value, double size)
        {
            double r = value % size;
            return r >= size / 2.0 ? value + size - r : value - r;
        }
    }
}
