// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Helper class for <see cref="Tool.Point"/> editor.
    /// </summary>
    public class ToolPoint : ToolBase
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XPoint _shape;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPoint"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="Editor"/> object.</param>
        public ToolPoint(Editor editor)
            : base()
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        _shape = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);

                        if (_editor.Project.Options.TryToConnect)
                        {
                            if (!_editor.TryToSplitLine(x, y, _shape, true))
                            {
                                _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _shape);
                            }
                        }
                        else
                        {
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _shape);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
            }
        }
    }
}
