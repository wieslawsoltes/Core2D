using System;
using System.Diagnostics;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Spatial;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes
{
    internal class ImageDrawNode : DrawNode, IImageDrawNode
    {
        public ImageShapeViewModel Image { get; set; }
        public SKRect Rect { get; set; }
        public IImageCache ImageCache { get; set; }
        public ICache<string, IDisposable> BitmapCache { get; set; }
        public SKBitmap ImageCached { get; set; }
        public SKRect SourceRect { get; set; }
        public SKRect DestRect { get; set; }

        public ImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
            : base()
        {
            Style = style;
            Image = image;
            ImageCache = imageCache;
            BitmapCache = bitmapCache;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Image.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Image.State.HasFlag(ShapeStateFlags.Size);

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
        }
    }
}
