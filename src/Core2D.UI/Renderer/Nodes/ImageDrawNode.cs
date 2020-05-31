using System;
using System.Diagnostics;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;
using AMI = Avalonia.Media.Imaging;

namespace Core2D.UI.Renderer
{
    internal class ImageDrawNode : TextDrawNode
    {
        public IImageShape Image { get; set; }
        public IImageCache ImageCache { get; set; }
        public ICache<string, AMI.Bitmap> BitmapCache { get; set; }
        public AMI.Bitmap ImageCached { get; set; }
        public A.Rect SourceRect { get; set; }
        public A.Rect DestRect { get; set; }

        public ImageDrawNode(IImageShape image, IShapeStyle style, IImageCache imageCache, ICache<string, AMI.Bitmap> bitmapCache)
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
                ImageCached = BitmapCache.Get(Image.Key);
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

            base.UpdateTextGeometry();
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
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

            base.OnDraw(context, dx, dy, zoom);
        }
    }
}
