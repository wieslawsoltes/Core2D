// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectionHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private BaseShape _shape;
        private BaseShape _previousHoverShape = default(BaseShape);
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public SelectionHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftDown(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    {
                        _previousHoverShape = default(BaseShape);

                        if (_editor.Renderer.SelectedShape == null
                            && _editor.Renderer.SelectedShapes != null)
                        {
                            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
                            if (result != null)
                            {
                                _startX = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                                _startY = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
                                _historyX = _startX;
                                _historyY = _startY;
                                // TODO: Add history hold.
                                _editor.IsContextMenu = false;
                                _currentState = State.One;
                                break;
                            }
                        }

                        if (_editor.TryToSelectShape(_editor.Project.CurrentContainer, x, y))
                        {
                            _startX = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                            _startY = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
                            _historyX = _startX;
                            _historyY = _startY;
                            // TODO: Add history hold.
                            _editor.IsContextMenu = false;
                            _currentState = State.One;
                            break;
                        }

                        _shape = XRectangle.Create(
                            x, y,
                            _editor.Project.Options.SelectionStyle,
                            null,
                            true);
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _currentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = x;
                            rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _currentState = State.None;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftUp(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        if (_editor.IsSelectionAvailable())
                        {
                            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
                            if (_historyX != sx || _historyY != sy)
                            {
                                // TODO: Enable history commit.
                                //_editor.History.Commit();
                            }
                            else
                            {
                                // TODO: Enable history release.
                                //_editor.History.Release();
                            }
                            _currentState = State.None;
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = x;
                            rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _currentState = State.None;

                            _editor.TryToSelectShapes(_editor.Project.CurrentContainer, rectangle);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightDown(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    {
                        _previousHoverShape = default(BaseShape);

                        _editor.IsContextMenu = _editor.TryToSelectShape(_editor.Project.CurrentContainer, x, y) ? true : false;
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightUp(double x, double y)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void MoveSelection(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

            double dx = sx - _startX;
            double dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            if (_editor.Renderer.SelectedShape != null)
            {
                switch (_editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!_editor.Renderer.SelectedShape.State.HasFlag(ShapeState.Locked))
                            {
                                var shapes = Enumerable.Repeat(_editor.Renderer.SelectedShape, 1);
                                var points = Editor.GetAllPoints(shapes, ShapeState.Connector).Distinct();
                                Editor.Move(points, dx, dy);
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!_editor.Renderer.SelectedShape.State.HasFlag(ShapeState.Locked)
                                && !_editor.Renderer.SelectedShape.State.HasFlag(ShapeState.Connector))
                            {
                                _editor.Renderer.SelectedShape.Move(dx, dy);
                            }
                        }
                        break;
                }
            }

            if (_editor.Renderer.SelectedShapes != null)
            {
                switch (_editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var shapes = _editor.Renderer.SelectedShapes.Where(s => !s.State.HasFlag(ShapeState.Locked));
                            var points = Editor.GetAllPoints(shapes, ShapeState.Connector).Distinct();
                            Editor.Move(points, dx, dy);
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            foreach (var shape in _editor.Renderer.SelectedShapes)
                            {
                                if (!shape.State.HasFlag(ShapeState.Locked))
                                {
                                    shape.Move(dx, dy);
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void TryToSelectHoverShape(double x, double y)
        {
            var renderer = _editor.Renderer;
            if (renderer.SelectedShapes == null
                && !(renderer.SelectedShape != null && _previousHoverShape != renderer.SelectedShape))
            {
                var result = ShapeBounds.HitTest(
                    _editor.Project.CurrentContainer,
                    new Vector2(x, y),
                    _editor.Project.Options.HitTreshold);
                if (result != null)
                {
                    _editor.Select(_editor.Project.CurrentContainer, result);
                    _previousHoverShape = result;
                }
                else
                {
                    if (_editor.Renderer.SelectedShape == _previousHoverShape)
                    {
                        _previousHoverShape = default(BaseShape);
                        _editor.Deselect(_editor.Project.CurrentContainer);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Move(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    {
                        TryToSelectHoverShape(x, y);
                    }
                    break;
                case State.One:
                    {
                        if (_editor.IsSelectionAvailable())
                        {
                            MoveSelection(x, y);
                            _editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                            break;
                        }

                        var rectangle = _shape as XRectangle;
                        if (rectangle != null)
                        {
                            rectangle.BottomRight.X = x;
                            rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateOne()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateFour()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Move(BaseShape shape)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Finalize(BaseShape shape)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Remove()
        {
        }
    }
}
