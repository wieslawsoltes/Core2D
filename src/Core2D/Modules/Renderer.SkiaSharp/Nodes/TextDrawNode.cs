using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class TextDrawNode : DrawNode, ITextDrawNode
    {
        public TextShapeViewModel Text { get; set; }
        public SKRect Rect { get; set; }
        public SKPoint Origin { get; set; }
        public SKTypeface Typeface { get; set; }
        public SKPaint FormattedText { get; set; }
        public string BoundText { get; set; }

        protected TextDrawNode()
        {
        }

        public TextDrawNode(TextShapeViewModel text, ShapeStyleViewModel style)
        {
            Style = style;
            Text = text;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Text.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Text.State.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Text.TopLeft.X, Text.TopLeft.Y, Text.BottomRight.X, Text.BottomRight.Y, 0, 0);
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);

            UpdateTextGeometry();
        }

        protected void UpdateTextGeometry()
        {
            BoundText = Text.GetProperty(nameof(TextShapeViewModel.Text)) is string boundText ? boundText : Text.Text;

            if (BoundText == null)
            {
                return;
            }

            if (Style.TextStyle.FontSize < 0.0)
            {
                return;
            }

            FormattedText = SkiaSharpDrawUtil.GetSKPaint(BoundText, Style, Text.TopLeft, Text.BottomRight, out var origin);

            Origin = origin;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (FormattedText != null)
            {
                canvas.DrawText(BoundText, Origin.X, Origin.Y, FormattedText);
            }
        }
    }
}
