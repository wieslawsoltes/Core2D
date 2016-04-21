// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
#if WPF
using System.Windows.Media.Imaging;
#endif

#if WPF
namespace Renderer.PdfSharp_wpf
#elif CORE
namespace Renderer.PdfSharp_core
#endif
{
    /// <summary>
    /// Native PdfSharp shape renderer.
    /// </summary>
    public class PdfRenderer : Core2D.Renderer.ShapeRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, XImage> _biCache;
        private Func<double, double> _scaleToPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRenderer"/> class.
        /// </summary>
        public PdfRenderer()
        {
            ClearCache(isZooming: false);

            _scaleToPage = (value) => (float)(value * 1.0);
        }

        /// <summary>
        /// Creates a new <see cref="PdfRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="PdfRenderer"/> class.</returns>
        public static Core2D.Renderer.ShapeRenderer Create()
        {
            return new PdfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page"></param>
        public void Save(string path, Core2D.Project.XPage page)
        {
            using (var pdf = new PdfDocument())
            {
                Add(pdf, page);
                pdf.Save(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="document"></param>
        public void Save(string path, Core2D.Project.XDocument document)
        {
            using (var pdf = new PdfDocument())
            {
                var documentOutline = default(PdfOutline);

                foreach (var container in document.Pages)
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
        public void Save(string path, Core2D.Project.XProject project)
        {
            using (var pdf = new PdfDocument())
            {
                var projectOutline = default(PdfOutline);

                foreach (var document in project.Documents)
                {
                    var documentOutline = default(PdfOutline);

                    foreach (var container in document.Pages)
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

        private PdfPage Add(PdfDocument pdf, Core2D.Project.XPage page)
        {
            // Create A3 page size with Landscape orientation.
            PdfPage pdfPage = pdf.AddPage();
            pdfPage.Size = PageSize.A3;
            pdfPage.Orientation = PageOrientation.Landscape;

            using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
            {
                // Calculate x and y page scale factors.
                double scaleX = pdfPage.Width.Value / page.Template.Width;
                double scaleY = pdfPage.Height.Value / page.Template.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Set scaling function.
                _scaleToPage = (value) => value * scale;

                // Draw container template contents to pdf graphics.
                if (page.Template.Background.A > 0)
                {
                    DrawBackgroundInternal(
                        gfx,
                        page.Template.Background,
                        Core2D.Math.Rect2.Create(0, 0, pdfPage.Width.Value / scale, pdfPage.Height.Value / scale));
                }

                // Draw template contents to pdf graphics.
                Draw(gfx, page.Template, page.Data.Properties, page.Data.Record);

                // Draw page contents to pdf graphics.
                Draw(gfx, page, page.Data.Properties, page.Data.Record);
            }

            return pdfPage;
        }

        private static XColor ToXColor(Core2D.Style.ArgbColor color)
        {
            return XColor.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        private static XPen ToXPen(Core2D.Style.BaseStyle style, Func<double, double> scale)
        {
            var pen = new XPen(ToXColor(style.Stroke), XUnit.FromPresentation(style.Thickness));
            switch (style.LineCap)
            {
                case Core2D.Style.LineCap.Flat:
                    pen.LineCap = XLineCap.Flat;
                    break;
                case Core2D.Style.LineCap.Square:
                    pen.LineCap = XLineCap.Square;
                    break;
                case Core2D.Style.LineCap.Round:
                    pen.LineCap = XLineCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                pen.DashPattern = Core2D.Style.ShapeStyle.ConvertDashesToDoubleArray(style.Dashes);
            }
            pen.DashOffset = style.DashOffset;
            return pen;
        }

        private static XSolidBrush ToXSolidBrush(Core2D.Style.ArgbColor color)
        {
            return new XSolidBrush(ToXColor(color));
        }

        private static void DrawLineInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint p0, ref XPoint p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        private static void DrawRectangleInternal(XGraphics gfx, XSolidBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
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

        private static void DrawEllipseInternal(XGraphics gfx, XSolidBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
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

        private void DrawGridInternal(XGraphics gfx, XPen stroke, ref Core2D.Math.Rect2 rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
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

        private void DrawBackgroundInternal(XGraphics gfx, Core2D.Style.ArgbColor color, Core2D.Math.Rect2 rect)
        {
            gfx.DrawRectangle(
                null,
                ToXSolidBrush(color),
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
        }

        /// <inheritdoc/>
        public override void ClearCache(bool isZooming)
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
                _biCache = new Dictionary<string, XImage>();
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XLine line, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            if (!line.IsStroked)
                return;

            var _gfx = dc as XGraphics;

            XPen strokeLine = ToXPen(line.Style, _scaleToPage);

            XSolidBrush fillStartArrow = ToXSolidBrush(line.Style.StartArrowStyle.Fill);
            XPen strokeStartArrow = ToXPen(line.Style.StartArrowStyle, _scaleToPage);

            XSolidBrush fillEndArrow = ToXSolidBrush(line.Style.EndArrowStyle.Fill);
            XPen strokeEndArrow = ToXPen(line.Style.EndArrowStyle, _scaleToPage);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            Core2D.Shapes.XLineExtensions.GetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

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
                case Core2D.Style.ArrowType.None:
                    {
                        pt1 = new XPoint(x1, y1);
                    }
                    break;
                case Core2D.Style.ArrowType.Rectangle:
                    {
                        pt1 = t1.Transform(new XPoint(x1 - sizeX1, y1));
                        var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        _gfx.Save();
                        _gfx.RotateAtTransform(a1, c1);
                        DrawRectangleInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Ellipse:
                    {
                        pt1 = t1.Transform(new XPoint(x1 - sizeX1, y1));
                        _gfx.Save();
                        _gfx.RotateAtTransform(a1, c1);
                        var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Arrow:
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
                case Core2D.Style.ArrowType.None:
                    {
                        pt2 = new XPoint(x2, y2);
                    }
                    break;
                case Core2D.Style.ArrowType.Rectangle:
                    {
                        pt2 = t2.Transform(new XPoint(x2 - sizeX2, y2));
                        var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        _gfx.Save();
                        _gfx.RotateAtTransform(a2, c2);
                        DrawRectangleInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Ellipse:
                    {
                        pt2 = t2.Transform(new XPoint(x2 - sizeX2, y2));
                        _gfx.Save();
                        _gfx.RotateAtTransform(a2, c2);
                        var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Arrow:
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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XRectangle rectangle, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Math.Rect2.Create(
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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XEllipse ellipse, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Math.Rect2.Create(
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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XArc arc, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var a = Core2D.Math.Arc.GdiArc.FromXArc(arc, dx, dy);

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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            if (cubicBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X + dx),
                    _scaleToPage(cubicBezier.Point1.Y + dy),
                    _scaleToPage(cubicBezier.Point2.X + dx),
                    _scaleToPage(cubicBezier.Point2.Y + dy),
                    _scaleToPage(cubicBezier.Point3.X + dx),
                    _scaleToPage(cubicBezier.Point3.Y + dy),
                    _scaleToPage(cubicBezier.Point4.X + dx),
                    _scaleToPage(cubicBezier.Point4.Y + dy));

                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(cubicBezier.Style, _scaleToPage),
                        ToXSolidBrush(cubicBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(cubicBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(cubicBezier.Style, _scaleToPage),
                        _scaleToPage(cubicBezier.Point1.X + dx),
                        _scaleToPage(cubicBezier.Point1.Y + dy),
                        _scaleToPage(cubicBezier.Point2.X + dx),
                        _scaleToPage(cubicBezier.Point2.Y + dy),
                        _scaleToPage(cubicBezier.Point3.X + dx),
                        _scaleToPage(cubicBezier.Point3.Y + dy),
                        _scaleToPage(cubicBezier.Point4.X + dx),
                        _scaleToPage(cubicBezier.Point4.Y + dy));
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
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
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy),
                    _scaleToPage(x2 + dx),
                    _scaleToPage(y2 + dy),
                    _scaleToPage(x3 + dx),
                    _scaleToPage(y3 + dy),
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy));

                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(quadraticBezier.Style, _scaleToPage),
                        ToXSolidBrush(quadraticBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(quadraticBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(quadraticBezier.Style, _scaleToPage),
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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XText text, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

            var fontStyle = XFontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Bold))
                {
                    fontStyle |= XFontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Italic))
                {
                    fontStyle |= XFontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Underline))
                {
                    fontStyle |= XFontStyle.Underline;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Strikeout))
                {
                    fontStyle |= XFontStyle.Strikeout;
                }
            }

            var font = new XFont(
                text.Style.TextStyle.FontName,
                _scaleToPage(text.Style.TextStyle.FontSize),
                fontStyle,
                options);

            var rect = Core2D.Math.Rect2.Create(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new XStringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case Core2D.Style.TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case Core2D.Style.TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case Core2D.Style.TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case Core2D.Style.TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case Core2D.Style.TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case Core2D.Style.TextVAlignment.Bottom:
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

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XImage image, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Math.Rect2.Create(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            var srect = new XRect(
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
                && _biCache.ContainsKey(image.Key))
            {
                _gfx.DrawImage(_biCache[image.Key], srect);
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                    return;

                var bytes = State.ImageCache.GetImage(image.Key);
                if (bytes != null)
                {
                    var ms = new System.IO.MemoryStream(bytes);
#if WPF
                    var bs = new BitmapImage();
                    bs.BeginInit();
                    bs.StreamSource = ms;
                    bs.EndInit();
                    bs.Freeze();
                    var bi = XImage.FromBitmapSource(bs);
#else
                    var bi = XImage.FromStream(ms);
#endif
                    if (_enableImageCache)
                        _biCache[image.Key] = bi;

                    _gfx.DrawImage(bi, srect);

                    if (!_enableImageCache)
                        bi.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XPath path, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

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
