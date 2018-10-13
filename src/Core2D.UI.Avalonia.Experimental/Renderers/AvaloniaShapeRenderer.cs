// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.UI.Avalonia.Renderers
{
    public class AvaloniaShapeRenderer : ShapeRenderer
    {
        private readonly IDictionary<ShapeStyle, AvaloniaBrushCache> _brushCache;
        private readonly IDictionary<MatrixObject, Matrix> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private readonly IDictionary<EllipseShape, Geometry> _ellipseGeometryCache;
        private readonly IDictionary<TextShape, FormattedTextCache> _formattedTextCache;

        private BaseShape _hover;
        private ISet<BaseShape> _selected;

        public override BaseShape HoveredShape
        {
            get => _hover;
            set => Update(ref _hover, value);
        }

        public override ISet<BaseShape> SelectedShapes
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public AvaloniaShapeRenderer()
        {
            _brushCache = new Dictionary<ShapeStyle, AvaloniaBrushCache>();
            _formattedTextCache = new Dictionary<TextShape, FormattedTextCache>();
            _matrixCache = new Dictionary<MatrixObject, Matrix>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _ellipseGeometryCache = new Dictionary<EllipseShape, Geometry>();
            _hover = null;
            _selected = new HashSet<BaseShape>();
        }

        private static Point ToPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static IEnumerable<Point> ToPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new Point(point.X + dx, point.Y + dy));
        }

        private static Rect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double x = Math.Min(p1.X + dx, p2.X + dx);
            double y = Math.Min(p1.Y + dy, p2.Y + dy);
            double width = Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - x);
            double height = Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - y);
            return new Rect(x, y, width, height);
        }

        private AvaloniaBrushCache? GetBrushCache(ShapeStyle style)
        {
            if (style == null)
            {
                return null;
            }
            if (!_brushCache.TryGetValue(style, out var cache))
            {
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private static Matrix ToMatrixTransform(MatrixObject matrix)
        {
            return new Matrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        private Matrix? GetMatrixCache(MatrixObject matrix)
        {
            if (matrix == null)
            {
                return null;
            }
            if (!_matrixCache.TryGetValue(matrix, out var cache))
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
                return _matrixCache[matrix];
            }
            return cache;
        }

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), false);
                context.CubicBezierTo(
                    ToPoint(cubicBezier.Point1, dx, dy),
                    ToPoint(cubicBezier.Point2, dx, dy),
                    ToPoint(cubicBezier.Point3, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), false);
                context.QuadraticBezierTo(
                    ToPoint(quadraticBezier.Point1, dx, dy),
                    ToPoint(quadraticBezier.Point2, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.SetFillRule(path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);

                foreach (var figure in path.Figures)
                {
                    bool isFirstShape = true;
                    foreach (var shape in figure.Shapes)
                    {
                        if (shape is LineShape line)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(line.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.LineTo(ToPoint(line.Point, dx, dy));
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.CubicBezierTo(
                                ToPoint(cubicBezier.Point1, dx, dy),
                                ToPoint(cubicBezier.Point2, dx, dy),
                                ToPoint(cubicBezier.Point3, dx, dy));
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                ToPoint(quadraticBezier.Point1, dx, dy),
                                ToPoint(quadraticBezier.Point2, dx, dy));
                        }
                    }

                    if (!isFirstShape)
                    {
                        context.EndFigure(figure.IsClosed);
                    }
                }
            }

            return geometry;
        }

        private static Geometry ToGeometry(EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            return new EllipseGeometry(rect);
        }

        private Geometry GetGeometryCache(CubicBezierShape cubic, ShapeStyle style, double dx, double dy)
        {
            if (cubic == null)
            {
                return null;
            }
            if (!_cubicGeometryCache.TryGetValue(cubic, out var cache))
            {
                var geometry = ToGeometry(cubic, style, dx, dy);
                if (geometry != null)
                {
                    _cubicGeometryCache[cubic] = geometry;
                    return _cubicGeometryCache[cubic];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(QuadraticBezierShape quad, ShapeStyle style, double dx, double dy)
        {
            if (quad == null)
            {
                return null;
            }
            if (!_quadGeometryCache.TryGetValue(quad, out var cache))
            {
                var geometry = ToGeometry(quad, style, dx, dy);
                if (geometry != null)
                {
                    _quadGeometryCache[quad] = geometry;
                    return _quadGeometryCache[quad];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(PathShape path, ShapeStyle style, double dx, double dy)
        {
            if (path == null)
            {
                return null;
            }
            if (!_pathGeometryCache.TryGetValue(path, out var cache))
            {
                var geometry = ToGeometry(path, style, dx, dy);
                if (geometry != null)
                {
                    _pathGeometryCache[path] = geometry;
                    return _pathGeometryCache[path];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            if (ellipse == null)
            {
                return null;
            }
            if (!_ellipseGeometryCache.TryGetValue(ellipse, out var cache))
            {
                var geometry = ToGeometry(ellipse, style, dx, dy);
                if (geometry != null)
                {
                    _ellipseGeometryCache[ellipse] = geometry;
                    return _ellipseGeometryCache[ellipse];
                }
                return null;
            }
            return cache;
        }

        private FormattedTextCache GetTextCache(TextShape text, Rect rect)
        {
            if (!_formattedTextCache.TryGetValue(text, out var cache))
            {
                _formattedTextCache[text] = FormattedTextCache.FromTextShape(text, rect);
                return _formattedTextCache[text];
            }
            return cache;
        }

        public override void InvalidateCache(ShapeStyle style)
        {
            if (style != null)
            {
                if (!_brushCache.TryGetValue(style, out var cache))
                {
                    cache.Dispose();
                }
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
            }
        }

        public override void InvalidateCache(MatrixObject matrix)
        {
            if (matrix != null)
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
            }
        }

        public override void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy)
        {
            switch (shape)
            {
                case CubicBezierShape cubic:
                    {
                        var geometry = ToGeometry(cubic, style, dx, dy);
                        if (geometry != null)
                        {
                            _cubicGeometryCache[cubic] = geometry;
                        }
                    }
                    break;
                case QuadraticBezierShape quad:
                    {
                        var geometry = ToGeometry(quad, style, dx, dy);
                        if (geometry != null)
                        {
                            _quadGeometryCache[quad] = geometry;
                        }
                    }
                    break;
                case PathShape path:
                    {
                        var geometry = ToGeometry(path, style, dx, dy);
                        if (geometry != null)
                        {
                            _pathGeometryCache[path] = geometry;
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        var geometry = ToGeometry(ellipse, style, dx, dy);
                        if (geometry != null)
                        {
                            _ellipseGeometryCache[ellipse] = geometry;
                        }
                    }
                    break;
                case TextShape text:
                    {
                        if (!_formattedTextCache.TryGetValue(text, out var cache))
                        {
                            cache.Dispose();
                        }
                        var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);
                        _formattedTextCache[text] = FormattedTextCache.FromTextShape(text, rect);
                    }
                    break;
            }
        }

        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            return _dc.PushPreTransform(GetMatrixCache(matrix).Value);
        }

        public override void PopMatrix(object dc, object state)
        {
            var _state = (DrawingContext.PushedState)state;
            _state.Dispose();
        }

        public override void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy));
        }

        public override void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(ellipse, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);

            //if (style.IsFilled)
            //{
            //    _dc.FillRectangle(cache?.Fill, rect);
            //}

            //if (style.IsStroked)
            //{
            //    _dc.DrawRectangle(cache?.StrokePen, rect);
            //}

            if (text.Text != null)
            {
                var ftc = GetTextCache(text, rect);
                _dc.DrawText(cache?.Stroke, ftc.Origin, ftc.FormattedText);
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
