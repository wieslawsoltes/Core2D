// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Tools.Selection;
using Core2D.Math.Arc;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Arc"/> editor.
    /// </summary>
    public class ToolArc : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private XArc _shape;
        private bool _connectedPoint3;
        private bool _connectedPoint4;
        private ArcSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolArc"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolArc(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        var style = _editor.Project.CurrentStyleLibrary.Selected;
                        _connectedPoint3 = false;
                        _connectedPoint4 = false;
                        _shape = XArc.Create(
                            sx, sy,
                            _editor.Project.Options.CloneStyle ? style.Clone() : style,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);

                        var result = _editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _shape.Point1 = result;
                        }

                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_shape != null)
                        {
                            _shape.Point2.X = sx;
                            _shape.Point2.Y = sy;
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point2 = result;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_shape != null)
                        {
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point3 = result;
                                _connectedPoint3 = true;
                            }
                            else
                            {
                                _connectedPoint3 = false;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_shape != null)
                        {
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point4 = result;
                                _connectedPoint4 = true;
                            }
                            else
                            {
                                _connectedPoint4 = false;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _shape);
                            _currentState = ToolState.None;
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
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point2.X = sx;
                            _shape.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new ArcSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _shape,
                _editor.Project.Options.HelperStyle,
                _editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _selection.ToStateTwo();
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _selection.ToStateThree();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);

            var arc = shape as XArc;
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (!_connectedPoint3)
            {
                arc.Point3.X = a.Start.X;
                arc.Point3.Y = a.Start.Y;
            }

            if (!_connectedPoint4)
            {
                arc.Point4.X = a.End.X;
                arc.Point4.Y = a.End.Y;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _selection.Remove();
            _selection = null;
        }
    }
}
