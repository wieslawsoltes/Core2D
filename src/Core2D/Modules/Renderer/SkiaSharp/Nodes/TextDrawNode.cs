// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class TextDrawNode : DrawNode, ITextDrawNode
{
    public TextShapeViewModel Text { get; set; }
    public SKRect Rect { get; set; }
    public SKPoint Origin { get; set; }
    public SKTypeface? Typeface { get; set; }
    public SKPaint? FormattedText { get; set; }
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
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
        }
        else
        {
            Rect = SKRect.Empty;
            Center = SKPoint.Empty;
        }

        UpdateTextGeometry();
    }

    private void UpdateTextGeometry()
    {
        BoundText = Text.GetProperty(nameof(TextShapeViewModel.Text)) as string ?? Text.Text;

        if (BoundText is null || Style?.TextStyle is null)
        {
            FormattedText = null;
            Origin = SKPoint.Empty;
            return;
        }

        if (Style.TextStyle.FontSize < 0.0)
        {
            FormattedText = null;
            Origin = SKPoint.Empty;
            return;
        }

        if (Text.TopLeft is { } && Text.BottomRight is { })
        {
            FormattedText = SkiaSharpDrawUtil.GetSKPaint(BoundText, Style, Text.TopLeft, Text.BottomRight, out var origin);
            Origin = origin;
        }
        else
        {
            FormattedText = null;
            Origin = SKPoint.Empty;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (FormattedText is { })
        {
            canvas.DrawText(BoundText, Origin.X, Origin.Y, FormattedText);
        }
    }
}
