// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Test2d;

namespace TestWinForms
{
    /// <summary>
    /// 
    /// </summary>
    internal class ContainerPanel : Panel
    {
        private ZoomState _state;

        /// <summary>
        /// 
        /// </summary>
        public EditorContext Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action InvalidateContainer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ContainerPanel()
        {
        }

        /// <summary>
        /// 
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

            _state = new ZoomState(Context, InvalidateContainer);

            this.MouseDown +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Middle)
                    {
                        _state.MiddleDown(p.X, p.Y);
                        this.Cursor = Cursors.Hand;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.PrimaryDown(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        _state.AlternateDown(p.X, p.Y);
                    }
                };

            this.MouseUp +=
                (sender, e) =>
                {
                    var p = e.Location;

                    if (e.Button == MouseButtons.Middle)
                    {
                        this.Focus();
                        _state.MiddleUp(p.X, p.Y);
                        this.Cursor = Cursors.Default;
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Focus();
                        _state.PrimaryUp(p.X, p.Y);
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.Focus();
                        _state.AlternateUp(p.X, p.Y);
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
                    _state.Wheel(p.X, p.Y, e.Delta);
                };
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetZoom()
        {
            _state.ResetZoom();
            InvalidateContainer();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AutoFit()
        {
            // TODO: Autofit panel.
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Context != null)
            {
                Draw(e.Graphics);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Background(Graphics g, ArgbColor c, double width, double height)
        {
            var brush = new SolidBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
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
        /// 
        /// </summary>
        /// <param name="g"></param>
        private void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TranslateTransform(_state.PanX, _state.PanY);
            g.ScaleTransform(_state.Zoom, _state.Zoom);

            g.Clear(Color.FromArgb(255, 211, 211, 211));

            g.PageUnit = GraphicsUnit.Display;

            var renderer = Context.Editor.Renderers[0];
            var container = Context.Editor.Project.CurrentContainer;

            if (container.Template != null)
            {
                Background(g, container.Template.Background, this.Width, this.Height);
                renderer.Draw(g, container.Template, container.Properties, null);
            }

            Background(g, container.Background, this.Width, this.Height);
            renderer.Draw(g, container, container.Properties, null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(g, container.WorkingLayer, container.Properties, null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(g, container.HelperLayer, container.Properties, null);
            }
        }
    }
}
