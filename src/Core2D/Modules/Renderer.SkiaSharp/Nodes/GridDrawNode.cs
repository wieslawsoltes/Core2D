using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class GridDrawNode : DrawNode, IGridDrawNode
    {
        public SKRect Rect { get; set; }
        public IGrid Grid { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public GridDrawNode(IGrid grid, double x, double y, double width, double height)
        {
            Grid = grid;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = false;
            ScaleSize = false;
            Rect = SKRect.Create(
                (float)(X + Grid.GridOffsetLeft),
                (float)(Y + Grid.GridOffsetTop),
                (float)(Width - Grid.GridOffsetLeft + Grid.GridOffsetRight),
                (float)(Height - Grid.GridOffsetTop + Grid.GridOffsetBottom));
            Center = new SKPoint(Rect.MidX, Rect.MidY);
        }

        public override void UpdateStyle()
        {
            if (Grid.GridStrokeColor != null)
            {
                Stroke = SkiaSharpDrawUtil.ToSKPaintPen(Grid.GridStrokeColor, Grid.GridStrokeThickness);
            }
            else
            {
                Stroke = null;
            }
        }

        public override void Draw(object dc, double zoom)
        {
            OnDraw(dc, zoom);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Grid.GridStrokeColor != null)
            {
                if (Grid.IsGridEnabled)
                {
                    float ox = Rect.Left;
                    float ex = Rect.Left + Rect.Width;
                    float oy = Rect.Top;
                    float ey = Rect.Top + Rect.Height;
                    float cw = (float)Grid.GridCellWidth;
                    float ch = (float)Grid.GridCellHeight;

                    for (float x = ox; x < ex; x += cw)
                    {
                        var p0 = new SKPoint(x, oy);
                        var p1 = new SKPoint(x, ey);
                        canvas.DrawLine(p0, p1, Stroke);
                    }

                    for (float y = oy; y < ey; y += ch)
                    {
                        var p0 = new SKPoint(ox, y);
                        var p1 = new SKPoint(ex, y);
                        canvas.DrawLine(p0, p1, Stroke);
                    }
                }

                if (Grid.IsBorderEnabled)
                {
                    canvas.DrawRect(Rect, Stroke);
                }
            }
        }
    }
}
