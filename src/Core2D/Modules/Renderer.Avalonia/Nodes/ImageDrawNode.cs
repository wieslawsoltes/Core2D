using System;
using System.Diagnostics;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;
using AMI = Avalonia.Media.Imaging;

namespace Core2D.Renderer
{
    internal class ImageDrawNode : DrawNode, IImageDrawNode
    {
        public ImageShapeViewModel Image { get; set; }
        public A.Rect Rect { get; set; }
        public IImageCache ImageCache { get; set; }
        public ICache<string, IDisposable> BitmapCache { get; set; }
        public AMI.Bitmap ImageCached { get; set; }
        public A.Rect SourceRect { get; set; }
        public A.Rect DestRect { get; set; }

        public ImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel styleViewModel, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
            : base()
        {
            StyleViewModel = styleViewModel;
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
                ImageCached = BitmapCache.Get(Image.Key) as AMI.Bitmap;
                if (ImageCached == null && ImageCache != null)
                {
                    try
                    {
                        var bytes = ImageCache.GetImage(Image.Key);
                        if (bytes != null)
                        {
                            using var ms = new System.IO.MemoryStream(bytes);
                            ImageCached = new AMI.Bitmap(ms);
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
                    SourceRect = new A.Rect(0, 0, ImageCached.PixelSize.Width, ImageCached.PixelSize.Height);
                }
            }
            else
            {
                ImageCached = null;
            }

            var rect2 = Rect2.FromPoints(Image.TopLeft.X, Image.TopLeft.Y, Image.BottomRight.X, Image.BottomRight.Y, 0, 0);
            DestRect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = DestRect.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            if (Image.IsFilled)
            {
                context.FillRectangle(Fill, DestRect);
            }

            if (Image.IsStroked)
            {
                context.DrawRectangle(Stroke, DestRect);
            }

            if (ImageCached != null)
            {
                try
                {
                    context.DrawImage(ImageCached, SourceRect, DestRect);
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
