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
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;
        private IEnumerable<XPoint> _pointsCache;
        private IEnumerable<BaseShape> _shapesCache;

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
                        _editor.Dehover();
                        if (_editor.Renderers[0].State.SelectedShape == null
                            && _editor.Renderers[0].State.SelectedShapes != null)
                        {
                            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
                            if (result != null)
                            {
                                _startX = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                                _startY = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
                                _historyX = _startX;
                                _historyY = _startY;
                                GenerateMoveSelectionCache();
                                _currentState = State.One;
                                _editor.CancelAvailable = true;
                                break;
                            }
                        }

                        if (_editor.TryToSelectShape(_editor.Project.CurrentContainer, x, y))
                        {
                            _startX = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                            _startY = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
                            _historyX = _startX;
                            _historyY = _startY;
                            GenerateMoveSelectionCache();
                            _currentState = State.One;
                            _editor.CancelAvailable = true;
                            break;
                        }

                        _shape = XRectangle.Create(
                            x, y,
                            _editor.Project.Options.SelectionStyle,
                            null,
                            true, true);
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
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
                            _editor.CancelAvailable = false;
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
                                double dx = sx - _historyX;
                                double dy = sy - _historyY;

                                var previous = new 
                                { 
                                    DeltaX = -dx, 
                                    DeltaY = -dy, 
                                    Points = _pointsCache, 
                                    Shapes = _shapesCache 
                                };
                                var next = new
                                {
                                    DeltaX = dx,
                                    DeltaY = dy,
                                    Points = _pointsCache,
                                    Shapes = _shapesCache
                                };
                                _editor.History.Snapshot(previous, next, 
                                    (state) => 
                                    {
                                        if (state.Points != null)
                                        {
                                            Editor.MovePointsBy(state.Points, state.DeltaX, state.DeltaY);
                                        }

                                        if (state.Shapes != null)
                                        {
                                            Editor.MoveShapesBy(state.Shapes, state.DeltaX, state.DeltaY);
                                        }
                                    });
                            }
                            DisposeMoveSelectionCache();
                            _currentState = State.None;
                            _editor.CancelAvailable = false;
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
                            _editor.CancelAvailable = false;
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
                        _editor.Dehover();
                    }
                    break;
                case State.One:
                    {
                        DisposeMoveSelectionCache();
                        _editor.CancelAvailable = false;
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
        private void GenerateMoveSelectionCache()
        {
            if (_editor.Renderers[0].State.SelectedShape != null)
            {
                var state = _editor.Renderers[0].State.SelectedShape.State;

                switch (_editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!state.HasFlag(ShapeState.Locked))
                            {
                                var shape = _editor.Renderers[0].State.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1);
                                _pointsCache = Editor.GetAllPoints(shapes, ShapeState.Connector).Distinct().ToList();
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!state.HasFlag(ShapeState.Locked) && !state.HasFlag(ShapeState.Connector))
                            {
                                var shape = _editor.Renderers[0].State.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1).ToList();
                                _shapesCache = shapes;
                            }
                        }
                        break;
                }
            }

            if (_editor.Renderers[0].State.SelectedShapes != null)
            {
                var shapes = _editor.Renderers[0].State.SelectedShapes.Where(s => !s.State.HasFlag(ShapeState.Locked));

                switch (_editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            _pointsCache = Editor.GetAllPoints(shapes, ShapeState.Connector).Distinct().ToList();
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            _shapesCache = shapes.ToList();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisposeMoveSelectionCache()
        {
            _pointsCache = null;
            _shapesCache = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void MoveSelectionTo(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

            double dx = sx - _startX;
            double dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            if (_pointsCache != null)
            {
                Editor.MovePointsBy(_pointsCache, dx, dy);
            }

            if (_shapesCache != null)
            {
                Editor.MoveShapesBy(_shapesCache, dx, dy);
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
                        _editor.TryToHoverShape(x, y);
                    }
                    break;
                case State.One:
                    {
                        if (_editor.IsSelectionAvailable())
                        {
                            MoveSelectionTo(x, y);
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
