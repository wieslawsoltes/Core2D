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
using Test2d;

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

            context.Invalidate = 
                () =>
                {
                    InitializeLayers();
                    ResizeDrawable();

                    var container = context.Editor.Project.CurrentContainer;
                    if (container != null)
                    {
                        container.Invalidate();
                    }
                };
            
            _state = new ZoomState(context);

            context.Commands.ZoomResetCommand =
                Command.Create(
                    () => 
                    {
                        _state.ResetZoom();
                        if (context.Invalidate != null)
                        {
                            context.Invalidate();
                        }
                    },
                    () => true);

            context.Commands.ZoomExtentCommand =
                Command.Create(
                    () => 
                    {
                        _state.AutoFit();
                        if (context.Invalidate != null)
                        {
                            context.Invalidate();
                        }
                    },
                    () => true);

            this.PointerPressed +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;
                
                    var p = e.GetPosition(this);

                    if (e.MouseButton == MouseButton.Middle)
                    {
                        _state.MiddleDown(p.X, p.Y);
                        // TODO: this.Cursor = Cursors.Pointer;
                    }

                    if (e.MouseButton == MouseButton.Left)
                    {
                        this.Focus();
                        _state.PrimaryDown(p.X, p.Y);
                    }

                    if (e.MouseButton == MouseButton.Right)
                    {
                        this.Focus();
                        _state.AlternateDown(p.X, p.Y);
                    }
                };

            this.PointerReleased +=
                (sender, e) =>
                {
                    if (_state == null)
                        return;
                    
                    var p = e.GetPosition(this);

                    if (e.MouseButton == MouseButton.Middle)
                    {
                        this.Focus();
                        _state.MiddleUp(p.X, p.Y);
                        // TODO: this.Cursor = Cursors.Default;
                    }

                    if (e.MouseButton == MouseButton.Left)
                    {
                        this.Focus();
                        _state.PrimaryUp(p.X, p.Y);
                    }

                    if (e.MouseButton == MouseButton.Right)
                    {
                        this.Focus();
                        _state.AlternateUp(p.X, p.Y);
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
        private void ResizeDrawable()
        {
            var context = this.DataContext as EditorContext;
            if (context == null
                || context.Editor == null
                || context.Editor.Project == null)
                return;

            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            this.Width = container.Width;
            this.Height = container.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeLayers()
        {
            var context = this.DataContext as EditorContext;
            if (context == null 
                || context.Editor == null 
                || context.Editor.Project == null)
                return;
            
            var container = context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer +=
                    (s, e) =>
                    {
                        this.InvalidateVisual();
                    };
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                       this.InvalidateVisual();
                    };
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        this.InvalidateVisual();
                    };
            }
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
            var color = Color.FromArgb(c.A, c.R, c.G, c.B);
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
                    container.Width,
                    container.Height);

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
                InitializeLayers();
                ResizeDrawable();
            }

            Draw(context);
        }
    }
}
