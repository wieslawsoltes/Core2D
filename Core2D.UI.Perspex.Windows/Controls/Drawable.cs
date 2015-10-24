// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Animation;
using Perspex.Collections;
using Perspex.Controls;
using Perspex.Controls.Primitives;
using Perspex.Controls.Shapes;
using Perspex.Controls.Templates;
using Perspex.Diagnostics;
using Perspex.Input;
using Perspex.Layout;
using Perspex.Media;
using Perspex.Media.Imaging;
using Core2D;

namespace TestPerspex
{
    /// <summary>
    /// 
    /// </summary>
    public class Drawable : Control
    {
        private ZoomState _state;

        /// <summary>
        /// 
        /// </summary>
        private void InitializeDrawable()
        {
            var context = this.DataContext as EditorContext;
            if (context == null)
                return;

            context.Editor.Invalidate = () => this.InvalidateVisual();

            _state = new ZoomState(context);

            if (context.Renderers[0].State.EnableAutofit)
            {
                AutoFit(this.Bounds.Width, this.Bounds.Height);
            }

            context.Commands.ZoomResetCommand =
                Command.Create(
                    () =>
                    {
                        ResetZoom(this.Bounds.Width, this.Bounds.Height);
                        context.Editor.Invalidate();
                    },
                    () => true);

            context.Commands.ZoomExtentCommand =
                Command.Create(
                    () =>
                    {
                        AutoFit(this.Bounds.Width, this.Bounds.Height);
                        context.Editor.Invalidate();
                    },
                    () => true);

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
                        this.Focus();
                        // TODO: this.Cursor = Cursors.Pointer;
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
                        this.Focus();
                        // TODO: this.Cursor = Cursors.Default;
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

                    var p = e.GetPosition(this);
                    _state.Wheel(p.X, p.Y, e.Delta.Y);
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var context = this.DataContext as EditorContext;
            if (context != null
                && context.Editor != null
                && context.Editor.Project != null)
            {
                if (context.Renderers[0].State.EnableAutofit)
                {
                    AutoFit(finalSize.Width, finalSize.Height);
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResetZoom(double width, double height)
        {
            if (_state == null)
                return;

            var context = this.DataContext as EditorContext;
            if (context == null
                || context.Editor == null
                || context.Editor.Project == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            _state.ResetZoom(
                width,
                height,
                container.Width,
                container.Height);

            context.Editor.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AutoFit(double width, double height)
        {
            if (_state == null)
                return;

            var context = this.DataContext as EditorContext;
            if (context == null
                || context.Editor == null
                || context.Editor.Project == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            _state.AutoFit(
                width,
                height,
                container.Width,
                container.Height);

            context.Editor.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void DrawBackground(DrawingContext dc, ArgbColor c, double width, double height)
        {
            var color = Color.FromArgb(
                (byte)c.A,
                (byte)c.R,
                (byte)c.G,
                (byte)c.B);
            var brush = new SolidColorBrush(color);
            var rect = new Rect(0, 0, width, height);
            dc.FillRectangle(brush, rect);
            // TODO: brush.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        private void Draw(DrawingContext dc)
        {
            var context = this.DataContext as EditorContext;
            if (context == null
                || context.Editor == null
                || context.Editor.Project == null)
                return;

            // TODO: Disable anti-aliasing.

            var translate = dc.PushPreTransform(Matrix.CreateTranslation(_state.PanX, _state.PanY));
            var scale = dc.PushPreTransform(Matrix.CreateScale(_state.Zoom, _state.Zoom));

            var renderer = context.Editor.Renderers[0];

            if (context.Editor.Project == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            if (container.Template != null)
            {
                DrawBackground(
                    dc,
                    container.Template.Background,
                    container.Template.Width,
                    container.Template.Height);

                renderer.Draw(
                    dc,
                    container.Template,
                    container.Properties,
                    null);
            }

            DrawBackground(
                dc,
                container.Background,
                container.Width,
                container.Height);

            renderer.Draw(
                dc,
                container,
                container.Properties,
                null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(
                    dc,
                    container.WorkingLayer,
                    container.Properties,
                    null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(
                    dc,
                    container.HelperLayer,
                    container.Properties,
                    null);
            }

            scale.Dispose();
            translate.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_state == null)
            {
                InitializeDrawable();
            }

            Draw(context);
        }
    }
}
