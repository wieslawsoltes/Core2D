// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Controls;
using Perspex.Data;
using Perspex.Input;
using Perspex.Media;
using Renderer.Perspex;
using System;
using static System.Math;

namespace Core2D.Perspex.Controls.PanAndZoom
{
    public class PanAndZoom : Border
    {
        private static AutoFitMode[] _autoFitModes = (AutoFitMode[])Enum.GetValues(typeof(AutoFitMode));

        public static AutoFitMode[] AutoFitModes
        {
            get { return _autoFitModes; }
        }

        private IControl _element;
        private Point _pan;
        private Point _previous;
        private Matrix _matrix;

        public Action<double, double, double, double> InvalidatedChild { get; set; }

        public static PerspexProperty<double> ZoomSpeedProperty =
            PerspexProperty.Register<PanAndZoom, double>("ZoomSpeed", 1.2, false, BindingMode.TwoWay);

        public double ZoomSpeed
        {
            get { return GetValue(ZoomSpeedProperty); }
            set { SetValue(ZoomSpeedProperty, value); }
        }

        public static PerspexProperty<AutoFitMode> AutoFitModeProperty =
            PerspexProperty.Register<PanAndZoom, AutoFitMode>("AutoFitMode", AutoFitMode.Extent, false, BindingMode.TwoWay);

        public AutoFitMode AutoFitMode
        {
            get { return GetValue(AutoFitModeProperty); }
            set { SetValue(AutoFitModeProperty, value); }
        }

        static PanAndZoom()
        {
            AffectsArrange(ZoomSpeedProperty, AutoFitModeProperty);
        }

        public PanAndZoom()
            : base()
        {
            _matrix = MatrixHelper.Identity;

            Focusable = true;
            Background = Brushes.Transparent;

            DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;

            this.GetObservable(ChildProperty).Subscribe(value =>
            {
                if (value != null && value != _element && _element != null)
                {
                    Unload();
                }

                if (value != null && value != _element)
                {
                    Initialize(value);
                }
            });
        }

        private void PanAndZoom_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (_element != null)
            {
                Unload();
            }
        }

        private void Initialize(IControl element)
        {
            if (element != null)
            {
                _element = element;
                this.PointerWheelChanged += Border_PointerWheelChanged;
                this.PointerPressed += Border_PointerPressed;
                this.PointerReleased += Border_PointerReleased;
                this.PointerMoved += Border_PointerMoved;
            }
        }

        private void Unload()
        {
            if (_element != null)
            {
                this.PointerWheelChanged -= Border_PointerWheelChanged;
                this.PointerPressed -= Border_PointerPressed;
                this.PointerReleased -= Border_PointerReleased;
                this.PointerMoved -= Border_PointerMoved;
                _element.RenderTransform = null;
                _element = null;
            }
        }

        private void Border_PointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            if (_element != null)
            {
                Point point = e.GetPosition(_element);
                point = FixInvalidPointPosition(point);
                ZoomDeltaTo(e.Delta.Y, point);
            }
        }

        private void Border_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            switch (e.MouseButton)
            {
                case MouseButton.Right:
                    {
                        if (_element != null)
                        {
                            Point point = e.GetPosition(_element);
                            point = FixInvalidPointPosition(point);
                            StartPan(point);
                            e.Device.Capture(_element);
                        }
                    }
                    break;
            }
        }

        private void Border_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (_element != null)
            {
                switch (e.MouseButton)
                {
                    case MouseButton.Right:
                        {
                            if (_element != null && e.Device.Captured == _element)
                            {
                                e.Device.Capture(null);
                            }
                        }
                        break;
                }
            }
        }

        private void Border_PointerMoved(object sender, PointerEventArgs e)
        {
            if (_element != null && e.Device.Captured == _element)
            {
                Point point = e.GetPosition(_element);
                point = FixInvalidPointPosition(point);
                PanTo(point);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (_element != null && _element.IsMeasureValid)
            {
                AutoFit(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height), _element.Bounds);
            }

            return size;
        }

        public void Invalidate()
        {
            if (_element != null)
            {
                this.InvalidatedChild?.Invoke(_matrix.M11, _matrix.M12, _matrix.M31, _matrix.M32);
                _element.TransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
                _element.RenderTransform = new MatrixTransform(_matrix);
                _element.InvalidateVisual();
            }
        }

        public void ZoomTo(double zoom, Point point)
        {
            _matrix = MatrixHelper.ScaleAtPrepend(_matrix, zoom, zoom, point.X, point.Y);

            Invalidate();
        }

        public void ZoomDeltaTo(double delta, Point point)
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

        public void Extent(Rect panelSize, Rect elementSize)
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

        public void Fill(Rect panelSize, Rect elementSize)
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

        public void AutoFit(Rect panelSize, Rect elementSize)
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
            Extent(this.Bounds, _element.Bounds);
        }

        public void Fill()
        {
            Fill(this.Bounds, _element.Bounds);
        }

        public void AutoFit()
        {
            if (_element != null)
            {
                AutoFit(this.Bounds, _element.Bounds);
            }
        }

        public Point FixInvalidPointPosition(Point point)
        {
            return MatrixHelper.TransformPoint(_matrix.Invert(), point);
        }
    }
}
