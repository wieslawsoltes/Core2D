// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Line tool.
    /// </summary>
    public class ToolLine : ObservableObject, IEditorTool
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsLine _settings;
        private State _currentState = State.Start;
        private ILineShape _line;
        private ToolLineSelection _selection;

        /// <inheritdoc/>
        public string Title => "Line";

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
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double x, double y) = args;
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _line = factory.CreateLineShape(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? (IShapeStyle)style.Copy(null) : style,
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
                        editor.IsToolIdle = false;
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
                            Finalize(_line);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _line);

                            Reset();
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void LeftUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void RightDown(InputArgs args)
        {
            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    Reset();
                    break;
            }
        }

        /// <inheritdoc/>
        public void RightUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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
            var editor = _serviceProvider.GetService<IProjectEditor>();
            _selection = new ToolLineSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateEnd();
        }

        /// <inheritdoc/>
        public void Move(IBaseShape shape)
        {
            _selection.Move();
        }

        /// <inheritdoc/>
        public void Finalize(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                    }
                    break;
            }

            _currentState = State.Start;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
