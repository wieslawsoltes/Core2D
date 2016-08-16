// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Point"/> editor.
    /// </summary>
    public class ToolPoint : ToolBase
    {
        private ToolState _currentState = ToolState.None;
        private XPoint _point;

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
                        _point = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);

                        if (Editor.Project.Options.TryToConnect)
                        {
                            if (!Editor.TryToSplitLine(x, y, _point, true))
                            {
                                Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _point);
                            }
                        }
                        else
                        {
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _point);
                        }
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
            }
        }
    }
}
