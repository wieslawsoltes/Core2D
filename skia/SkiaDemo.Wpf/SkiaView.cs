using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core2D.Project;
using Core2D.Renderer;
using SkiaSharp;

namespace SkiaDemo.Wpf
{
    public class SkiaView : FrameworkElement
    {
        private double _actualWidth = 0.0;
        private double _actualHeight = 0.0;
        private double _dpiX = 1.0;
        private double _dpiY = 1.0;
        private int _width = 0;
        private int _height = 0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;
        private WriteableBitmap _bitmap = null;

        public ShapeRenderer Renderer { get; set; }

        public XContainer Container { get; set; }

        public ContainerPresenter Presenter { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Renderer != null && Container != null && Presenter != null)
            {
                Update();
                Render(drawingContext);
            }
        }

        private void Update()
        {
            var matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            _dpiX = matrix.M11;
            _dpiY = matrix.M22;

            _actualWidth = ActualWidth;
            _actualHeight = ActualHeight;

            _width = (int)(_actualWidth * _dpiX);
            _height = (int)(_actualHeight * _dpiY);

            _offsetX = (_width - Container.Width) / 2.0;
            _offsetY = (_height - Container.Height) / 2.0;
        }

        private void Render(DrawingContext drawingContext)
        {
            if (_width > 0 && _height > 0)
            {
                if (_bitmap == null || _bitmap.Width != _width || _bitmap.Height != _height)
                {
                    _bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Pbgra32, null);
                }

                _bitmap.Lock();
                using (var surface = SKSurface.Create(_width, _height, SKImageInfo.PlatformColorType, SKAlphaType.Premul, _bitmap.BackBuffer, _bitmap.BackBufferStride))
                {
                    var canvas = surface.Canvas;
                    canvas.Scale((float)_dpiX, (float)_dpiY);
                    canvas.Clear();
                    using (new SKAutoCanvasRestore(canvas, true))
                    {
                        Presenter.Render(canvas, Renderer, Container, _offsetX, _offsetY);
                    }
                }
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _bitmap.Unlock();

                drawingContext.DrawImage(_bitmap, new Rect(0, 0, _actualWidth, _actualHeight));
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }

        public Point FixPointOffset(Point point)
        {
            return new Point(point.X - _offsetX, point.Y - _offsetY);
        }
    }
}
