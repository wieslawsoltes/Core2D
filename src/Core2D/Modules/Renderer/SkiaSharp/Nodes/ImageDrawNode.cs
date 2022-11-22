﻿#nullable enable
using System;
using System.Diagnostics;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class ImageDrawNode : DrawNode, IImageDrawNode
{
    public ImageShapeViewModel Image { get; set; }
    public SKRect Rect { get; set; }
    public IImageCache? ImageCache { get; set; }
    public ICache<string, IDisposable>? BitmapCache { get; set; }
    public SKBitmap? ImageCached { get; set; }
    public SKRect SourceRect { get; set; }
    public SKRect DestRect { get; set; }

    public ImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel? style, IImageCache? imageCache, ICache<string, IDisposable>? bitmapCache)
    {
        Style = style;
        Image = image;
        ImageCache = imageCache;
        BitmapCache = bitmapCache;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Image.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Image.State.HasFlag(ShapeStateFlags.Size);

        if (!string.IsNullOrEmpty(Image.Key))
        {
            ImageCached = BitmapCache?.Get(Image.Key) as SKBitmap;
            if (ImageCached is null && ImageCache is { })
            {
                try
                {
                    var bytes = ImageCache.GetImage(Image.Key);
                    if (bytes is { })
                    {
                        ImageCached = SKBitmap.Decode(bytes);
                        BitmapCache?.Set(Image.Key, ImageCached);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                    Debug.WriteLine($"{ex.StackTrace}");
                }
            }

            if (ImageCached is { })
            {
                SourceRect = new SKRect(0, 0, ImageCached.Width, ImageCached.Height);
            }
        }
        else
        {
            ImageCached = null;
        }

        if (Image.TopLeft is { } && Image.BottomRight is { })
        {
            var rect2 = Rect2.FromPoints(Image.TopLeft.X, Image.TopLeft.Y, Image.BottomRight.X, Image.BottomRight.Y);
            DestRect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(DestRect.MidX, DestRect.MidY);
        }
        else
        {
            DestRect = SKRect.Empty;
            Center = SKPoint.Empty;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Image.IsFilled)
        {
            canvas.DrawRect(DestRect, Fill);
        }

        if (Image.IsStroked)
        {
            canvas.DrawRect(DestRect, Stroke);
        }

        if (ImageCached is { })
        {
            try
            {
                canvas.DrawBitmap(ImageCached, SourceRect, DestRect);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                Debug.WriteLine($"{ex.StackTrace}");
            }
        }
    }
}
