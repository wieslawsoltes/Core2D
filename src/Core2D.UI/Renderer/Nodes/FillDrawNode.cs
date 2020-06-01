using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class FillDrawNode : DrawNode
    {
        public A.Rect Rect { get; set; }
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
            Rect = new A.Rect(X, Y, Width, Height);
            Center = Rect.Center;
        }

        public override void UpdateStyle()
        {
            Fill = DrawUtil.ToBrush(Color);
        }

        public override void Draw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            OnDraw(context, dx, dy, zoom);
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.FillRectangle(Fill, Rect);
        }
    }
}
