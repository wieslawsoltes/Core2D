using System;
using System.Collections.Generic;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Quadratic bezier path tool.
    /// </summary>
    public class PathToolQuadraticBezier : ObservableObject, IPathTool
    {
        public enum State { Point1, Point3, Point2 }
        private readonly IServiceProvider _serviceProvider;
        private PathToolSettingsQuadraticBezier _settings;
        private State _currentState = State.Point1;
        private QuadraticBezierShape _quadraticBezier = new QuadraticBezierShape();
        private ToolQuadraticBezierSelection _selection;

        /// <inheritdoc/>
        public string Title => "QuadraticBezier";

        /// <summary>
        /// Gets or sets the path tool settings.
        /// </summary>
        public PathToolSettingsQuadraticBezier Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathToolQuadraticBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolQuadraticBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new PathToolSettingsQuadraticBezier();
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
            var pathTool = _serviceProvider.GetService<ToolPath>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        _quadraticBezier.Point1 = editor.TryToGetConnectionPoint(sx, sy) ?? factory.CreatePointShape(sx, sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_quadraticBezier.Point1);
                        }
                        else
                        {
                            _quadraticBezier.Point1 = pathTool.GetLastPathPoint();
                        }

                        _quadraticBezier.Point2 = factory.CreatePointShape(sx, sy);
                        _quadraticBezier.Point3 = factory.CreatePointShape(sx, sy);
                        pathTool.GeometryContext.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint3();
                        Move(null);
                        _currentState = State.Point3;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Point3:
                    {
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegment;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezier.Point3 = point2;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint2();
                        Move(null);
                        _currentState = State.Point2;
                    }
                    break;
                case State.Point2:
                    {
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezier.Point2 = point1;
                            }
                        }

                        _quadraticBezier.Point1 = _quadraticBezier.Point3;
                        _quadraticBezier.Point2 = factory.CreatePointShape(sx, sy);
                        _quadraticBezier.Point3 = factory.CreatePointShape(sx, sy);
                        pathTool.GeometryContext.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint3();
                        Move(null);
                        _currentState = State.Point3;
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
                case State.Point1:
                    break;
                case State.Point3:
                case State.Point2:
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
                case State.Point1:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
                case State.Point2:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point3"/>.
        /// </summary>
        public void ToStatePoint3()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            _selection?.Reset();
            _selection = new ToolQuadraticBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _quadraticBezier,
                editor.PageState.HelperStyle);
            _selection.ToStatePoint3();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point2"/>.
        /// </summary>
        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
        }

        /// <inheritdoc/>
        public void Move(IBaseShape shape)
        {
            if (_selection != null)
            {
                _selection.Move();
            }
        }

        /// <inheritdoc/>
        public void Finalize(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point3:
                case State.Point2:
                    {
                        pathTool.RemoveLastSegment<QuadraticBezierSegment>();
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(pathTool.Path);
                        if (pathTool.Path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, pathTool.Path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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
