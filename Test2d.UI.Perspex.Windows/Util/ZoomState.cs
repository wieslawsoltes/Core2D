﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ZoomState
    {
        private EditorContext _context;
        private Action _invalidate;

        /// <summary>
        /// 
        /// </summary>
        public double MinimumZoom = 0.01;

        /// <summary>
        /// 
        /// </summary>
        public double MaximumZoom = 1000.0;

        /// <summary>
        /// 
        /// </summary>
        public double ZoomSpeed = 3.5;

        /// <summary>
        /// 
        /// </summary>
        public double Zoom = 1.0;

        /// <summary>
        /// 
        /// </summary>
        public double PanX = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public double PanY = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public bool IsPanMode = false;

        /// <summary>
        /// 
        /// </summary>
        public double StartX = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public double StartY = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public double OriginX = 0.0;

        /// <summary>
        /// 
        /// </summary>
        public double OriginY = 0.0;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invalidate"></param>
        public ZoomState(EditorContext context, Action invalidate)
        {
            _context = context;
            _invalidate = invalidate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MiddleDown(double x, double y)
        {
            StartX = x;
            StartY = y;
            OriginX = PanX;
            OriginY = PanY;
            IsPanMode = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PrimaryDown(double x, double y)
        {
            if (_context.Editor.IsLeftDownAvailable())
            {
                _context.Editor.LeftDown(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AlternateDown(double x, double y)
        {
            if (_context.Editor.IsRightDownAvailable())
            {
                _context.Editor.RightDown(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MiddleUp(double x, double y)
        {
            IsPanMode = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PrimaryUp(double x, double y)
        {
            if (_context.Editor.IsLeftUpAvailable())
            {
                _context.Editor.LeftUp(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AlternateUp(double x, double y)
        {
            if (_context.Editor.IsRightUpAvailable())
            {
                _context.Editor.RightUp(
                    (x - PanX) / Zoom,
                    (y - PanY) / Zoom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(double x, double y)
        {
            if (IsPanMode)
            {
                double vx = StartX - x;
                double vy = StartY - y;
                PanX = OriginX - vx;
                PanY = OriginY - vy;
                _context.Editor.Renderers[0].State.PanX = PanX;
                _context.Editor.Renderers[0].State.PanY = PanY;
                _invalidate();
            }
            else
            {
                if (_context.Editor.IsMoveAvailable())
                {
                    _context.Editor.Move(
                        (x - PanX) / Zoom,
                        (y - PanY) / Zoom);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        public void Wheel(double x, double y, double delta)
        {
            double zoom = Zoom;
            zoom = delta > 0.0 ?
                zoom + zoom / ZoomSpeed :
                zoom - zoom / ZoomSpeed;

            if (zoom < MinimumZoom || zoom > MaximumZoom)
                return;

            ZoomTo(zoom, x, y);
            _invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        public void ZoomTo(double zoom, double rx, double ry)
        {
            double ax = (rx * Zoom) + PanX;
            double ay = (ry * Zoom) + PanY;
            Zoom = zoom;
            PanX = ax - (rx * Zoom);
            PanY = ay - (ry * Zoom);
            _context.Editor.Renderers[0].State.Zoom = Zoom;
            _context.Editor.Renderers[0].State.PanX = PanX;
            _context.Editor.Renderers[0].State.PanY = PanY;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetZoom()
        {
            Zoom = 1.0;
            PanX = 0.0;
            PanY = 0.0;
            _context.Editor.Renderers[0].State.Zoom = Zoom;
            _context.Editor.Renderers[0].State.PanX = PanX;
            _context.Editor.Renderers[0].State.PanY = PanY;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void AutoFit()
        {
            // TODO: Implement zoom auto-fit.
        }
    }
}
