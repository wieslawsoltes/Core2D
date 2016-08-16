// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Line"/> editor.
    /// </summary>
    public class ToolLine : ToolBase
    {
        private ToolState _currentState = ToolState.None;
        private XLine _line;
        private LineSelection _selection;

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
                        _line = XLine.Create(
                            sx, sy,
                            Editor.Project.Options.CloneStyle ? style.Clone() : style,
                            Editor.Project.Options.PointShape,
                            Editor.Project.Options.DefaultIsStroked);
                        if (Editor.Project.Options.TryToConnect)
                        {
                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _line.Start = result;
                            }
                            else
                            {
                                Editor.TryToSplitLine(x, y, _line.Start);
                            }
                        }
                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_line);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_line);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_line != null)
                        {
                            _line.End.X = sx;
                            _line.End.Y = sy;
                            if (Editor.Project.Options.TryToConnect)
                            {
                                var result = Editor.TryToGetConnectionPoint(sx, sy);
                                if (result != null)
                                {
                                    _line.End = result;
                                }
                                else
                                {
                                    Editor.TryToSplitLine(x, y, _line.End);
                                }
                            }
                            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                            Remove();
                            base.Finalize(_line);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _line);
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
                    {
                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
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
                        if (_line != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _line.End.X = sx;
                            _line.End.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_line);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new LineSelection(
                Editor.Project.CurrentContainer.HelperLayer,
                _line,
                Editor.Project.Options.HelperStyle,
                Editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
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
