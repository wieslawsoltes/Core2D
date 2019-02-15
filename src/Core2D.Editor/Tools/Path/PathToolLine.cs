// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Collections.Generic;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Editor.Tools.Selection;
using Core2D.Interfaces;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Line path tool.
    /// </summary>
    public class PathToolLine : PathToolBase
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private PathToolSettingsLine _settings;
        private State _currentState = State.Start;
        private PathShapeLine _line = new PathShapeLine();
        private ToolLineSelection _selection;

        /// <inheritdoc/>
        public override string Title => "Line";

        /// <summary>
        /// Gets or sets the path tool settings.
        /// </summary>
        public PathToolSettingsLine Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathToolLine"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolLine(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new PathToolSettingsLine();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        _line.Start = editor.TryToGetConnectionPoint(sx, sy) ?? factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_line.Start);
                        }
                        else
                        {
                            _line.Start = pathTool.GetLastPathPoint();
                        }

                        _line.End = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.LineTo(
                            _line.End,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateEnd();
                        Move(null);
                        _currentState = State.End;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.End:
                    {
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint(sx, sy);
                            if (end != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as LineSegment;
                                line.Point = end;
                            }
                        }

                        _line.Start = _line.End;
                        _line.End = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.LineTo(_line.End,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _currentState = State.End;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    {
                        pathTool.RemoveLastSegment<LineSegment>();

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(pathTool.Path);
                        Remove();
                        if (pathTool.Path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, pathTool.Path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        pathTool.DeInitializeWorkingPath();
                        _currentState = State.Start;
                        editor.IsToolIdle = true;
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
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
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
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateEnd();
        }

        /// <inheritdoc/>
        public override void Move(IBaseShape shape)
        {
            base.Move(shape);

            if (_selection != null)
            {
                _selection.Move();
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _currentState = State.Start;

            if (_selection != null)
            {
                _selection.Remove();
                _selection = null;
            }
        }
    }
}
