// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Image"/> editor.
    /// </summary>
    public class ToolImage : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private XImage _image;
        private ImageSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolImage"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolImage(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override async void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (_editor.GetImageKey == null)
                            return;

                        var key = await _editor.GetImageKey();
                        if (key == null || string.IsNullOrEmpty(key))
                            return;

                        var style = _editor.Project.CurrentStyleLibrary.Selected;
                        _image = XImage.Create(
                            sx, sy,
                            _editor.Project.Options.CloneStyle ? style.Clone() : style,
                            _editor.Project.Options.PointShape,
                            key);

                        var result = _editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _image.TopLeft = result;
                        }

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_image);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_image);
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_image != null)
                        {
                            _image.BottomRight.X = sx;
                            _image.BottomRight.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _image.BottomRight = result;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                            Remove();
                            base.Finalize(_image);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _image);
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
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
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
                    if (_editor.Project.Options.TryToConnect)
                    {
                        _editor.TryToHoverShape(sx, sy);
                    }
                    break;
                case ToolState.One:
                    {
                        if (_image != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _image.BottomRight.X = sx;
                            _image.BottomRight.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_image);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new ImageSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _image,
                _editor.Project.Options.HelperStyle,
                _editor.Project.Options.PointShape);

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
