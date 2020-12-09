using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Core2D.Renderer.PdfSharp
{
    public partial class PdfSharpRenderer : ViewModelBase, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private ShapeRendererStateViewModel _stateViewModel;
        private ICache<string, XImage> _biCache;
        private Func<double, double> _scaleToPage;
        private double _sourceDpi = 96.0;
        private double _targetDpi = 72.0;

        public ShapeRendererStateViewModel StateViewModel
        {
            get => _stateViewModel;
            set => RaiseAndSetIfChanged(ref _stateViewModel, value);
        }

        public PdfSharpRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stateViewModel = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, XImage>(bi => bi.Dispose());
            _scaleToPage = (value) => (float)(value * 1.0);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static XColor ToXColor(BaseColorViewModel colorViewModel)
        {
            return colorViewModel switch
            {
                ArgbColorViewModelViewModel argbColor => XColor.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
                _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported."),
            };
        }

        private static XPen ToXPen(ShapeStyleViewModel styleViewModel, Func<double, double> scale, double sourceDpi, double targetDpi)
        {
            var strokeWidth = scale(styleViewModel.Stroke.Thickness * targetDpi / sourceDpi);
            var pen = new XPen(ToXColor(styleViewModel.Stroke.ColorViewModel), strokeWidth);
            switch (styleViewModel.Stroke.LineCap)
            {
                case LineCap.Flat:
                    pen.LineCap = XLineCap.Flat;
                    break;

                case LineCap.Square:
                    pen.LineCap = XLineCap.Square;
                    break;

                case LineCap.Round:
                    pen.LineCap = XLineCap.Round;
                    break;
            }
            if (styleViewModel.Stroke.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                var dashPattern = StyleHelper.ConvertDashesToDoubleArray(styleViewModel.Stroke.Dashes, strokeWidth);
                var dashOffset = styleViewModel.Stroke.DashOffset * strokeWidth;
                if (dashPattern != null)
                {
                    pen.DashPattern = dashPattern;
                    pen.DashStyle = XDashStyle.Custom;
                    pen.DashOffset = dashOffset;
                }
            }
            return pen;
        }

        private static XPen ToXPen(BaseColorViewModel colorViewModel, double thickness, Func<double, double> scale, double sourceDpi, double targetDpi)
        {
            var strokeWidth = scale(thickness * targetDpi / sourceDpi);
            var pen = new XPen(ToXColor(colorViewModel), strokeWidth);
            pen.LineCap = XLineCap.Flat;
            return pen;
        }

        private static XBrush ToXBrush(BaseColorViewModel colorViewModel)
        {
            return colorViewModel switch
            {
                ArgbColorViewModelViewModel argbColor => new XSolidBrush(ToXColor(argbColor)),
                _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported."),
            };
        }

        private static void DrawLineInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint p0, ref XPoint p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        private void DrawLineArrowsInternal(XGraphics gfx, LineShapeViewModel line, out XPoint pt1, out XPoint pt2)
        {
            var fillStartArrow = ToXBrush(line.StyleViewModel.Fill.ColorViewModel);
            var strokeStartArrow = ToXPen(line.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi);

            var fillEndArrow = ToXBrush(line.StyleViewModel.Fill.ColorViewModel);
            var strokeEndArrow = ToXPen(line.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi);

            double _x1 = line.Start.X;
            double _y1 = line.Start.Y;
            double _x2 = line.End.X;
            double _y2 = line.End.Y;

            double x1 = _scaleToPage(_x1);
            double y1 = _scaleToPage(_y1);
            double x2 = _scaleToPage(_x2);
            double y2 = _scaleToPage(_y2);

            var sas = line.StyleViewModel.Stroke.StartArrowStyleViewModel;
            var eas = line.StyleViewModel.Stroke.EndArrowStyleViewModel;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas, line);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas, line);
        }

        private static XPoint DrawLineArrowInternal(XGraphics gfx, XPen pen, XBrush brush, double x, double y, double angle, ArrowStyleViewModel styleViewModel, BaseShapeViewModel shapeViewModel)
        {
            XPoint pt;
            var rt = new XMatrix();
            var c = new XPoint(x, y);
            rt.RotateAtPrepend(angle, c);
            double rx = styleViewModel.RadiusX;
            double ry = styleViewModel.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (styleViewModel.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new XPoint(x, y);
                    }
                    break;

                case ArrowType.Rectangle:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        DrawRectangleInternal(gfx, brush, pen, shapeViewModel.IsStroked, shapeViewModel.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;

                case ArrowType.Ellipse:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(gfx, brush, pen, shapeViewModel.IsStroked, shapeViewModel.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;

                case ArrowType.Arrow:
                    {
                        pt = rt.Transform(new XPoint(x, y));
                        var p11 = rt.Transform(new XPoint(x - sx, y + sy));
                        var p21 = rt.Transform(new XPoint(x, y));
                        var p12 = rt.Transform(new XPoint(x - sx, y - sy));
                        var p22 = rt.Transform(new XPoint(x, y));
                        DrawLineInternal(gfx, pen, shapeViewModel.IsStroked, ref p11, ref p21);
                        DrawLineInternal(gfx, pen, shapeViewModel.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
        {
            if (isStroked && isFilled)
            {
                gfx.DrawRectangle(pen, brush, rect);
            }
            else if (isStroked && !isFilled)
            {
                gfx.DrawRectangle(pen, rect);
            }
            else if (!isStroked && isFilled)
            {
                gfx.DrawRectangle(brush, rect);
            }
        }

        private static void DrawEllipseInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
        {
            if (isStroked && isFilled)
            {
                gfx.DrawEllipse(pen, brush, rect);
            }
            else if (isStroked && !isFilled)
            {
                gfx.DrawEllipse(pen, rect);
            }
            else if (!isStroked && isFilled)
            {
                gfx.DrawEllipse(brush, rect);
            }
        }

        private void DrawGridInternal(XGraphics gfx, IGrid grid, XPen stroke, ref Spatial.Rect2 rect)
        {
            double ox = rect.X;
            double ex = rect.X + rect.Width;
            double oy = rect.Y;
            double ey = rect.Y + rect.Height;
            double cw = grid.GridCellWidth;
            double ch = grid.GridCellHeight;

            for (double x = ox + cw; x < ex; x += cw)
            {
                var p0 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }

            for (double y = oy + ch; y < ey; y += ch)
            {
                var p0 = new XPoint(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new XPoint(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
            }
        }

        public void ClearCache()
        {
            _biCache.Reset();
        }

        public void Fill(object dc, double x, double y, double width, double height, BaseColorViewModel colorViewModel)
        {
            var _gfx = dc as XGraphics;
            _gfx.DrawRectangle(
                ToXBrush(colorViewModel),
                _scaleToPage(x),
                _scaleToPage(y),
                _scaleToPage(width),
                _scaleToPage(height));
        }

        public void Grid(object dc, IGrid grid, double x, double y, double width, double height)
        {
            var _gfx = dc as XGraphics;

            var strokeLine = ToXPen(grid.GridStrokeColorViewModel, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi);

            var rect = Spatial.Rect2.FromPoints(
                x + grid.GridOffsetLeft,
                y + grid.GridOffsetTop,
                x + width - grid.GridOffsetLeft + grid.GridOffsetRight,
                y + height - grid.GridOffsetTop + grid.GridOffsetBottom,
                0, 0);

            if (grid.IsGridEnabled)
            {
                DrawGridInternal(
                    _gfx,
                    grid,
                    ToXPen(grid.GridStrokeColorViewModel, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi),
                    ref rect);
            }

            if (grid.IsBorderEnabled)
            {
                _gfx.DrawRectangle(
                    ToXPen(grid.GridStrokeColorViewModel, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        public void DrawPage(object dc, PageContainerViewModel containerViewModel)
        {
            foreach (var layer in containerViewModel.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        public void DrawLayer(object dc, LayerContainerViewModel layer)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(StateViewModel.DrawShapeState))
                {
                    shape.DrawShape(dc, this);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_stateViewModel.DrawShapeState))
                {
                    shape.DrawPoints(dc, this);
                }
            }
        }

        public void DrawPoint(object dc, PointShapeViewModel point)
        {
            // TODO:
        }

        public void DrawLine(object dc, LineShapeViewModel line)
        {
            if (!line.IsStroked)
            {
                return;
            }

            var _gfx = dc as XGraphics;

            var strokeLine = ToXPen(line.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi);
            DrawLineArrowsInternal(_gfx, line, out var pt1, out var pt2);
            DrawLineInternal(_gfx, strokeLine, line.IsStroked, ref pt1, ref pt2);
        }

        public void DrawRectangle(object dc, RectangleShapeViewModel rectangle)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                0, 0);

            if (rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(rectangle.StyleViewModel.Fill.ColorViewModel),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (rectangle.IsStroked && !rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXBrush(rectangle.StyleViewModel.Fill.ColorViewModel),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        public void DrawEllipse(object dc, EllipseShapeViewModel ellipse)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                0, 0);

            if (ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(ellipse.StyleViewModel.Fill.ColorViewModel),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (ellipse.IsStroked && !ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXBrush(ellipse.StyleViewModel.Fill.ColorViewModel),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        public void DrawArc(object dc, ArcShapeViewModelViewModel arc)
        {
            var _gfx = dc as XGraphics;

            var a = new Spatial.Arc.GdiArc(
                Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (arc.IsFilled)
            {
                var path = new XGraphicsPath();
                // NOTE: Not implemented in PdfSharp Core version.
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    a.StartAngle,
                    a.SweepAngle);

                if (arc.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(arc.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(arc.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(arc.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
            }
            else
            {
                if (arc.IsStroked)
                {
                    _gfx.DrawArc(
                        ToXPen(arc.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShapeViewModel cubicBezier)
        {
            var _gfx = dc as XGraphics;

            if (cubicBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));

                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(cubicBezier.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(cubicBezier.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(cubicBezier.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
            }
            else
            {
                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(cubicBezier.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(cubicBezier.Point1.X),
                        _scaleToPage(cubicBezier.Point1.Y),
                        _scaleToPage(cubicBezier.Point2.X),
                        _scaleToPage(cubicBezier.Point2.Y),
                        _scaleToPage(cubicBezier.Point3.X),
                        _scaleToPage(cubicBezier.Point3.Y),
                        _scaleToPage(cubicBezier.Point4.X),
                        _scaleToPage(cubicBezier.Point4.Y));
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShapeViewModel quadraticBezier)
        {
            var _gfx = dc as XGraphics;

            double x1 = quadraticBezier.Point1.X;
            double y1 = quadraticBezier.Point1.Y;
            double x2 = quadraticBezier.Point1.X + (2.0 * (quadraticBezier.Point2.X - quadraticBezier.Point1.X)) / 3.0;
            double y2 = quadraticBezier.Point1.Y + (2.0 * (quadraticBezier.Point2.Y - quadraticBezier.Point1.Y)) / 3.0;
            double x3 = x2 + (quadraticBezier.Point3.X - quadraticBezier.Point1.X) / 3.0;
            double y3 = y2 + (quadraticBezier.Point3.Y - quadraticBezier.Point1.Y) / 3.0;
            double x4 = quadraticBezier.Point3.X;
            double y4 = quadraticBezier.Point3.Y;

            if (quadraticBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));

                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(quadraticBezier.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXBrush(quadraticBezier.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXBrush(quadraticBezier.StyleViewModel.Fill.ColorViewModel),
                        path);
                }
            }
            else
            {
                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(quadraticBezier.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(x1),
                        _scaleToPage(y1),
                        _scaleToPage(x2),
                        _scaleToPage(y2),
                        _scaleToPage(x3),
                        _scaleToPage(y3),
                        _scaleToPage(x4),
                        _scaleToPage(y4));
                }
            }
        }

        public void DrawText(object dc, TextShapeViewModel text)
        {
            var _gfx = dc as XGraphics;

            if (!(text.GetProperty(nameof(TextShapeViewModel.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

            var fontStyle = XFontStyle.Regular;

            if (text.StyleViewModel.TextStyleViewModel.FontStyle.HasFlag(FontStyleFlags.Bold))
            {
                fontStyle |= XFontStyle.Bold;
            }

            if (text.StyleViewModel.TextStyleViewModel.FontStyle.HasFlag(FontStyleFlags.Italic))
            {
                fontStyle |= XFontStyle.Italic;
            }

            var font = new XFont(
                text.StyleViewModel.TextStyleViewModel.FontName,
                _scaleToPage(text.StyleViewModel.TextStyleViewModel.FontSize),
                fontStyle,
                options);

            var rect = Spatial.Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                0, 0);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new XStringFormat();
            switch (text.StyleViewModel.TextStyleViewModel.TextHAlignment)
            {
                case TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;

                case TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;

                case TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.StyleViewModel.TextStyleViewModel.TextVAlignment)
            {
                case TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;

                case TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;

                case TextVAlignment.Bottom:
                    format.LineAlignment = XLineAlignment.Far;
                    break;
            }

            _gfx.DrawString(
                tbind,
                font,
                ToXBrush(text.StyleViewModel.Stroke.ColorViewModel),
                srect,
                format);
        }

        public void DrawImage(object dc, ImageShapeViewModel image)
        {
            var _gfx = dc as XGraphics;

            var rect = Spatial.Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y,
                0, 0);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsStroked || image.IsFilled)
            {
                DrawRectangleInternal(
                    _gfx,
                    ToXBrush(image.StyleViewModel.Fill.ColorViewModel),
                    ToXPen(image.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    image.IsStroked,
                    image.IsFilled,
                    ref srect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                _gfx.DrawImage(imageCached, srect);
            }
            else
            {
                if (StateViewModel.ImageCache != null && !string.IsNullOrEmpty(image.Key))
                {
                    var bytes = StateViewModel.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        var ms = new System.IO.MemoryStream(bytes);

                        var bi = XImage.FromStream(ms);

                        _biCache.Set(image.Key, bi);

                        _gfx.DrawImage(bi, srect);
                    }
                }
            }
        }

        public void DrawPath(object dc, PathShapeViewModel path)
        {
            var _gfx = dc as XGraphics;

            var gp = path.GeometryViewModel.ToXGraphicsPath(_scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXBrush(path.StyleViewModel.Fill.ColorViewModel),
                    gp);
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXBrush(path.StyleViewModel.Fill.ColorViewModel),
                    gp);
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.StyleViewModel, _scaleToPage, _sourceDpi, _targetDpi),
                    gp);
            }
        }
    }
}
