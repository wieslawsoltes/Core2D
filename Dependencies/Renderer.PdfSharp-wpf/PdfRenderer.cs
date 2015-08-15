// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfRenderer : Test2d.ObservableObject, Test2d.IRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<Uri, XImage> _biCache;
        private Test2d.RendererState _state = new Test2d.RendererState();

        /// <summary>
        /// 
        /// </summary>
        public Test2d.RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PdfRenderer()
        {
            ClearCache(isZooming: false);

            _scaleToPage = (value) => (float)(value * 1.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Test2d.IRenderer Create()
        {
            return new PdfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        private Func<double, double> _scaleToPage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public void Save(string path, Test2d.Container container)
        {
            using (var pdf = new PdfDocument())
            {
                Add(pdf, container);
                pdf.Save(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="document"></param>
        public void Save(string path, Test2d.Document document)
        {
            using (var pdf = new PdfDocument())
            {
                PdfOutline documentOutline = default(PdfOutline);

                foreach (var container in document.Containers)
                {
                    var page = Add(pdf, container);

                    if (documentOutline == null)
                    {
                        documentOutline = pdf.Outlines.Add(
                            document.Name,
                            page,
                            true,
                            PdfOutlineStyle.Regular,
                            XColors.Black);
                    }

                    documentOutline.Outlines.Add(
                        container.Name,
                        page,
                        true,
                        PdfOutlineStyle.Regular,
                        XColors.Black);
                }

                pdf.Save(path);
                ClearCache(isZooming: false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="project"></param>
        public void Save(string path, Test2d.Project project)
        {
            using (var pdf = new PdfDocument())
            {
                PdfOutline projectOutline = default(PdfOutline);

                foreach (var document in project.Documents)
                {
                    PdfOutline documentOutline = default(PdfOutline);

                    foreach (var container in document.Containers)
                    {
                        var page = Add(pdf, container);

                        if (projectOutline == null)
                        {
                            projectOutline = pdf.Outlines.Add(
                                project.Name,
                                page,
                                true,
                                PdfOutlineStyle.Regular,
                                XColors.Black);
                        }

                        if (documentOutline == null)
                        {
                            documentOutline = projectOutline.Outlines.Add(
                                document.Name,
                                page,
                                true,
                                PdfOutlineStyle.Regular,
                                XColors.Black);
                        }

                        documentOutline.Outlines.Add(
                            container.Name,
                            page,
                            true,
                            PdfOutlineStyle.Regular,
                            XColors.Black);
                    }
                }

                pdf.Save(path);
                ClearCache(isZooming: false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private PdfPage Add(PdfDocument pdf, Test2d.Container container)
        {
            // create A4 page with landscape orientation
            PdfPage page = pdf.AddPage();
            page.Size = PageSize.A3;
            page.Orientation = PageOrientation.Landscape;

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                // calculate x and y page scale factors
                double scaleX = page.Width.Value / container.Width;
                double scaleY = page.Height.Value / container.Height;
                double scale = Math.Min(scaleX, scaleY);

                // set scaling function
                _scaleToPage = (value) => value * scale;

                // draw container template contents to pdf graphics
                if (container.Template != null)
                {
                    if (container.Template.Background.A > 0)
                    {
                        DrawBackgroundInternal(
                            gfx,
                            container.Template.Background,
                            Test2d.Rect2.Create(0, 0, page.Width.Value / scale, page.Height.Value / scale));
                    }
                    Draw(gfx, container.Template, container.Properties, null);
                }
                
                // draw container contents to pdf graphics
                if (container.Background.A > 0)
                {
                    DrawBackgroundInternal(
                        gfx,
                        container.Background,
                        Test2d.Rect2.Create(0, 0, page.Width.Value / scale, page.Height.Value / scale));
                }
                Draw(gfx, container, container.Properties, null);
            }

            return page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static XColor ToXColor(Test2d.ArgbColor color)
        {
            return XColor.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static XPen ToXPen(Test2d.BaseStyle style, Func<double, double> scale)
        {
            var pen = new XPen(ToXColor(style.Stroke), XUnit.FromPresentation(style.Thickness));
            switch (style.LineCap)
            {
                case Test2d.LineCap.Flat:
                    pen.LineCap = XLineCap.Flat;
                    break;
                case Test2d.LineCap.Square:
                    pen.LineCap = XLineCap.Square;
                    break;
                case Test2d.LineCap.Round:
                    pen.LineCap = XLineCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                pen.DashPattern = style.Dashes;
            }
            pen.DashOffset = style.DashOffset;
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static XSolidBrush ToXSolidBrush(Test2d.ArgbColor color)
        {
            return new XSolidBrush(ToXColor(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            XGraphics gfx,
            XPen pen,
            bool isStroked,
            ref XPoint p0,
            ref XPoint p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            XGraphics gfx,
            XSolidBrush brush,
            XPen pen,
            bool isStroked,
            bool isFilled,
            ref XRect rect)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawEllipseInternal(
            XGraphics gfx,
            XSolidBrush brush,
            XPen pen,
            bool isStroked,
            bool isFilled,
            ref XRect rect)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="stroke"></param>
        /// <param name="rect"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="isStroked"></param>
        private void DrawGridInternal(
            XGraphics gfx,
            XPen stroke,
            ref Test2d.Rect2 rect,
            double offsetX, double offsetY,
            double cellWidth, double cellHeight,
            bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new XPoint(
                    _scaleToPage(x), 
                    _scaleToPage(oy));
                var p1 = new XPoint(
                    _scaleToPage(x), 
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new XPoint(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new XPoint(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="color"></param>
        /// <param name="rect"></param>
        private void DrawBackgroundInternal(XGraphics gfx, Test2d.ArgbColor color, Test2d.Rect2 rect)
        {
            gfx.DrawRectangle(
                null,
                ToXSolidBrush(color),
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                if (_biCache != null)
                {
                    foreach (var kvp in _biCache)
                    {
                        kvp.Value.Dispose();
                    }
                    _biCache.Clear();
                }
                _biCache = new Dictionary<Uri, XImage>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.Container container, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(gfx, layer, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.Layer layer, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    shape.Draw(gfx, this, 0, 0, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XLine line, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            XSolidBrush fillLine = ToXSolidBrush(line.Style.Fill);
            XPen strokeLine = ToXPen(line.Style, _scaleToPage);

            XSolidBrush fillStartArrow = ToXSolidBrush(line.Style.StartArrowStyle.Fill);
            XPen strokeStartArrow = ToXPen(line.Style.StartArrowStyle, _scaleToPage);

            XSolidBrush fillEndArrow = ToXSolidBrush(line.Style.EndArrowStyle.Fill);
            XPen strokeEndArrow = ToXPen(line.Style.EndArrowStyle, _scaleToPage);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            Test2d.XLine.SetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            double x1 = _scaleToPage(_x1);
            double y1 = _scaleToPage(_y1);
            double x2 = _scaleToPage(_x2);
            double y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            var t1 = new XMatrix();
            var c1 = new XPoint(x1, y1);
            t1.RotateAtPrepend(a1, c1);

            var t2 = new XMatrix();
            var c2 = new XPoint(x2, y2);
            t2.RotateAtPrepend(a2, c2);

            XPoint pt1;
            XPoint pt2;

            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType)
            {
                default:
                case Test2d.ArrowType.None:
                    {
                        pt1 = new XPoint(x1, y1);
                    }
                    break;
                case Test2d.ArrowType.Rectangle:
                    {
                        pt1 = t1.Transform(new XPoint(x1 - sizeX1, y1));
                        var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        _gfx.Save();
                        _gfx.RotateAtTransform(a1, c1);
                        DrawRectangleInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Test2d.ArrowType.Ellipse:
                    {
                        pt1 = t1.Transform(new XPoint(x1 - sizeX1, y1));
                        _gfx.Save();
                        _gfx.RotateAtTransform(a1, c1);
                        var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Test2d.ArrowType.Arrow:
                    {
                        pt1 = t1.Transform(new XPoint(x1, y1));
                        var p11 = t1.Transform(new XPoint(x1 - sizeX1, y1 + sizeY1));
                        var p21 = t1.Transform(new XPoint(x1, y1));
                        var p12 = t1.Transform(new XPoint(x1 - sizeX1, y1 - sizeY1));
                        var p22 = t1.Transform(new XPoint(x1, y1));
                        DrawLineInternal(_gfx, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(_gfx, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            double radiusX2 = eas.RadiusX;
            double radiusY2 = eas.RadiusY;
            double sizeX2 = 2.0 * radiusX2;
            double sizeY2 = 2.0 * radiusY2;

            switch (eas.ArrowType)
            {
                default:
                case Test2d.ArrowType.None:
                    {
                        pt2 = new XPoint(x2, y2);
                    }
                    break;
                case Test2d.ArrowType.Rectangle:
                    {
                        pt2 = t2.Transform(new XPoint(x2 - sizeX2, y2));
                        var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        _gfx.Save();
                        _gfx.RotateAtTransform(a2, c2);
                        DrawRectangleInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Test2d.ArrowType.Ellipse:
                    {
                        pt2 = t2.Transform(new XPoint(x2 - sizeX2, y2));
                        _gfx.Save();
                        _gfx.RotateAtTransform(a2, c2);
                        var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Test2d.ArrowType.Arrow:
                    {
                        pt2 = t2.Transform(new XPoint(x2, y2));
                        var p11 = t2.Transform(new XPoint(x2 - sizeX2, y2 + sizeY2));
                        var p21 = t2.Transform(new XPoint(x2, y2));
                        var p12 = t2.Transform(new XPoint(x2 - sizeX2, y2 - sizeY2));
                        var p22 = t2.Transform(new XPoint(x2, y2));
                        DrawLineInternal(_gfx, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(_gfx, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            _gfx.DrawLine(strokeLine, pt1, pt2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XRectangle rectangle, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var rect = Test2d.Rect2.Create(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            if (rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage),
                    ToXSolidBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (rectangle.IsStroked && !rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXSolidBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _gfx,
                    ToXPen(rectangle.Style, _scaleToPage),
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XEllipse ellipse, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var rect = Test2d.Rect2.Create(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

            if (ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage),
                    ToXSolidBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (ellipse.IsStroked && !ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXSolidBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XArc arc, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var a = Test2d.GdiArc.FromXArc(arc, dx, dy);

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
                        ToXPen(arc.Style, _scaleToPage),
                        ToXSolidBrush(arc.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(arc.Style.Fill),
                        path);
                }
            }
            else
            {
                if (arc.IsStroked)
                {
                    _gfx.DrawArc(
                        ToXPen(arc.Style, _scaleToPage),
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XBezier bezier, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            if (bezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(bezier.Point1.X),
                    _scaleToPage(bezier.Point1.Y),
                    _scaleToPage(bezier.Point2.X), 
                    _scaleToPage(bezier.Point2.Y),
                    _scaleToPage(bezier.Point3.X), 
                    _scaleToPage(bezier.Point3.Y),
                    _scaleToPage(bezier.Point4.X),
                    _scaleToPage(bezier.Point4.Y));

                if (bezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(bezier.Style, _scaleToPage),
                        ToXSolidBrush(bezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(bezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (bezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(bezier.Style, _scaleToPage),
                        _scaleToPage(bezier.Point1.X),
                        _scaleToPage(bezier.Point1.Y),
                        _scaleToPage(bezier.Point2.X),
                        _scaleToPage(bezier.Point2.Y),
                        _scaleToPage(bezier.Point3.X),
                        _scaleToPage(bezier.Point3.Y),
                        _scaleToPage(bezier.Point4.X),
                        _scaleToPage(bezier.Point4.Y));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XQBezier qbezier, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            double x1 = qbezier.Point1.X;
            double y1 = qbezier.Point1.Y;
            double x2 = qbezier.Point1.X + (2.0 * (qbezier.Point2.X - qbezier.Point1.X)) / 3.0;
            double y2 = qbezier.Point1.Y + (2.0 * (qbezier.Point2.Y - qbezier.Point1.Y)) / 3.0;
            double x3 = x2 + (qbezier.Point3.X - qbezier.Point1.X) / 3.0;
            double y3 = y2 + (qbezier.Point3.Y - qbezier.Point1.Y) / 3.0;
            double x4 = qbezier.Point3.X;
            double y4 = qbezier.Point3.Y;

            if (qbezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy),
                    _scaleToPage(x2 + dx), 
                    _scaleToPage(y2 + dy),
                    _scaleToPage(x3 + dx), 
                    _scaleToPage(y3 + dy),
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy));

                if (qbezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(qbezier.Style, _scaleToPage),
                        ToXSolidBrush(qbezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(qbezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (qbezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(qbezier.Style, _scaleToPage),
                        _scaleToPage(x1 + dx),
                        _scaleToPage(y1 + dy),
                        _scaleToPage(x2 + dx),
                        _scaleToPage(y2 + dy),
                        _scaleToPage(x3 + dx),
                        _scaleToPage(y3 + dy),
                        _scaleToPage(x4 + dx),
                        _scaleToPage(y4 + dy));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XText text, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var tbind = text.BindToTextProperty(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            XPdfFontOptions options = new XPdfFontOptions(
                PdfFontEncoding.Unicode,
                PdfFontEmbedding.Always);

            var fontStyle = XFontStyle.Regular;
            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Bold))
            {
                fontStyle |= XFontStyle.Bold;
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Italic))
            {
                fontStyle |= XFontStyle.Italic;
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline))
            {
                fontStyle |= XFontStyle.Underline;
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
            {
                fontStyle |= XFontStyle.Strikeout;
            }

            XFont font = new XFont(
                text.Style.TextStyle.FontName,
                _scaleToPage(text.Style.TextStyle.FontSize),
                fontStyle,
                options);

            var rect = Test2d.Rect2.Create(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            XRect srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            XStringFormat format = new XStringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case Test2d.TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case Test2d.TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case Test2d.TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case Test2d.TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case Test2d.TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case Test2d.TextVAlignment.Bottom:
                    format.LineAlignment = XLineAlignment.Far;
                    break;
            }

            _gfx.DrawString(
                tbind,
                font,
                ToXSolidBrush(text.Style.Stroke),
                srect,
                format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XImage image, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var rect = Test2d.Rect2.Create(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            XRect srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsStroked && image.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(image.Style, _scaleToPage),
                    ToXSolidBrush(image.Style.Fill),
                    srect);
            }
            else if (image.IsStroked && !image.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(image.Style, _scaleToPage),
                    srect);
            }
            else if (!image.IsStroked && image.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXSolidBrush(image.Style.Fill),
                    srect);
            }

            if (_enableImageCache
                && _biCache.ContainsKey(image.Path))
            {
                _gfx.DrawImage(_biCache[image.Path], srect);
            }
            else
            {
                if (!image.Path.IsAbsoluteUri || !System.IO.File.Exists(image.Path.LocalPath))
                    return;

                var bi = XImage.FromFile(image.Path.LocalPath);

                if (_enableImageCache)
                    _biCache[image.Path] = bi;

                _gfx.DrawImage(bi, srect);

                if (!_enableImageCache)
                    bi.Dispose();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object gfx, Test2d.XPath path, double dx, double dy, ImmutableArray<Test2d.ShapeProperty> db, Test2d.Record r)
        {
            var _gfx = gfx as XGraphics;

            var gp = path.Geometry.ToXGraphicsPath(dx, dy, _scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage),
                    ToXSolidBrush(path.Style.Fill),
                    gp);
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXSolidBrush(path.Style.Fill),
                    gp);
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage),
                    gp);
            }
        }
    }
}
