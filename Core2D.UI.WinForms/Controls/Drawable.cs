// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Core2D.UI.WinForms
{
    /// <summary>
    /// Custom drawable control based on <see cref="Panel"/> control.
    /// </summary>
    internal class Drawable : Panel
    {
        private ZoomState _state;

        /// <summary>
        /// The <see cref="EditorContext"/> object.
        /// </summary>
        public EditorContext Context { get; set; }

        /// <summary>
        /// Initialize the <see cref="Drawable"/> panel.
        /// </summary>
        public void Initialize()
        {
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.SupportsTransparentBackColor,
                true);

            this.BackColor = Color.Transparent;

            _state = new ZoomState(Context.Editor);

            this.MouseDown +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.LeftDown(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        this.Cursor = Cursors.Hand;
                        _state.RightDown(p.X, p.Y);
                    }
                };

            this.MouseUp +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.LeftUp(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        this.Cursor = Cursors.Default;
                        _state.RightUp(p.X, p.Y);
                    }
                };

            this.MouseMove +=
                (sender, e) =>
                {
                    var p = e.Location;
                    _state.Move(p.X, p.Y);
                };

            this.MouseWheel +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (Context == null || Context.Editor.Project == null)
                        return;

                    var container = Context.Editor.Project.CurrentContainer;
                    _state.Wheel(
                        p.X,
                        p.Y, e.Delta,
                        this.Width,
                        this.Height,
                        container.Template.Width,
                        container.Template.Height);
                };
        }

        /// <summary>
        /// Reset pan and zoom to default state.
        /// </summary>
        public void ResetZoom()
        {
            if (Context != null && Context.Editor.Project != null)
            {
                var container = Context.Editor.Project.CurrentContainer;
                _state.CenterTo(
                    this.Width,
                    this.Height,
                    container.Template.Width,
                    container.Template.Height);
            }
        }

        /// <summary>
        /// Stretch view to the available extents.
        /// </summary>
        public void AutoFit()
        {
            if (Context != null && Context.Editor.Project != null)
            {
                var container = Context.Editor.Project.CurrentContainer;
                _state.FitTo(
                    this.Width,
                    this.Height,
                    container.Template.Width,
                    container.Template.Height);
            }
        }

        /// <summary>
        /// Set <see cref="CreateParams"/> to enable <see cref="Panel"/> transparency.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        /// <summary>
        /// Handle OnPaintBackground events.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs"/> args.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// Handle OnSizeChanged events.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> args.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Context != null && Context.Renderers != null && Context.Editor.Project != null)
            {
                if (Context.Renderers[0].State.EnableAutofit)
                {
                    AutoFit();
                }
            }
        }

        /// <summary>
        /// Handle OnPaint events.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs"/> args.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        /// <summary>
        /// Draws background rectangle with specified color.
        /// </summary>
        /// <param name="g">The drawing context.</param>
        /// <param name="c">The backgroud color.</param>
        /// <param name="width">The width of background rectangle.</param>
        /// <param name="height">The height of background rectangle.</param>
        private void DrawBackground(Graphics g, ArgbColor c, double width, double height)
        {
            var brush = new SolidBrush(
                Color.FromArgb(
                    c.A,
                    c.R,
                    c.G,
                    c.B));
            var rect = Rect2.Create(0, 0, width, height);
            g.FillRectangle(
                brush,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
            brush.Dispose();
        }

        /// <summary>
        /// Renders drawable control contents.
        /// </summary>
        /// <param name="g">The drawing context.</param>
        private void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PageUnit = GraphicsUnit.Display;

            g.Clear(Color.FromArgb(255, 211, 211, 211));

            if (Context == null || Context.Editor.Project == null)
                return;

            var container = Context.Editor.Project.CurrentContainer;
            var renderer = Context.Editor.Renderers[0];

            var gs = g.Save();

            g.TranslateTransform((float)_state.PanX, (float)_state.PanY);
            g.ScaleTransform((float)_state.Zoom, (float)_state.Zoom);

            var template = container.Template;
            if (template != null)
            {
                DrawBackground(g, template.Background, template.Width, template.Height);
                renderer.Draw(g, template, container.Data.Properties, null);
            }
            else
            {
                DrawBackground(g, container.Background, container.Width, container.Height);
            }

            renderer.Draw(g, container, container.Data.Properties, null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(g, container.WorkingLayer, container.Data.Properties, null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(g, container.HelperLayer, container.Data.Properties, null);
            }

            g.Restore(gs);
        }
    }
}
