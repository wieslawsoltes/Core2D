// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Line tool.
    /// </summary>
    public class ToolLine : ToolBase
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsLine _settings;
        private State _currentState = State.Start;
        private LineShape _line;
        private ToolLineSelection _selection;

        /// <inheritdoc/>
        public override string Title => "Line";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsLine Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolLine"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolLine(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsLine();
        }

        /// <inheritdoc/>
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double x, double y) = args;
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _line = LineShape.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            editor.Project.Options.DefaultIsStroked);
                        if (editor.Project.Options.TryToConnect)
                        {
                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _line.Start = result;
                            }
                            else
                            {
                                editor.TryToSplitLine(x, y, _line.Start);
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_line);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateEnd();
                        Move(_line);
                        _currentState = State.End;
                        editor.CancelAvailable = true;
                    }
                    break;
                case State.End:
                    {
                        if (_line != null)
                        {
                            _line.End.X = sx;
                            _line.End.Y = sy;
                            if (editor.Project.Options.TryToConnect)
                            {
                                var result = editor.TryToGetConnectionPoint(sx, sy);
                                if (result != null)
                                {
                                    _line.End = result;
                                }
                                else
                                {
                                    editor.TryToSplitLine(x, y, _line.End);
                                }
                            }
                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                            Remove();
                            base.Finalize(_line);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _line);
                            _currentState = State.Start;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = State.Start;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(InputArgs args)
        {
            base.Move(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.End:
                    {
                        if (_line != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _line.End.X = sx;
                            _line.End.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_line);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.End"/>.
        /// </summary>
        public void ToStateEnd()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolLineSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateEnd();
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
