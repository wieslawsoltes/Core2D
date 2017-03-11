// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Text tool.
    /// </summary>
    public class ToolText : ToolBase
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsText _settings;
        private State _currentState = State.TopLeft;
        private XText _text;
        private ToolTextSelection _selection;

        /// <inheritdoc/>
        public override string Name => "Text";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsText Settings
        {
            get { return _settings; }
            set { Update(ref _settings, value); }
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolText"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolText(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsText();
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _text = XText.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            "Text",
                            editor.Project.Options.DefaultIsStroked);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _text.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_text);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateBottomRight();
                        Move(_text);
                        _currentState = State.BottomRight;
                        editor.CancelAvailable = true;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text != null)
                        {
                            _text.BottomRight.X = sx;
                            _text.BottomRight.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _text.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                            Remove();
                            Finalize(_text);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _text);
                            _currentState = State.TopLeft;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = State.TopLeft;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.TopLeft:
                    if (editor.Project.Options.TryToConnect)
                    {
                        editor.TryToHoverShape(sx, sy);
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _text.BottomRight.X = sx;
                            _text.BottomRight.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_text);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.BottomRight"/>.
        /// </summary>
        public void ToStateBottomRight()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolTextSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _text,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateBottomRight();
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
