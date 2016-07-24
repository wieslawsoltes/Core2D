// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Editor.Bounds;
using Core2D.Math;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Selection"/> editor.
    /// </summary>
    public class ToolSelection : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private XRectangle _rectangle;
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;
        private IEnumerable<XPoint> _pointsCache;
        private IEnumerable<BaseShape> _shapesCache;

        /// <summary>
        /// Initialize new instance of <see cref="ToolSelection"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolSelection(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <summary>
        /// Generate selected shapes cache.
        /// </summary>
        private void GenerateMoveSelectionCache()
        {
            if (_editor.Renderers[0].State.SelectedShape != null)
            {
                var state = _editor.Renderers[0].State.SelectedShape.State;

                switch (_editor.Project.Options.MoveMode)
                {
                    case XMoveMode.Point:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var shape = _editor.Renderers[0].State.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1);
                                _pointsCache = shapes.SelectMany(s => s.GetPoints()).Distinct().ToList();
                            }
                        }
                        break;
                    case XMoveMode.Shape:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked) && !state.Flags.HasFlag(ShapeStateFlags.Connector))
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
                var shapes = _editor.Renderers[0].State.SelectedShapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));

                switch (_editor.Project.Options.MoveMode)
                {
                    case XMoveMode.Point:
                        {
                            _pointsCache = shapes.SelectMany(s => s.GetPoints()).Distinct().ToList();
                        }
                        break;
                    case XMoveMode.Shape:
                        {
                            _shapesCache = shapes.ToList();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Dispose selected shapes cache.
        /// </summary>
        private void DisposeMoveSelectionCache()
        {
            _pointsCache = null;
            _shapesCache = null;
        }

        /// <summary>
        /// Move selected shapes to new location.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        private void MoveSelectionCacheTo(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;

            double dx = sx - _startX;
            double dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            if (_pointsCache != null)
            {
                ProjectEditor.MoveShapesBy(_pointsCache, dx, dy);
            }

            if (_shapesCache != null)
            {
                ProjectEditor.MoveShapesBy(_shapesCache, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _editor.Dehover(_editor.Project.CurrentContainer.CurrentLayer);
                        if (_editor.Renderers[0].State.SelectedShape == null
                            && _editor.Renderers[0].State.SelectedShapes != null)
                        {
                            var result = ShapeHitTestPoint.HitTest(_editor.Project.CurrentContainer.CurrentLayer.Shapes, new Vector2(x, y), _editor.Project.Options.HitThreshold);
                            if (result != null)
                            {
                                _startX = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
                                _startY = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
                                _historyX = _startX;
                                _historyY = _startY;
                                GenerateMoveSelectionCache();
                                _currentState = ToolState.One;
                                _editor.CancelAvailable = true;
                                break;
                            }
                        }

                        if (_editor.TryToSelectShape(_editor.Project.CurrentContainer.CurrentLayer, x, y))
                        {
                            _startX = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
                            _startY = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
                            _historyX = _startX;
                            _historyY = _startY;
                            GenerateMoveSelectionCache();
                            _currentState = ToolState.One;
                            _editor.CancelAvailable = true;
                            break;
                        }

                        _rectangle = XRectangle.Create(
                            x, y,
                            _editor.Project.Options.SelectionStyle,
                            null,
                            true, true);
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _currentState = ToolState.None;
                            _editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void LeftUp(double x, double y)
        {
            base.LeftUp(x, y);

            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                    {
                        if (_editor.IsSelectionAvailable())
                        {
                            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
                            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
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
                                _editor.Project?.History?.Snapshot(previous, next,
                                    (state) =>
                                    {
                                        if (state.Points != null)
                                        {
                                            ProjectEditor.MoveShapesBy(state.Points, state.DeltaX, state.DeltaY);
                                        }

                                        if (state.Shapes != null)
                                        {
                                            ProjectEditor.MoveShapesBy(state.Shapes, state.DeltaX, state.DeltaY);
                                        }
                                    });
                            }

                            DisposeMoveSelectionCache();
                            _currentState = ToolState.None;
                            _editor.CancelAvailable = false;
                            break;
                        }

                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _currentState = ToolState.None;
                            _editor.TryToSelectShapes(_editor.Project.CurrentContainer.CurrentLayer, _rectangle);
                            _editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _editor.Dehover(_editor.Project.CurrentContainer.CurrentLayer);
                    }
                    break;
                case ToolState.One:
                    {
                        DisposeMoveSelectionCache();
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _editor.TryToHoverShape(x, y);
                    }
                    break;
                case ToolState.One:
                    {
                        if (_editor.IsSelectionAvailable())
                        {
                            MoveSelectionCacheTo(x, y);
                            _editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                            break;
                        }

                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
        }
    }
}
