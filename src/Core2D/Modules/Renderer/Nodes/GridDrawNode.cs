using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class GridDrawNode : DrawNode, IGridDrawNode
    {
        public A.Rect Rect { get; set; }
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
            ScaleThickness = true;
            ScaleSize = false;
            Rect = new A.Rect(
                X + Grid.GridOffsetLeft,
                Y + Grid.GridOffsetTop,
                Width - Grid.GridOffsetLeft + Grid.GridOffsetRight,
                Height - Grid.GridOffsetTop + Grid.GridOffsetBottom);
            Center = Rect.Center;
        }

        public override void UpdateStyle()
        {
            if (Grid.GridStrokeColor != null)
            {
                Stroke = AvaloniaDrawUtil.ToPen(Grid.GridStrokeColor, Grid.GridStrokeThickness);
            }
            else
            {
                Stroke = null;
            }
        }

        public override void Draw(object dc, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;

            double thickness = Grid.GridStrokeThickness;

            if (ScaleThickness)
            {
                thickness /= zoom;
            }

            if (scale != 1.0)
            {
                thickness /= scale;
            }

            if (Stroke != null && Stroke.Thickness != thickness)
            {
                Stroke = AvaloniaDrawUtil.ToPen(Grid.GridStrokeColor, thickness);
            }

            OnDraw(dc, zoom);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            if (Grid.GridStrokeColor != null)
            {
                if (Grid.IsGridEnabled)
                {
                    double ox = Rect.X;
                    double ex = Rect.X + Rect.Width;
                    double oy = Rect.Y;
                    double ey = Rect.Y + Rect.Height;
                    double cw = Grid.GridCellWidth;
                    double ch = Grid.GridCellHeight;

                    for (double x = ox + cw; x < ex; x += cw)
                    {
                        var p0 = new A.Point(x, oy);
                        var p1 = new A.Point(x, ey);
                        context.DrawLine(Stroke, p0, p1);
                    }

                    for (double y = oy + ch; y < ey; y += ch)
                    {
                        var p0 = new A.Point(ox, y);
                        var p1 = new A.Point(ex, y);
                        context.DrawLine(Stroke, p0, p1);
                    }
                }

                if (Grid.IsBorderEnabled)
                {
                    context.DrawRectangle(Stroke, Rect);
                }
            }
        }
    }
}
