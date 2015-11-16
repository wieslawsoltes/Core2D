// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Helper class for <see cref="Tool.Point"/> editor.
    /// </summary>
    public class PointHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XPoint _shape;

        /// <summary>
        /// Initialize new instance of <see cref="PointHelper"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="Editor"/> object.</param>
        public PointHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
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
                                _editor.AddShape(_shape);
                            }
                        }
                        else
                        {
                            _editor.AddShape(_shape);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void LeftUp(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void RightUp(double x, double y)
        {
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
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

        /// <inheritdoc/>
        public override void ToStateOne()
        {
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
        }

        /// <inheritdoc/>
        public override void ToStateFour()
        {
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public override void Remove()
        {
        }
    }
}
