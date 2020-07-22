using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class FillDrawNode : DrawNode, IFillDrawNode
    {
        public SKRect Rect { get; set; }
        public IColor Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FillDrawNode(double x, double y, double width, double height, IColor color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = false;
            ScaleSize = false;
            Rect = SKRect.Create((float)X, (float)Y, (float)Width, (float)Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
        }

        public override void UpdateStyle()
        {
            Fill = SkiaSharpDrawUtil.ToSKPaintBrush(Color);
        }

        public override void Draw(object dc, double zoom)
        {
            OnDraw(dc, zoom);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            canvas.DrawRect(Rect, Fill);
        }
    }
}
