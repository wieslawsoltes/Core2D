using System;
using System.Diagnostics;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class ImageDrawNode : TextDrawNode, IImageDrawNode
    {
        public ImageShape Image { get; set; }
        public IImageCache ImageCache { get; set; }
        public ICache<string, IDisposable> BitmapCache { get; set; }
        public SKBitmap ImageCached { get; set; }
        public SKRect SourceRect { get; set; }
        public SKRect DestRect { get; set; }

        public ImageDrawNode(ImageShape image, ShapeStyle style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
            : base()
        {
            Style = style;
            Image = image;
            Text = image;
            ImageCache = imageCache;
            BitmapCache = bitmapCache;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Image.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Image.State.Flags.HasFlag(ShapeStateFlags.Size);

            if (!string.IsNullOrEmpty(Image.Key))
            {
                ImageCached = BitmapCache.Get(Image.Key) as SKBitmap;
                if (ImageCached == null && ImageCache != null)
                {
                    try
                    {
                        var bytes = ImageCache.GetImage(Image.Key);
                        if (bytes != null)
                        {
                            ImageCached = SKBitmap.Decode(bytes);
                            BitmapCache.Set(Image.Key, ImageCached);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex.Message}");
                        Debug.WriteLine($"{ex.StackTrace}");
                    }
                }

                if (ImageCached != null)
                {
                    SourceRect = new SKRect(0, 0, ImageCached.Width, ImageCached.Height);
                }
            }
            else
            {
                ImageCached = null;
            }

            var rect2 = Rect2.FromPoints(Image.TopLeft.X, Image.TopLeft.Y, Image.BottomRight.X, Image.BottomRight.Y, 0, 0);
            DestRect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(DestRect.MidX, DestRect.MidY);

            base.UpdateTextGeometry();
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Image.IsFilled)
            {
                canvas.DrawRect(DestRect, Fill);
            }

            if (Image.IsStroked)
            {
                canvas.DrawRect(DestRect, Stroke);
            }

            if (ImageCached != null)
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

            base.OnDraw(dc, zoom);
        }
    }
}
