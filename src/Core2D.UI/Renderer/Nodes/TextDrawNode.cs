using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class TextDrawNode : DrawNode, ITextDrawNode
    {
        public ITextShape Text { get; set; }
        public A.Rect Rect { get; set; }
        public A.Point Origin { get; set; }
        public AM.Typeface Typeface { get; set; }
        public AM.FormattedText FormattedText { get; set; }
        public string BoundText { get; set; }

        protected TextDrawNode()
        {
        }

        public TextDrawNode(ITextShape text, IShapeStyle style)
        {
            Style = style;
            Text = text;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Text.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Text.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Text.TopLeft.X, Text.TopLeft.Y, Text.BottomRight.X, Text.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;

            UpdateTextGeometry();
        }

        protected void UpdateTextGeometry()
        {
            BoundText = Text.GetProperty(nameof(ITextShape.Text)) is string boundText ? boundText : Text.Text;

            if (BoundText == null)
            {
                return;
            }

            if (Style.TextStyle.FontSize < 0.0)
            {
                return;
            }

            var fontStyle = AM.FontStyle.Normal;
            var fontWeight = AM.FontWeight.Normal;

            if (Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
            {
                fontStyle |= AM.FontStyle.Italic;
            }

            if (Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
            {
                fontWeight |= AM.FontWeight.Bold;
            }

            // TODO: Cache Typeface
            // TODO: Cache FormattedText

            Typeface = new AM.Typeface(Style.TextStyle.FontName, fontWeight, fontStyle);

            var textAlignment = Style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Right => AM.TextAlignment.Right,
                TextHAlignment.Center => AM.TextAlignment.Center,
                _ => AM.TextAlignment.Left,
            };

            FormattedText = new AM.FormattedText()
            {
                Typeface = Typeface,
                Text = BoundText,
                TextAlignment = textAlignment,
                TextWrapping = AM.TextWrapping.NoWrap,
                FontSize = Style.TextStyle.FontSize,
                Constraint = Rect.Size
            };

            var size = FormattedText.Bounds.Size;
            var rect = Rect;

            var originX = rect.X; // NOTE: Using AM.TextAlignment
            //var originX = Style.TextStyle.TextHAlignment switch
            //{
            //    TextHAlignment.Left => rect.X,
            //    TextHAlignment.Right => rect.Right - size.Width,
            //    _ => (rect.Left + rect.Width / 2.0) - (size.Width / 2.0)
            //};

            var originY = Style.TextStyle.TextVAlignment switch
            {
                TextVAlignment.Top => rect.Y,
                TextVAlignment.Bottom => rect.Bottom - size.Height,
                _ => (rect.Bottom - rect.Height / 2f) - (size.Height / 2f)
            };

            Origin = new A.Point(originX, originY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            if (FormattedText != null)
            {
                context.DrawText(Stroke.Brush, Origin, FormattedText); 
            }
        }
    }
}
