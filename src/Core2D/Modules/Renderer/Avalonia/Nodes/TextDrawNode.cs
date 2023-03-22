#nullable enable
using System.Globalization;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Model.Style;
using Core2D.Spatial;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class TextDrawNode : DrawNode, ITextDrawNode
{
    public TextShapeViewModel Text { get; set; }
    public A.Rect Rect { get; set; }
    public A.Point Origin { get; set; }
    public AM.Typeface? Typeface { get; set; }
    public AM.FormattedText? FormattedText { get; set; }
    public AM.Geometry? Geometry { get; set; }
    public string? BoundText { get; set; }

    public TextDrawNode(TextShapeViewModel text, ShapeStyleViewModel? style)
    {
        Style = style;
        Text = text;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Text.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Text.State.HasFlag(ShapeStateFlags.Size);

        if (Text.TopLeft is { } && Text.BottomRight is { })
        {
            var rect2 = Rect2.FromPoints(Text.TopLeft.X, Text.TopLeft.Y, Text.BottomRight.X, Text.BottomRight.Y);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }
        else
        {
            Rect = new A.Rect();
            Center = new A.Point();
        }

        UpdateTextGeometry();
    }

    private void UpdateTextGeometry()
    {
        BoundText = Text.GetProperty(nameof(TextShapeViewModel.Text)) as string ?? Text.Text;

        if (BoundText is null || Style?.TextStyle is null)
        {
            Origin = new A.Point();
            FormattedText = null;
            Geometry = null;
            return;
        }

        if (Style.TextStyle.FontSize < 0.0)
        {
            Origin = new A.Point();
            FormattedText = null;
            Geometry = null;
            return;
        }

        var fontStyle = AM.FontStyle.Normal;
        var fontWeight = AM.FontWeight.Normal;

        if (Style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Italic))
        {
            fontStyle = AM.FontStyle.Italic;
        }

        if (Style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Bold))
        {
            fontWeight = AM.FontWeight.Bold;
        }

        // TODO: Cache Typeface
        // TODO: Cache FormattedText

        if (string.IsNullOrEmpty(Style.TextStyle.FontName))
        {
            Typeface = null;
        }
        else
        {
            Typeface = new AM.Typeface(Style.TextStyle.FontName, fontStyle, fontWeight);
        }

        Typeface ??= AM.Typeface.Default;

        if (Stroke is null)
        {
            UpdateStyle();
        }

        if (Stroke?.Brush is null)
        {
            Origin = new A.Point();
            FormattedText = null;
            Geometry = null;
            return;
        }

        FormattedText = new AM.FormattedText(
            BoundText,
            CultureInfo.InvariantCulture,
            AM.FlowDirection.LeftToRight,
            Typeface.Value,
            Style.TextStyle.FontSize,
            Stroke.Brush
        );

        //FormattedText.MaxTextWidth = Rect.Size.Width;
        //FormattedText.MaxTextHeight = Rect.Size.Height;
        //FormattedText.TextAlignment = textAlignment;
        FormattedText.Trimming = AM.TextTrimming.None;
        // TODO: AM.TextWrapping.NoWrap

        var size = new A.Size(FormattedText.Width, FormattedText.Height);
        var rect = Rect;

        var originX = Style.TextStyle.TextHAlignment switch
        {
            TextHAlignment.Left => rect.X,
            TextHAlignment.Right => rect.Right - size.Width,
            _ => (rect.Left + rect.Width / 2.0) - (size.Width / 2.0)
        };

        var originY = Style.TextStyle.TextVAlignment switch
        {
            TextVAlignment.Top => rect.Y,
            TextVAlignment.Bottom => rect.Bottom - size.Height,
            _ => (rect.Bottom - rect.Height / 2f) - (size.Height / 2f)
        };

        Origin = new A.Point(originX, originY);

        Geometry = FormattedText.BuildGeometry(Origin);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AP.IDrawingContextImpl context)
        {
            return;
        }

        if (Stroke?.Brush is { } && Geometry?.PlatformImpl is { })
        {
            // context.DrawGeometry(Text.IsFilled ? Fill : null, Text.IsStroked ? Stroke : null, Geometry.PlatformImpl);
            context.DrawGeometry(Stroke.Brush, null, Geometry.PlatformImpl);
        }
    }
}
