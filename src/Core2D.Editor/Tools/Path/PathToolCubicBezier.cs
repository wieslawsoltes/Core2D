// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Collections.Generic;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Editor.Tools.Selection;
using Core2D.Interfaces;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Cubic bezier path tool.
    /// </summary>
    public class PathToolCubicBezier : PathToolBase
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private PathToolSettingsCubicBezier _settings;
        private State _currentState = State.Point1;
        private CubicBezierShape _cubicBezier = new CubicBezierShape();
        private ToolCubicBezierSelection _selection;

        /// <inheritdoc/>
        public override string Title => "CubicBezier";

        /// <summary>
        /// Gets or sets the path tool settings.
        /// </summary>
        public PathToolSettingsCubicBezier Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathToolCubicBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolCubicBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new PathToolSettingsCubicBezier();
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
                case State.Point1:
                    {
                        _cubicBezier.Point1 = editor.TryToGetConnectionPoint(sx, sy) ?? factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_cubicBezier.Point1);
                        }
                        else
                        {
                            _cubicBezier.Point1 = pathTool.GetLastPathPoint();
                        }

                        _cubicBezier.Point2 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStatePoint4();
                        Move(null);
                        _currentState = State.Point4;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Point4:
                    {
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point3 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezier.Point4 = point3;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStatePoint2();
                        Move(null);
                        _currentState = State.Point2;
                    }
                    break;
                case State.Point2:
                    {
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezier.Point2 = point1;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _currentState = State.Point3;
                    }
                    break;
                case State.Point3:
                    {
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezier.Point3 = point2;
                            }
                        }

                        _cubicBezier.Point1 = _cubicBezier.Point4;
                        _cubicBezier.Point2 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStatePoint4();
                        Move(null);
                        _currentState = State.Point4;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    Reset();
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
                case State.Point1:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case State.Point2:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case State.Point3:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point4"/>.
        /// </summary>
        public void ToStatePoint4()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection?.Reset();
            _selection = new ToolCubicBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);
            _selection.ToStatePoint4();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point2"/>.
        /// </summary>
        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point3"/>.
        /// </summary>
        public void ToStateThree()
        {
            _selection.ToStatePoint3();
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
        public override void Reset()
        {
            base.Reset();

            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    {
                        pathTool.RemoveLastSegment<CubicBezierSegment>();
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(pathTool.Path);
                        if (pathTool.Path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, pathTool.Path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }

            _currentState = State.Point1;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
