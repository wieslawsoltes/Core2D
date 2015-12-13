// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Controls;
using Perspex.Input;
using Perspex.Media;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Controls.Editor
{
    /// <summary>
    /// Interaction logic for <see cref="DrawableControl"/> xaml.
    /// </summary>
    public class DrawableControl : UserControl
    {
        private ZoomState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawableControl"/> class.
        /// </summary>
        public DrawableControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }

        /// <summary>
        /// Initialize <see cref="ZoomState"/> object
        /// </summary>
        private void InitializeState()
        {
            var editor = this.DataContext as Core2D.Editor;
            if (editor == null)
                return;

            editor.Invalidate = () => this.InvalidateVisual();
            editor.ResetZoom = () => this.OnZoomReset();
            editor.ExtentZoom = () => this.OnZoomExtent();

            _state = new ZoomState(editor);

            if (editor.Renderers != null && editor.Renderers[0].State.EnableAutofit)
            {
                AutoFit(this.Bounds.Width, this.Bounds.Height);
            }

            this.PointerPressed +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;

                    var p = e.GetPosition(this);

                    if (e.MouseButton == MouseButton.Left)
                    {
                        this.Focus();
                        _state.LeftDown(p.X, p.Y);
                    }

                    if (e.MouseButton == MouseButton.Right)
                    {
                        this.Cursor = new Cursor(StandardCursorType.Hand);
                        this.Focus();
                        _state.RightDown(p.X, p.Y);
                    }
                };

            this.PointerReleased +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;

                    var p = e.GetPosition(this);

                    if (e.MouseButton == MouseButton.Left)
                    {
                        this.Focus();
                        _state.LeftUp(p.X, p.Y);
                    }

                    if (e.MouseButton == MouseButton.Right)
                    {
                        this.Cursor = new Cursor(StandardCursorType.Arrow);
                        this.Focus();
                        _state.RightUp(p.X, p.Y);
                    }
                };

            this.PointerMoved +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;

                    var p = e.GetPosition(this);
                    _state.Move(p.X, p.Y);
                };

            this.PointerWheelChanged +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;

                    if (editor == null || editor.Project == null)
                        return;

                    var container = editor.Project.CurrentContainer;
                    if (container == null)
                        return;

                    var p = e.GetPosition(this);
                    _state.Wheel(
                        p.X,
                        p.Y,
                        e.Delta.Y,
                        this.Bounds.Width,
                        this.Bounds.Height,
                        container.Template.Width,
                        container.Template.Height);
                };
        }

        /// <summary>
        /// Positions child elements as part of a layout pass.
        /// </summary>
        /// <param name="finalSize">The size available to the control.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var editor = this.DataContext as Core2D.Editor;
            if (editor != null && editor.Project != null)
            {
                if (editor.Renderers != null && editor.Renderers[0].State.EnableAutofit)
                {
                    AutoFit(finalSize.Width, finalSize.Height);
                }
                else
                {
                    ResetZoom(finalSize.Width, finalSize.Height);
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Resets control zoom and pan properties.
        /// </summary>
        /// <param name="width">The control width.</param>
        /// <param name="height">The control height.</param>
        public void ResetZoom(double width, double height)
        {
            if (_state == null)
                return;

            var editor = this.DataContext as Core2D.Editor;
            if (editor == null || editor.Project == null)
                return;

            var container = editor.Project.CurrentContainer;
            if (container == null)
                return;

            _state.CenterTo(
                width,
                height,
                container.Template.Width,
                container.Template.Height);

            editor.Invalidate();
        }

        /// <summary>
        /// Auto-fits control using zoom and pan properties.
        /// </summary>
        /// <param name="width">The control width.</param>
        /// <param name="height">The control height.</param>
        public void AutoFit(double width, double height)
        {
            if (_state == null)
                return;

            var editor = this.DataContext as Core2D.Editor;
            if (editor == null || editor.Project == null)
                return;

            var container = editor.Project.CurrentContainer;
            if (container == null)
                return;

            _state.FitTo(
                width,
                height,
                container.Template.Width,
                container.Template.Height);

            editor.Invalidate();
        }

        /// <summary>
        /// Reset pan and zoom to default state.
        /// </summary>
        public void OnZoomReset()
        {
            var editor = this.DataContext as Core2D.Editor;
            if (editor == null)
                return;

            ResetZoom(this.Bounds.Width, this.Bounds.Height);
            editor.Invalidate();
        }

        /// <summary>
        /// Stretch view to the available extents.
        /// </summary>
        public void OnZoomExtent()
        {
            var editor = this.DataContext as Core2D.Editor;
            if (editor == null)
                return;

            AutoFit(this.Bounds.Width, this.Bounds.Height);
            editor.Invalidate();
        }

        /// <summary>
        /// Draws background rectangle with specified color.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="c">The background color.</param>
        /// <param name="width">The width of background rectangle.</param>
        /// <param name="height">The height of background rectangle.</param>
        private void DrawBackground(DrawingContext dc, ArgbColor c, double width, double height)
        {
            var color = Color.FromArgb(c.A, c.R, c.G, c.B);
            var brush = new SolidColorBrush(color);
            var rect = new Rect(0, 0, width, height);
            dc.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draws the current container.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        private void Draw(DrawingContext dc)
        {
            var editor = this.DataContext as Core2D.Editor;
            if (editor == null || editor.Project == null)
                return;

            var renderer = editor.Renderers[0];
            var container = editor.Project.CurrentContainer;
            if (container == null)
                return;

            var translate = dc.PushPreTransform(Matrix.CreateTranslation(_state.PanX, _state.PanY));
            var scale = dc.PushPreTransform(Matrix.CreateScale(_state.Zoom, _state.Zoom));

            var template = container.Template;
            if (template != null)
            {
                DrawBackground(
                    dc,
                    template.Background,
                    template.Width,
                    template.Height);

                renderer.Draw(
                    dc,
                    template,
                    container.Data.Properties,
                    null);
            }
            else
            {
                DrawBackground(
                    dc,
                    container.Background,
                    container.Width,
                    container.Height);
            }

            renderer.Draw(
                dc,
                container,
                container.Data.Properties,
                null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(
                    dc,
                    container.WorkingLayer,
                    container.Data.Properties,
                    null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(
                    dc,
                    container.HelperLayer,
                    container.Data.Properties,
                    null);
            }

            scale.Dispose();
            translate.Dispose();
        }

        /// <summary>
        /// Renders drawable control contents.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_state == null)
            {
                InitializeState();
            }

            Draw(context);
        }
    }
}
