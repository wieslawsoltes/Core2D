// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Core2D.Wpf.Controls
{
    /// <summary>
    /// The custom pan and zoom control.
    /// </summary>
    public class PanAndZoomBorder : Border
    {
        private const double _minimum = 0.01;
        private const double _maximum = 1000.0;
        private const double _zoomSpeed = 3.5;

        /// <summary>
        /// Gets or sets invalidate action.
        /// </summary>
        public Action<double, double, double> InvalidateChild { get; set; }

        /// <summary>
        /// Gets or sets auto-fit action.
        /// </summary>
        public Action<double, double> AutoFitChild { get; set; }

        /// <summary>
        /// Gets or sets pan and zoom action.
        /// </summary>
        public Action<double, double, double> ZoomAndPanChild { get; set; }

        /// <summary>
        /// Gets or sets the scale transform.
        /// </summary>
        public ScaleTransform Scale { get; set; }

        /// <summary>
        /// Gets or sets the translate transform.
        /// </summary>
        public TranslateTransform Translate { get; set; }

        /// <summary>
        /// Auto-fit container in parent panel.
        /// </summary>
        /// <param name="pwidth">The parent panel width.</param>
        /// <param name="pheight">The parent panel height.</param>
        /// <param name="cwidth">The container width.</param>
        /// <param name="cheight">The container height.</param>
        public void FitTo(double pwidth, double pheight, double cwidth, double cheight)
        {
            double zoom = Math.Min(pwidth / cwidth, pheight / cheight) - 0.001;
            double px = (pwidth - (cwidth * zoom)) / 2.0;
            double py = (pheight - (cheight * zoom)) / 2.0;
            double x = px - Math.Max(0, (pwidth - cwidth) / 2.0);
            double y = py - Math.Max(0, (pheight - cheight) / 2.0);

            if (this.ZoomAndPanChild != null)
            {
                this.ZoomAndPanChild(x, y, zoom);
            }
        }

        private void SetChild(UIElement value)
        {
            if (value != null && value != this.Child)
            {
                UIElement child = value;
                var origin = new Point();
                var start = new Point();

                var group = new TransformGroup();
                var st = new ScaleTransform();
                group.Children.Add(st);
                var tt = new TranslateTransform();
                group.Children.Add(tt);

                this.Scale = st;
                this.Translate = tt;

                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);

                this.ZoomAndPanChild = (x, y, zoom) =>
                {
                    st.ScaleX = zoom;
                    st.ScaleY = zoom;
                    tt.X = x;
                    tt.Y = y;

                    if (InvalidateChild != null)
                    {
                        InvalidateChild(st.ScaleX, tt.X, tt.Y);
                    }
                };

                this.MouseWheel += (s, e) =>
                {
                    if (child != null)
                    {
                        double zoom = st.ScaleX;
                        zoom = e.Delta > 0 ?
                            zoom + zoom / _zoomSpeed :
                            zoom - zoom / _zoomSpeed;
                        if (zoom < _minimum || zoom > _maximum)
                            return;

                        Point relative = e.GetPosition(child);
                        double abosuluteX = relative.X * st.ScaleX + tt.X;
                        double abosuluteY = relative.Y * st.ScaleY + tt.Y;
                        st.ScaleX = zoom;
                        st.ScaleY = zoom;
                        tt.X = abosuluteX - relative.X * st.ScaleX;
                        tt.Y = abosuluteY - relative.Y * st.ScaleY;

                        if (InvalidateChild != null)
                        {
                            InvalidateChild(st.ScaleX, tt.X, tt.Y);
                        }
                    }
                };

                this.MouseMove += (s, e) =>
                {
                    if (child != null && child.IsMouseCaptured)
                    {
                        Vector v = start - e.GetPosition(this);
                        tt.X = origin.X - v.X;
                        tt.Y = origin.Y - v.Y;
                        if (InvalidateChild != null)
                        {
                            InvalidateChild(st.ScaleX, tt.X, tt.Y);
                        }
                    }
                };

                this.PreviewMouseDown += (s, e) =>
                {
                    if (this.Tag != null)
                    {
                        bool cancelAvailable = (bool)this.Tag;
                        if (cancelAvailable)
                            return;
                    }

                    if (child != null
                        && e.ChangedButton == MouseButton.Right
                        && e.ClickCount == 1
                        && e.ButtonState == MouseButtonState.Pressed)
                    {
                        start = e.GetPosition(this);
                        origin = new Point(tt.X, tt.Y);
                        this.Cursor = Cursors.Hand;
                        child.CaptureMouse();
                    }
                };

                this.MouseUp += (s, e) =>
                {
                    if (this.Tag != null)
                    {
                        bool cancelAvailable = (bool)this.Tag;
                        if (cancelAvailable)
                            return;
                    }

                    if (child != null
                        && e.ChangedButton == MouseButton.Right
                        && e.ClickCount == 1
                        && e.ButtonState == MouseButtonState.Released)
                    {
                        child.ReleaseMouseCapture();
                        this.Cursor = Cursors.Arrow;
                    }
                };
            }

            base.Child = value;
        }

        /// <inheritdoc/>
        public override UIElement Child
        {
            get { return base.Child; }
            set { SetChild(value); }
        }
    }
}
