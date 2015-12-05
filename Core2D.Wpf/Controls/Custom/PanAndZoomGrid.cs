// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Custom
{
    /// <summary>
    /// The custom pan and zoom control.
    /// </summary>
    public class PanAndZoomGrid : Grid
    {
        /// <summary>
        /// Gets or sets flag indicating whether auto-fit is enabled.
        /// </summary>
        public bool EnableAutoFit
        {
            get { return (bool)GetValue(EnableAutoFitProperty); }
            set { SetValue(EnableAutoFitProperty, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether auto-fit is enabled.
        /// </summary>
        public static readonly DependencyProperty EnableAutoFitProperty =
            DependencyProperty.Register("EnableAutoFit", typeof(bool), typeof(PanAndZoomGrid), new PropertyMetadata(true));

        /// <summary>
        /// Stretch view to the available extents.
        /// </summary>
        public void AutoFit()
        {
            if (base.Children != null && base.Children.Count == 1)
            {
                var zoom = base.Children[0] as PanAndZoomBorder;
                if (zoom != null && zoom.AutoFitChild != null)
                {
                    zoom.AutoFitChild(
                        this.ActualWidth,
                        this.ActualHeight);
                }
            }
        }

        /// <summary>
        /// Reset pan and zoom to default state.
        /// </summary>
        public void ResetZoomAndPan()
        {
            if (base.Children != null && base.Children.Count == 1)
            {
                var zoom = base.Children[0] as PanAndZoomBorder;
                if (zoom != null && zoom.ZoomAndPanChild != null)
                {
                    zoom.ZoomAndPanChild(0.0, 0.0, 1.0);
                }
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (EnableAutoFit
                && base.Children != null
                && base.Children.Count == 1)
            {
                var zoom = base.Children[0] as PanAndZoomBorder;
                if (zoom != null
                    && zoom.AutoFitChild != null)
                {
                    zoom.AutoFitChild(
                        arrangeSize.Width,
                        arrangeSize.Height);
                }
            }
            return base.ArrangeOverride(arrangeSize);
        }
    }
}
