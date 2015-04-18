// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class PdfRenderer : Test2d.IRenderer
    {
        public bool DrawPoints { get; set; }
        public double Zoom { get; set; }

        private Func<double, double> _scaleToPage;

        public void Save(string path, Test2d.Container container)
        {
            using (var doc = new PdfDocument())
            {
                Add(doc, container);
                doc.Save(path);
            }
        }

        public void Save(string path, IEnumerable<Test2d.Container> containers)
        {
            using (var doc = new PdfDocument())
            {
                foreach (var page in containers)
                {
                    Add(doc, page);
                }
                doc.Save(path);
            }
        }

        private void Add(PdfDocument doc, Test2d.Container container)
        {
            // create A4 page with landscape orientation
            PdfPage page = doc.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                // calculate x and y page scale factors
                double scaleX = page.Width.Value / container.Width;
                double scaleY = page.Height.Value / container.Height;
                double scale = Math.Min(scaleX, scaleY);

                // set scaling function
                _scaleToPage = (value) => value * scale;

                // draw block contents to pdf graphics
                Draw(gfx, container);
            }
        }

        private static XColor ToXColor(Test2d.ArgbColor color)
        {
            return XColor.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        private static XPen ToXPen(Test2d.ShapeStyle style, Func<double, double> scale)
        {
            return new XPen(
                ToXColor(style.Stroke),
                scale(style.Thickness))
            {
                LineCap = XLineCap.Flat
            };
        }

        private static XSolidBrush ToXSolidBrush(Test2d.ArgbColor color)
        {
            return new XSolidBrush(ToXColor(color));
        }

        private static System.Windows.Rect CreateRect(Test2d.XPoint tl, Test2d.XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new System.Windows.Rect(
                new System.Windows.Point(tlx + dx, tly + dy),
                new System.Windows.Point(brx + dx, bry + dy));
        }

        public void ClearCache()
        {
        }

        public void Draw(object gfx, Test2d.Container container)
        {
            if (container.TemplateLayer.IsVisible)
            {
                Draw(gfx, container.TemplateLayer);
            }

            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(gfx, layer);
                }
            }
        }

        public void Draw(object gfx, Test2d.Layer layer)
        {
            foreach (var shape in layer.Shapes)
            {
                shape.Draw(gfx, this, 0, 0);
            }
        }

        public void Draw(object gfx, Test2d.XLine line, double dx, double dy)
        {
            (gfx as XGraphics).DrawLine(
                ToXPen(line.Style, _scaleToPage),
                _scaleToPage(line.Start.X + dx),
                _scaleToPage(line.Start.Y + dy),
                _scaleToPage(line.End.X + dx),
                _scaleToPage(line.End.Y + dy));
        }

        public void Draw(object gfx, Test2d.XRectangle rectangle, double dx, double dy)
        {
            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            if (rectangle.IsFilled)
            {
                (gfx as XGraphics).DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage),
                    ToXSolidBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else
            {
                (gfx as XGraphics).DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        public void Draw(object gfx, Test2d.XEllipse ellipse, double dx, double dy)
        {
            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

            if (ellipse.IsFilled)
            {
                (gfx as XGraphics).DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage),
                    ToXSolidBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else
            {
                (gfx as XGraphics).DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        public void Draw(object gfx, Test2d.XArc arc, double dx, double dy)
        {
            var a = PdfArc.FromXArc(arc, dx, dy);

            (gfx as XGraphics).DrawArc(
                ToXPen(arc.Style, _scaleToPage),
                _scaleToPage(a.X),
                _scaleToPage(a.Y),
                _scaleToPage(a.Width),
                _scaleToPage(a.Height),
                a.StartAngle,
                a.SweepAngle);
        }

        public void Draw(object gfx, Test2d.XBezier bezier, double dx, double dy)
        {
            (gfx as XGraphics).DrawBezier(
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

        public void Draw(object gfx, Test2d.XQBezier qbezier, double dx, double dy)
        {
            double x1 = qbezier.Point1.X;
            double y1 = qbezier.Point1.Y;
            double x2 = qbezier.Point1.X + (2.0 * (qbezier.Point2.X - qbezier.Point1.X)) / 3.0;
            double y2 = qbezier.Point1.Y + (2.0 * (qbezier.Point2.Y - qbezier.Point1.Y)) / 3.0;
            double x3 = x2 + (qbezier.Point3.X - qbezier.Point1.X) / 3.0;
            double y3 = y2 + (qbezier.Point3.Y - qbezier.Point1.Y) / 3.0;
            double x4 = qbezier.Point3.X;
            double y4 = qbezier.Point3.Y;

            (gfx as XGraphics).DrawBezier(
                ToXPen(qbezier.Style, _scaleToPage),
                _scaleToPage(x1 + dx), _scaleToPage(y1 + dy),
                _scaleToPage(x2 + dx), _scaleToPage(y2 + dy),
                _scaleToPage(x3 + dx), _scaleToPage(y3 + dy),
                _scaleToPage(x4 + dx), _scaleToPage(y4 + dy));
        }

        public void Draw(object gfx, Test2d.XText text, double dx, double dy)
        {
            XPdfFontOptions options = new XPdfFontOptions(
                PdfFontEncoding.Unicode,
                PdfFontEmbedding.Always);

            XFont font = new XFont(
                text.Style.FontName,
                _scaleToPage(text.Style.FontSize),
                XFontStyle.Regular,
                options);

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            XRect srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            XStringFormat format = new XStringFormat();
            switch (text.Style.TextHAlignment)
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

            switch (text.Style.TextVAlignment)
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

            if (text.IsFilled)
            {
                (gfx as XGraphics).DrawRectangle(ToXSolidBrush(text.Style.Fill), srect);
            }

            (gfx as XGraphics).DrawString(
                text.Text,
                font,
                ToXSolidBrush(text.Style.Stroke),
                srect,
                format);
        }
    }
}
