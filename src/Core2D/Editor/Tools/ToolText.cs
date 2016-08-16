// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Text"/> editor.
    /// </summary>
    public class ToolText : ToolBase
    {
        private ToolState _currentState = ToolState.None;
        private XText _text;
        private TextSelection _selection;

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
                        _text = XText.Create(
                            sx, sy,
                            Editor.Project.Options.CloneStyle ? style.Clone() : style,
                            Editor.Project.Options.PointShape,
                            "Text",
                            Editor.Project.Options.DefaultIsStroked);

                        var result = Editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _text.TopLeft = result;
                        }

                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_text);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_text);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_text != null)
                        {
                            _text.BottomRight.X = sx;
                            _text.BottomRight.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _text.BottomRight = result;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                            Remove();
                            Finalize(_text);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _text);
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
                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
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
                    if (Editor.Project.Options.TryToConnect)
                    {
                        Editor.TryToHoverShape(sx, sy);
                    }
                    break;
                case ToolState.One:
                    {
                        if (_text != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _text.BottomRight.X = sx;
                            _text.BottomRight.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_text);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new TextSelection(
                Editor.Project.CurrentContainer.HelperLayer,
                _text,
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
