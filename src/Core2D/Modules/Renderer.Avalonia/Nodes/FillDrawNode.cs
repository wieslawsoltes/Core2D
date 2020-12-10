using Core2D.Renderer;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Renderer
{
    internal class FillDrawNode : DrawNode, IFillDrawNode
    {
        public A.Rect Rect { get; set; }
        public BaseColorViewModel ColorViewModel { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FillDrawNode(double x, double y, double width, double height, BaseColorViewModel colorViewModel)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = colorViewModel;
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
            Fill = AvaloniaDrawUtil.ToBrush(ColorViewModel);
        }

        public override void Draw(object dc, double zoom)
        {
            OnDraw(dc, zoom);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.FillRectangle(Fill, Rect);
        }
    }
}
