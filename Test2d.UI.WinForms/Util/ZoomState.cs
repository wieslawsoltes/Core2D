﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
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
        public float MinimumZoom = 0.01f;

        /// <summary>
        /// 
        /// </summary>
        public float MaximumZoom = 1000.0f;

        /// <summary>
        /// 
        /// </summary>
        public float ZoomSpeed = 3.5f;

        /// <summary>
        /// 
        /// </summary>
        public float Zoom = 1f;

        /// <summary>
        /// 
        /// </summary>
        public float PanX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float PanY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public bool IsPanMode = false;

        /// <summary>
        /// 
        /// </summary>
        public float PanOffsetX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float PanOffsetY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float OriginX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float OriginY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float StartX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float StartY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float WheelOriginX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float WheelOriginY = 0f;

        /// <summary>
        /// 
        /// </summary>
        public bool HaveWheelOrigin = false;

        /// <summary>
        /// 
        /// </summary>
        public float WheelOffsetX = 0f;

        /// <summary>
        /// 
        /// </summary>
        public float WheelOffsetY = 0f;

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
        public void MiddleDown(float x, float y)
        {
            Debug.Print("Pan Offset: {0}, {1}", PanOffsetX, PanOffsetY);

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
        public void PrimaryDown(float x, float y)
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
        public void AlternateDown(float x, float y)
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
        public void MiddleUp(float x, float y)
        {
            PanOffsetX += PanX - OriginX;
            PanOffsetY += PanY - OriginY;
            Debug.Print("Pan Offset: {0}, {1}", PanOffsetX, PanOffsetY);
            IsPanMode = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PrimaryUp(float x, float y)
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
        public void AlternateUp(float x, float y)
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
        public void Move(float x, float y)
        {
            if (IsPanMode)
            {
                float vx = StartX - x;
                float vy = StartY - y;

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
        public void Wheel(float x, float y, float delta)
        {
            float zoom = Zoom;
            zoom = delta > 0 ?
                zoom + zoom / ZoomSpeed :
                zoom - zoom / ZoomSpeed;

            if (zoom < MinimumZoom || zoom > MaximumZoom)
                return;

            if (!HaveWheelOrigin)
            {
                WheelOriginX = x;
                WheelOriginY = y;
                HaveWheelOrigin = true;
            }

            WheelOffsetX = x - WheelOriginX;
            WheelOffsetY = y - WheelOriginY;
            Debug.Print("Wheel Offset: {0}, {1}", WheelOffsetX, WheelOffsetY);

            ZoomTo(
                zoom,
                x - PanOffsetX - WheelOffsetX,
                y - PanOffsetY - WheelOffsetY);

            _invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        public void ZoomTo(float zoom, float rx, float ry)
        {
            float ax = (rx * Zoom) + PanX;
            float ay = (ry * Zoom) + PanY;

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
            Zoom = 1f;
            PanX = 0f;
            PanY = 0f;
            PanOffsetX = 0f;
            PanOffsetY = 0f;
            WheelOriginX = 0f;
            WheelOriginY = 0f;
            WheelOffsetX = 0f;
            WheelOffsetY = 0f;
            HaveWheelOrigin = false;

            _context.Editor.Renderers[0].State.Zoom = Zoom;
            _context.Editor.Renderers[0].State.PanX = PanX;
            _context.Editor.Renderers[0].State.PanY = PanY;
        }
    }
}
