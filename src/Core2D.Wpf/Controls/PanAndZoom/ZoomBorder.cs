// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Math;

namespace Wpf.Controls.PanAndZoom
{
    /// <summary>
    /// 
    /// </summary>
    public class ZoomBorder : Border
    {
        private UIElement _element;
        private Point _pan;
        private Point _previous;
        private Matrix _matrix;

        /// <summary>
        /// 
        /// </summary>
        public double ZoomSpeed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AutoFitMode AutoFitMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<double, double, double, double> InvalidatedChild { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ZoomBorder()
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_element != null && _element.IsMeasureValid)
            {
                AutoFit(this.DesiredSize, _element.DesiredSize);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Invalidate()
        {
            if (_element != null)
            {
                this.InvalidatedChild?.Invoke(_matrix.M11, _matrix.M12, _matrix.OffsetX, _matrix.OffsetY);
                _element.RenderTransformOrigin = new Point(0, 0);
                _element.RenderTransform = new MatrixTransform(_matrix);
                _element.InvalidateVisual();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="point"></param>
        public void ZoomTo(double zoom, Point point)
        {
            _matrix = MatrixHelper.ScaleAtPrepend(_matrix, zoom, zoom, point.X, point.Y);

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="point"></param>
        public void ZoomDeltaTo(int delta, Point point)
        {
            ZoomTo(delta > 0 ? ZoomSpeed : 1 / ZoomSpeed, point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void StartPan(Point point)
        {
            _pan = new Point();
            _previous = new Point(point.X, point.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void PanTo(Point point)
        {
            Point delta = new Point(point.X - _previous.X, point.Y - _previous.Y);
            _previous = new Point(point.X, point.Y);

            _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
            _matrix = MatrixHelper.TranslatePrepend(_matrix, _pan.X, _pan.Y);

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelSize"></param>
        /// <param name="elementSize"></param>
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
                double cx = ew / 2.0;
                double cy = eh / 2.0;

                _matrix = MatrixHelper.ScaleAt(zoom, zoom, cx, cy);

                Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelSize"></param>
        /// <param name="elementSize"></param>
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

                _matrix = MatrixHelper.ScaleAt(zx, zy, ew / 2.0, eh / 2.0);

                Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelSize"></param>
        /// <param name="elementSize"></param>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _matrix = MatrixHelper.Identity;

            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Extent()
        {
            Extent(this.DesiredSize, _element.RenderSize);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Fill()
        {
            Fill(this.DesiredSize, _element.RenderSize);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AutoFit()
        {
            if (_element != null)
            {
                AutoFit(this.DesiredSize, _element.RenderSize);
            }
        }
    }
}
