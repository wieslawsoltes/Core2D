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
        private ToolState _currentState = ToolState.None;
        private XArc _arc;
        private bool _connectedPoint3;
        private bool _connectedPoint4;
        private ArcSelection _selection;

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        var style = Editor.Project.CurrentStyleLibrary.Selected;
                        _connectedPoint3 = false;
                        _connectedPoint4 = false;
                        _arc = XArc.Create(
                            sx, sy,
                            Editor.Project.Options.CloneStyle ? style.Clone() : style,
                            Editor.Project.Options.PointShape,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsFilled);

                        var result = Editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _arc.Point1 = result;
                        }

                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_arc);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_arc != null)
                        {
                            _arc.Point2.X = sx;
                            _arc.Point2.Y = sy;
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point2 = result;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_arc);
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_arc != null)
                        {
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point3 = result;
                                _connectedPoint3 = true;
                            }
                            else
                            {
                                _connectedPoint3 = false;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_arc);
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_arc);
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_arc != null)
                        {
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point4 = result;
                                _connectedPoint4 = true;
                            }
                            else
                            {
                                _connectedPoint4 = false;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                            Remove();
                            Finalize(_arc);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _arc);
                            _currentState = ToolState.None;
                            Editor.CancelAvailable = false;
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
                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = ToolState.None;
                        Editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (_arc != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point2.X = sx;
                            _arc.Point2.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_arc != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_arc != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
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
                Editor.Project.CurrentContainer.HelperLayer,
                _arc,
                Editor.Project.Options.HelperStyle,
                Editor.Project.Options.PointShape);

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
            var a = WpfArc.FromXArc(arc);

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
