// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// The zoon state object used for pan and zoom transformations.
    /// </summary>
    public class ZoomState
    {
        private Editor _editor;

        /// <summary>
        /// The minimum zoom value that can be set.
        /// </summary>
        public double MinimumZoom = 0.01;

        /// <summary>
        /// The maximum zoom value that can be set.
        /// </summary>
        public double MaximumZoom = 1000.0;

        /// <summary>
        /// The speed of zoom change.
        /// </summary>
        public double ZoomSpeed = 3.5;

        /// <summary>
        /// The current zoom value.
        /// </summary>
        public double Zoom = 1.0;

        /// <summary>
        /// The X coordinate of current pan position.
        /// </summary>
        public double PanX = 0.0;

        /// <summary>
        /// The Y coordinate of current pan position.
        /// </summary>
        public double PanY = 0.0;

        /// <summary>
        /// Flag indicating current pan state.
        /// </summary>
        public bool IsPanMode = false;

        /// <summary>
        /// The X coordinate of pan start position.
        /// </summary>
        public double StartX = 0.0;

        /// <summary>
        /// The Y coordinate of pan start position.
        /// </summary>
        public double StartY = 0.0;

        /// <summary>
        /// The Y coordinate of previous pan position.
        /// </summary>
        public double OriginX = 0.0;

        /// <summary>
        /// The X coordinate of previous pan position.
        /// </summary>
        public double OriginY = 0.0;

        /// <summary>
        /// Initialize new instance of <see cref="ZoomState"/> class.
        /// </summary>
        /// <param name="editor">The current editor object.</param>
        public ZoomState(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// Set renderer zoom.
        /// </summary>
        /// <param name="zoom">The zoom value.</param>
        private void SetRendererZoom(double zoom)
        {
            if (_editor == null)
                return;

            _editor.Renderers[0].State.Zoom = zoom;
        }
        
        /// <summary>
        /// Set renderer pan position.
        /// </summary>
        /// <param name="panX">The X coordinate of pan position.</param>
        /// <param name="panY">The Y coordinate of pan position.</param>
        private void SetRendererPan(double panX, double panY)
        {
            if (_editor == null)
                return;
            
            _editor.Renderers[0].State.PanX = panX;
            _editor.Renderers[0].State.PanY = panY;
        }
        
        /// <summary>
        /// Handle mouse right left button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftDown(double x, double y)
        {
            if (_editor == null)
                return;
            
            if (_editor.IsLeftDownAvailable())
            {
                _editor.LeftDown((x - PanX) / Zoom, (y - PanY) / Zoom);
            }
        }

        /// <summary>
        /// Handle mouse right left button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void LeftUp(double x, double y)
        {
            if (_editor == null)
                return;
            
            if (_editor.IsLeftUpAvailable())
            {
                var cx = (x - PanX) / Zoom;
                var cy = (y - PanY) / Zoom;
                _editor.LeftUp(cx, cy);
            }
        }

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightDown(double x, double y)
        {
            if (_editor == null)
                return;
            
            if (!_editor.CancelAvailable)
            {
                StartX = x;
                StartY = y;
                OriginX = PanX;
                OriginY = PanY;
                IsPanMode = true;
            }
            else if (_editor.IsRightDownAvailable())
            {
                var cx = (x - PanX) / Zoom;
                var cy = (y - PanY) / Zoom;
                _editor.RightDown(cx, cy);
            }
        }

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void RightUp(double x, double y)
        {
            if (_editor == null)
                return;
            
            if (!_editor.CancelAvailable)
            {
                IsPanMode = false;
            }
            else if (_editor.IsRightUpAvailable())
            {
                var cx = (x - PanX) / Zoom;
                var cy = (y - PanY) / Zoom;
                _editor.RightUp(cx, cy);
            }
        }

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public void Move(double x, double y)
        {
            if (_editor == null)
                return;
            
            if (IsPanMode)
            {
                double vx = StartX - x;
                double vy = StartY - y;
                PanX = OriginX - vx;
                PanY = OriginY - vy;
                SetRendererPan(PanX, PanY);
                _editor.Invalidate();
            }
            else
            {
                if (_editor.IsMoveAvailable())
                {
                    var cx = (x - PanX) / Zoom;
                    var cy = (y - PanY) / Zoom;
                    _editor.Move(cx, cy);
                }
            }
        }

        /// <summary>
        /// Handle mouse wheel events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="delta">The mouse wheel delta change.</param>
        /// <param name="pwidth">The parent panel width.</param>
        /// <param name="pheight">The parent panel height.</param>
        /// <param name="cwidth">The container width.</param>
        /// <param name="cheight">The container height.</param>
        public void Wheel(double x, double y, double delta, double pwidth, double pheight, double cwidth, double cheight)
        {
            if (_editor == null)
                return;
            
            double zoom = Zoom;
            zoom = delta > 0.0 ? zoom + zoom / ZoomSpeed : zoom - zoom / ZoomSpeed;
            if (zoom < MinimumZoom || zoom > MaximumZoom)
                return;

            ZoomTo(zoom, x, y, pwidth, pheight, cwidth, cheight);

            _editor.Invalidate();
        }

        /// <summary>
        /// Zoom to relative point.
        /// </summary>
        /// <param name="zoom">The zoom value.</param>
        /// <param name="x">The X coordinate of point to zoom.</param>
        /// <param name="y">The Y coordinate of point to zoom.</param>
        /// <param name="pwidth">The parent panel width.</param>
        /// <param name="pheight">The parent panel height.</param>
        /// <param name="cwidth">The container width.</param>
        /// <param name="cheight">The container height.</param>
        public void ZoomTo(double zoom, double x, double y, double pwidth, double pheight, double cwidth, double cheight)
        {
            double px = (pwidth - cwidth) / 2.0;
            double py = (pheight - cheight) / 2.0;
            
            double cx = x - px;
            double cy = y - py;
            
            double ax = (cx * Zoom) + PanX;
            double ay = (cy * Zoom) + PanY;
            
            Zoom = zoom;
            
            PanX = ax - (cx * Zoom);
            PanY = ay - (cy * Zoom);
            
            SetRendererZoom(Zoom);
            SetRendererPan(PanX, PanY);
        }

        /// <summary>
        /// Reset container zoom and center container in parent panel.
        /// </summary>
        /// <param name="pwidth">The parent panel width.</param>
        /// <param name="pheight">The parent panel height.</param>
        /// <param name="cwidth">The container width.</param>
        /// <param name="cheight">The container height.</param>
        public void CenterTo(double pwidth, double pheight, double cwidth, double cheight)
        {
            double px = (pwidth - cwidth) / 2.0;
            double py = (pheight - cheight) / 2.0;
            Zoom = 1.0;
            PanX = px;
            PanY = py;
            SetRendererZoom(Zoom);
            SetRendererPan(PanX, PanY);
        }

        /// <summary>
        /// Autofit container in parent panel.
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
            Zoom = zoom;
            PanX = px;
            PanY = py;
            SetRendererZoom(Zoom);
            SetRendererPan(PanX, PanY);
        }

        /// <summary>
        /// Reset container pan and zoom.
        /// </summary>
        public void Reset()
        {
            Zoom = 1.0;
            PanX = 0.0;
            PanY = 0.0;
            SetRendererZoom(Zoom);
            SetRendererPan(PanX, PanY);
        }
    }
}
