// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Renderer.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace Core2D.Wpf.Controls.PanAndZoom
{
    public class PanAndZoom : Border
    {
        private UIElement _element;
        private Point _pan;
        private Point _previous;
        private Matrix _matrix;

        public double ZoomSpeed { get; set; }

        public AutoFitMode AutoFitMode { get; set; }

        public PanAndZoom()
            : base()
        {
            _matrix = MatrixHelper.Identity;

            ZoomSpeed = 1.2;
            AutoFitMode = AutoFitMode.None;

            Focusable = true;
            Background = Brushes.Transparent;

            Unloaded += PanAndZoom_Unloaded;
        }

        private void PanAndZoom_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_element != null)
            {
                Unload();
            }
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != _element && _element != null)
                {
                    Unload();
                }

                base.Child = value;

                if (value != null && value != _element)
                {
                    Initialize(value);
                }
            }
        }

        private void Initialize(UIElement element)
        {
            if (element != null)
            {
                _element = element;
                this.Focus();
                this.PreviewMouseWheel += Border_PreviewMouseWheel;
                this.PreviewMouseRightButtonDown += Border_PreviewMouseRightButtonDown;
                this.PreviewMouseRightButtonUp += Border_PreviewMouseRightButtonUp;
                this.PreviewMouseMove += Border_PreviewMouseMove;
            }
        }

        private void Unload()
        {
            if (_element != null)
            {
                this.PreviewMouseWheel -= Border_PreviewMouseWheel;
                this.PreviewMouseRightButtonDown -= Border_PreviewMouseRightButtonDown;
                this.PreviewMouseRightButtonUp -= Border_PreviewMouseRightButtonUp;
                this.PreviewMouseMove -= Border_PreviewMouseMove;
                _element.RenderTransform = null;
                _element = null;
            }
        }

        private void Border_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_element != null)
            {
                Point point = e.GetPosition(_element);
                ZoomDeltaTo(e.Delta, point);
            }
        }

        private void Border_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_element != null)
            {
                Point point = e.GetPosition(_element);
                StartPan(point);
                _element.CaptureMouse();
            }
        }

        private void Border_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_element != null)
            {
                _element.ReleaseMouseCapture();
            }
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_element != null && _element.IsMouseCaptured)
            {
                Point point = e.GetPosition(_element);
                PanTo(point);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_element != null && _element.IsMeasureValid)
            {
                AutoFit(finalSize, _element.RenderSize);
            }

            return base.ArrangeOverride(finalSize);
        }

        public void Invalidate()
        {
            if (_element != null)
            {
                _element.RenderTransformOrigin = new Point(0, 0);
                _element.RenderTransform = new MatrixTransform(_matrix);
                _element.InvalidateVisual();
            }
        }

        public void ZoomTo(double zoom, Point point)
        {
            _matrix = MatrixHelper.ScaleAtPrepend(_matrix, zoom, zoom, point.X, point.Y);

            Invalidate();
        }

        public void ZoomDeltaTo(int delta, Point point)
        {
            ZoomTo(delta > 0 ? ZoomSpeed : 1 / ZoomSpeed, point);
        }

        public void StartPan(Point point)
        {
            _pan = new Point();
            _previous = new Point(point.X, point.Y);
        }

        public void PanTo(Point point)
        {
            Point delta = new Point(point.X - _previous.X, point.Y - _previous.Y);
            _previous = new Point(point.X, point.Y);

            _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
            _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan.X, _pan.Y);

            Invalidate();
        }

        public void Extent(Size panelSize, Size elementSize)
        {
            if (_element != null)
            {
                double pw = panelSize.Width;
                double ph = panelSize.Height;
                double ew = elementSize.Width;
                double eh = elementSize.Height;
                double zx = pw / ew;
                double zy = ph / eh;
                double zoom = Min(zx, zy);
                double dx = (pw - (ew * zoom)) / 2.0;
                double dy = (ph - (eh * zoom)) / 2.0;
                double ox = dx - Max(0, (pw - ew) / 2.0);
                double oy = dy - Max(0, (ph - eh) / 2.0);

                _matrix = MatrixHelper.Scale(zoom, zoom) * MatrixHelper.Translate(ox, oy);

                Invalidate();
            }
        }

        public void Fill(Size panelSize, Size elementSize)
        {
            if (_element != null)
            {
                double pw = panelSize.Width;
                double ph = panelSize.Height;
                double ew = elementSize.Width;
                double eh = elementSize.Height;
                double zx = pw / ew;
                double zy = ph / eh;

                _matrix = MatrixHelper.ScaleAt(zx, zy, ew > pw ? 0.0 : ew / 2.0, eh > ph ? 0.0 : eh / 2.0);

                Invalidate();
            }
        }

        public void AutoFit(Size panelSize, Size elementSize)
        {
            if (_element != null)
            {
                switch (AutoFitMode)
                {
                    case AutoFitMode.None:
                        Reset();
                        break;
                    case AutoFitMode.Extent:
                        Extent(panelSize, elementSize);
                        break;
                    case AutoFitMode.Fill:
                        Fill(panelSize, elementSize);
                        break;
                }

                Invalidate();
            }
        }

        public void ToggleAutoFitMode()
        {
            switch (AutoFitMode)
            {
                case AutoFitMode.None:
                    AutoFitMode = AutoFitMode.Extent;
                    break;
                case AutoFitMode.Extent:
                    AutoFitMode = AutoFitMode.Fill;
                    break;
                case AutoFitMode.Fill:
                    AutoFitMode = AutoFitMode.None;
                    break;
            }
        }

        public void Reset()
        {
            _matrix = MatrixHelper.Identity;

            Invalidate();
        }

        public void Extent()
        {
            Extent(this.RenderSize, _element.RenderSize);
        }

        public void Fill()
        {
            Fill(this.RenderSize, _element.RenderSize);
        }

        public void AutoFit()
        {
            if (_element != null)
            {
                AutoFit(this.RenderSize, _element.RenderSize);
            }
        }
    }
}
