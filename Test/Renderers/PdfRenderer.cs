using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core = Test.Core;

namespace Test
{
    internal struct PdfArc
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double StartAngle;
        public double SweepAngle;

        public static PdfArc FromXArc(Test.Core.XArc arc, double dx, double dy)
        {
            double x1 = arc.Point1.X + dx;
            double y1 = arc.Point1.Y + dy;
            double x2 = arc.Point2.X + dx;
            double y2 = arc.Point2.Y + dy;

            double x0 = (x1 + x2) / 2.0;
            double y0 = (y1 + y2) / 2.0;

            double r = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            double startAngle = 180.0 / Math.PI * Math.Atan2(y1 - y0, x1 - x0);
            double endAngle = 180.0 / Math.PI * Math.Atan2(y2 - y0, x2 - x0);
            double sweepAngle = Math.Abs(startAngle) + Math.Abs(endAngle);

            return new PdfArc
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                StartAngle = startAngle,
                SweepAngle = sweepAngle
            };
        }
    }

    public class PdfRenderer : Core.IRenderer
    {
        public bool DrawPoints { get; set; }

        private Func<double, double> ScaleToPage;

        public void Create(string path, Core.IContainer container)
        {
            using (var doc = new PdfDocument())
            {
                Add(doc, container);
                doc.Save(path);
            }
        }

        public void Create(string path, IEnumerable<Core.IContainer> containers)
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

        private void Add(PdfDocument doc, Core.IContainer container)
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
                ScaleToPage = (value) => value * scale;

                // draw block contents to pdf graphics
                Render(gfx, container);
            }
        }

        private void Render(object gfx, Core.IContainer container)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Render(gfx, layer);
                }
            }
        }

        public void ClearCache()
        {
        }

        private XColor ToXColor(Core.ArgbColor color)
        {
            return XColor.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        private XPen ToXPen(Core.ShapeStyle style)
        {
            return new XPen(
                ToXColor(style.Stroke),
                ScaleToPage(style.Thickness))
            {
                LineCap = XLineCap.Flat
            };
        }

        private XSolidBrush ToXSolidBrush(Core.ArgbColor color)
        {
            return new XSolidBrush(ToXColor(color));
        }

        private System.Windows.Rect CreateRect(Core.XPoint tl, Core.XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new System.Windows.Rect(
                new System.Windows.Point(tlx + dx, tly + dy),
                new System.Windows.Point(brx + dx, bry + dy));
        }

        public void Render(object gfx, Core.ILayer layer)
        {
            foreach (var shape in layer.Shapes)
            {
                shape.Draw(gfx, this, 0, 0);
            }
        }

        public void Draw(object gfx, Core.XLine line, double dx, double dy)
        {
            (gfx as XGraphics).DrawLine(
                ToXPen(line.Style),
                ScaleToPage(line.Start.X + dx),
                ScaleToPage(line.Start.Y + dy),
                ScaleToPage(line.End.X + dx),
                ScaleToPage(line.End.Y + dy));
        }

        public void Draw(object gfx, Core.XRectangle rectangle, double dx, double dy)
        {
            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            if (rectangle.IsFilled)
            {
                (gfx as XGraphics).DrawRectangle(
                    ToXPen(rectangle.Style),
                    ToXSolidBrush(rectangle.Style.Fill),
                    ScaleToPage(rect.X),
                    ScaleToPage(rect.Y),
                    ScaleToPage(rect.Width),
                    ScaleToPage(rect.Height));
            }
            else
            {
                (gfx as XGraphics).DrawRectangle(
                    ToXPen(rectangle.Style),
                    ScaleToPage(rect.X),
                    ScaleToPage(rect.Y),
                    ScaleToPage(rect.Width),
                    ScaleToPage(rect.Height));
            }
        }

        public void Draw(object gfx, Core.XEllipse ellipse, double dx, double dy)
        {
            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

            if (ellipse.IsFilled)
            {
                (gfx as XGraphics).DrawEllipse(
                    ToXPen(ellipse.Style),
                    ToXSolidBrush(ellipse.Style.Fill),
                    ScaleToPage(rect.X),
                    ScaleToPage(rect.Y),
                    ScaleToPage(rect.Width),
                    ScaleToPage(rect.Height));
            }
            else
            {
                (gfx as XGraphics).DrawEllipse(
                    ToXPen(ellipse.Style),
                    ScaleToPage(rect.X),
                    ScaleToPage(rect.Y),
                    ScaleToPage(rect.Width),
                    ScaleToPage(rect.Height));
            }
        }

        public void Draw(object gfx, Core.XArc arc, double dx, double dy)
        {
            var a = PdfArc.FromXArc(arc, dx, dy);

            (gfx as XGraphics).DrawArc(
                ToXPen(arc.Style),
                ScaleToPage(a.X),
                ScaleToPage(a.Y),
                ScaleToPage(a.Width),
                ScaleToPage(a.Height),
                a.StartAngle,
                a.SweepAngle);
        }

        public void Draw(object gfx, Core.XBezier bezier, double dx, double dy)
        {
            (gfx as XGraphics).DrawBezier(
                ToXPen(bezier.Style),
                ScaleToPage(bezier.Point1.X),
                ScaleToPage(bezier.Point1.Y),
                ScaleToPage(bezier.Point2.X),
                ScaleToPage(bezier.Point2.Y),
                ScaleToPage(bezier.Point3.X),
                ScaleToPage(bezier.Point3.Y),
                ScaleToPage(bezier.Point4.X),
                ScaleToPage(bezier.Point4.Y));
        }

        public void Draw(object gfx, Core.XQBezier qbezier, double dx, double dy)
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
                ToXPen(qbezier.Style),
                ScaleToPage(x1 + dx), ScaleToPage(y1 + dy),
                ScaleToPage(x2 + dx), ScaleToPage(y2 + dy),
                ScaleToPage(x3 + dx), ScaleToPage(y3 + dy),
                ScaleToPage(x4 + dx), ScaleToPage(y4 + dy));
        }

        public void Draw(object gfx, Core.XText text, double dx, double dy)
        {
            XPdfFontOptions options = new XPdfFontOptions(
                PdfFontEncoding.Unicode,
                PdfFontEmbedding.Always);

            XFont font = new XFont(
                text.Style.FontName,
                ScaleToPage(text.Style.FontSize),
                XFontStyle.Regular,
                options);

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            XRect srect = new XRect(
                ScaleToPage(rect.X),
                ScaleToPage(rect.Y),
                ScaleToPage(rect.Width),
                ScaleToPage(rect.Height));

            XStringFormat format = new XStringFormat();
            switch (text.Style.TextHAlignment)
            {
                case Core.TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case Core.TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case Core.TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextVAlignment)
            {
                case Core.TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case Core.TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case Core.TextVAlignment.Bottom:
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
