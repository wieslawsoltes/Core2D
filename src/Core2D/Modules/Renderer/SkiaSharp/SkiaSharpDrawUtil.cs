#nullable enable
using System;
using Core2D.Model.Style;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp;

internal static class SkiaSharpDrawUtil
{
    public static SKRect ToSKRect(double x, double y, double width, double height)
    {
        float left = (float)x;
        float top = (float)y;
        float right = (float)(x + width);
        float bottom = (float)(y + height);
        return new SKRect(left, top, right, bottom);
    }

    public static SKRect CreateRect(PointShapeViewModel tl, PointShapeViewModel br)
    {
        float left = (float)Math.Min(tl.X, br.X);
        float top = (float)Math.Min(tl.Y, br.Y);
        float right = (float)Math.Max(tl.X, br.X);
        float bottom = (float)Math.Max(tl.Y, br.Y);
        return new SKRect(left, top, right, bottom);
    }

    public static SKColor ToSKColor(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => new SKColor(argbColor.R, argbColor.G, argbColor.B, argbColor.A),
            _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported."),
        };
    }

    public static SKPaint ToSKPaintBrush(BaseColorViewModel colorViewModel)
    {
        var brush = new SKPaint();

        brush.Style = SKPaintStyle.Fill;
        brush.IsAntialias = true;
        brush.IsStroke = false;
        brush.LcdRenderText = true;
        brush.SubpixelText = true;
        brush.Color = ToSKColor(colorViewModel);

        return brush;
    }

    public static SKStrokeCap ToStrokeCap(ShapeStyleViewModel style)
    {
        return style.Stroke?.LineCap switch
        {
            LineCap.Square => SKStrokeCap.Square,
            LineCap.Round => SKStrokeCap.Round,
            _ => SKStrokeCap.Butt,
        };
    }

    public static SKPaint ToSKPaintPen(ShapeStyleViewModel style, double strokeWidth)
    {
        var pen = new SKPaint();

        var pathEffect = default(SKPathEffect);
        if (style.Stroke?.Dashes is { })
        {
            var intervals = StyleHelper.ConvertDashesToFloatArray(style.Stroke.Dashes, strokeWidth);
            var phase = (float)(style.Stroke.DashOffset * strokeWidth);
            if (intervals is { })
            {
                pathEffect = SKPathEffect.CreateDash(intervals, phase);
            }
        }

        pen.Style = SKPaintStyle.Stroke;
        pen.IsAntialias = true;
        pen.IsStroke = true;
        pen.StrokeWidth = (float)strokeWidth;
        pen.Color = style.Stroke?.Color is { } ? ToSKColor(style.Stroke.Color) : SKColor.Empty;
        pen.StrokeCap = ToStrokeCap(style);
        pen.PathEffect = pathEffect;

        return pen;
    }

    public static SKPaint ToSKPaintPen(BaseColorViewModel colorViewModel, double strokeWidth)
    {
        var pen = new SKPaint();

        var pathEffect = default(SKPathEffect);

        pen.Style = SKPaintStyle.Stroke;
        pen.IsAntialias = true;
        pen.IsStroke = true;
        pen.StrokeWidth = (float)strokeWidth;
        pen.Color = ToSKColor(colorViewModel);
        pen.StrokeCap = SKStrokeCap.Butt;
        pen.PathEffect = pathEffect;

        return pen;
    }

    public static SKPoint GetTextOrigin(ShapeStyleViewModel style, ref SKRect rect, ref SKRect size)
    {
        var rectWidth = Math.Abs(rect.Right - rect.Left);
        var rectHeight = Math.Abs(rect.Bottom - rect.Top);
        var sizeWidth = Math.Abs(size.Right - size.Left);
        var sizeHeight = Math.Abs(size.Bottom - size.Top);
        var ox = style.TextStyle?.TextHAlignment switch
        {
            TextHAlignment.Left => rect.Left,
            TextHAlignment.Right => rect.Right - sizeWidth,
            _ => rect.Left + rectWidth / 2f - sizeWidth / 2f,
        };
        var oy = style.TextStyle?.TextVAlignment switch
        {
            TextVAlignment.Top => rect.Top,
            TextVAlignment.Bottom => rect.Bottom - sizeHeight,
            _ => rect.Bottom - rectHeight / 2f - sizeHeight / 2f,
        };
        return new SKPoint(ox, oy);
    }

    public static SKPaint? GetSKPaint(string text, ShapeStyleViewModel shapeStyleViewModel, PointShapeViewModel topLeft, PointShapeViewModel bottomRight, out SKPoint origin)
    {
        if (shapeStyleViewModel.Stroke?.Color is null || shapeStyleViewModel.TextStyle is null)
        {
            origin = SKPoint.Empty;
            return null;
        }
        
        var pen = ToSKPaintBrush(shapeStyleViewModel.Stroke.Color);

        var weight = SKFontStyleWeight.Normal;

        if (shapeStyleViewModel.TextStyle.FontStyle.HasFlag(FontStyleFlags.Bold))
        {
            weight = SKFontStyleWeight.Bold;
        }

        var style = SKFontStyleSlant.Upright;

        if (shapeStyleViewModel.TextStyle.FontStyle.HasFlag(FontStyleFlags.Italic))
        {
            style = SKFontStyleSlant.Italic;
        }

        var tf = shapeStyleViewModel.TextStyle.FontName is{ }
            ? SKTypeface.FromFamilyName(shapeStyleViewModel.TextStyle.FontName, weight, SKFontStyleWidth.Normal, style)
            : SKTypeface.Default;
        pen.Typeface = tf;
        pen.TextEncoding = SKTextEncoding.Utf16;
        pen.TextSize = (float)shapeStyleViewModel.TextStyle.FontSize;

        pen.TextAlign = shapeStyleViewModel.TextStyle.TextHAlignment switch
        {
            TextHAlignment.Center => SKTextAlign.Center,
            TextHAlignment.Right => SKTextAlign.Right,
            _ => SKTextAlign.Left,
        };

        var metrics = pen.FontMetrics;
        var mAscent = metrics.Ascent;
        var mDescent = metrics.Descent;
        var rect = CreateRect(topLeft, bottomRight);
        var x = rect.Left;
        var y = rect.Top;
        var width = rect.Width;
        var height = rect.Height;

        switch (shapeStyleViewModel.TextStyle.TextVAlignment)
        {
            default:
            {
                y -= mAscent;
                break;
            }
            case TextVAlignment.Center:
            {
                y += height / 2.0f - mAscent / 2.0f - mDescent / 2.0f;
                break;
            }
            case TextVAlignment.Bottom:
            {
                y += height - mDescent;
                break;
            }
        }

        switch (shapeStyleViewModel.TextStyle.TextHAlignment)
        {
            case TextHAlignment.Center:
            {
                x += width / 2.0f;
                break;
            }
            case TextHAlignment.Right:
            {
                x += width;
                break;
            }
        }

        origin = new SKPoint(x, y);

        return pen;
    }
}
