using System;
using System.Collections.Generic;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    public class PathToolCubicBezier : ViewModelBase, IPathTool
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private CubicBezierShape _cubicBezier = new CubicBezierShape();
        private ToolCubicBezierSelection _selection;

        public string Title => "CubicBezier";

        public PathToolCubicBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        _cubicBezier.Point1 = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_cubicBezier.Point1);
                        }
                        else
                        {
                            _cubicBezier.Point1 = pathTool.GetLastPathPoint();
                        }

                        _cubicBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                        _cubicBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                        _cubicBezier.Point4 = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint4();
                        Move(null);
                        _currentState = State.Point4;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Point4:
                    {
                        _cubicBezier.Point4.X = (double)sx;
                        _cubicBezier.Point4.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point3 = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (point3 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezier.Point4 = point3;
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
                        _cubicBezier.Point2.X = (double)sx;
                        _cubicBezier.Point2.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (point1 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezier.Point2 = point1;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStateThree();
                        Move(null);
                        _currentState = State.Point3;
                    }
                    break;
                case State.Point3:
                    {
                        _cubicBezier.Point3.X = (double)sx;
                        _cubicBezier.Point3.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (point2 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezier.Point3 = point2;
                            }
                        }

                        _cubicBezier.Point1 = _cubicBezier.Point4;
                        _cubicBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                        _cubicBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                        _cubicBezier.Point4 = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint4();
                        Move(null);
                        _currentState = State.Point4;
                    }
                    break;
            }
        }

        public void LeftUp(InputArgs args)
        {
        }

        public void RightDown(InputArgs args)
        {
            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    Reset();
                    Finalize(null);
                    break;
            }
        }

        public void RightUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _cubicBezier.Point2.X = (double)sx;
                        _cubicBezier.Point2.Y = (double)sy;
                        _cubicBezier.Point3.X = (double)sx;
                        _cubicBezier.Point3.Y = (double)sy;
                        _cubicBezier.Point4.X = (double)sx;
                        _cubicBezier.Point4.Y = (double)sy;
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
                case State.Point2:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _cubicBezier.Point2.X = (double)sx;
                        _cubicBezier.Point2.Y = (double)sy;
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
                case State.Point3:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _cubicBezier.Point3.X = (double)sx;
                        _cubicBezier.Point3.Y = (double)sy;
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
            }
        }

        public void ToStatePoint4()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection?.Reset();
            _selection = new ToolCubicBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.PageState.HelperStyle);
            _selection.ToStatePoint4();
        }

        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
        }

        public void ToStateThree()
        {
            _selection.ToStatePoint3();
        }

        public void Move(BaseShape shape)
        {
            _selection?.Move();
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
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
