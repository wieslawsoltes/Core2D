// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Helpers
{
    public abstract class CommonHelper : ShapeHelper
    {
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private ShapeStyle _strokeStyle;
        private ShapeStyle _fillStyle;
        private LineShape _line;
        private EllipseShape _ellipse;

        public CommonHelper()
        {
            _stroke = new ArgbColor(255, 0, 255, 255);
            _fill = new ArgbColor(255, 0, 255, 255);
            _strokeStyle = new ShapeStyle(_stroke, _fill, 2.0, true, false);
            _fillStyle = new ShapeStyle(_stroke, _fill, 2.0, false, true);
            _line = new LineShape(new PointShape(0, 0, null), new PointShape(0, 0, null));
            _ellipse = new EllipseShape(new PointShape(0, 0, null), new PointShape(0, 0, null));
        }

        public void DrawLine(object dc, ShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy)
        {
            _line.Style = _strokeStyle;
            _line.StartPoint.X = a.X;
            _line.StartPoint.Y = a.Y;
            _line.Point.X = b.X;
            _line.Point.Y = b.Y;
            _line.Draw(dc, renderer, dx, dy, null, null);
        }

        public void FillEllipse(object dc, ShapeRenderer renderer, PointShape s, double radius, double dx, double dy)
        {
            _ellipse.Style = _fillStyle;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, null, null);
        }

        public void DrawEllipse(object dc, ShapeRenderer renderer, PointShape s, double radius, double dx, double dy)
        {
            _ellipse.Style = _strokeStyle;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, null, null);
        }
    }
}
