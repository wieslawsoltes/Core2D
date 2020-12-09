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
    public class QuadraticBezierPathToolViewModel : ViewModelBase, IPathTool
    {
        public enum State { Point1, Point3, Point2 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private QuadraticBezierShapeViewModel _quadraticBezier = new QuadraticBezierShapeViewModel();
        private QuadraticBezierSelection _selection;

        public string Title => "QuadraticBezier";

        public QuadraticBezierPathToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void BeginDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        editor.IsToolIdle = false;
                        _quadraticBezier.Point1 = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_quadraticBezier.Point1);
                        }
                        else
                        {
                            _quadraticBezier.Point1 = pathTool.GetLastPathPoint();
                        }

                        _quadraticBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                        _quadraticBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStatePoint3();
                        Move(null);
                        _currentState = State.Point3;
                    }
                    break;
                case State.Point3:
                    {
                        _quadraticBezier.Point3.X = (double)sx;
                        _quadraticBezier.Point3.Y = (double)sy;
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (point2 != null)
                            {
                                var figure = pathTool.GeometryViewModel.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegmentViewModel;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezier.Point3 = point2;
                            }
                        }
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStatePoint2();
                        Move(null);
                        _currentState = State.Point2;
                    }
                    break;
                case State.Point2:
                    {
                        _quadraticBezier.Point2.X = (double)sx;
                        _quadraticBezier.Point2.Y = (double)sy;
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (point1 != null)
                            {
                                var figure = pathTool.GeometryViewModel.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegmentViewModel;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezier.Point2 = point1;
                            }
                        }

                        _quadraticBezier.Point1 = _quadraticBezier.Point3;
                        _quadraticBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                        _quadraticBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStatePoint3();
                        Move(null);
                        _currentState = State.Point3;
                    }
                    break;
            }
        }

        public void BeginUp(InputArgs args)
        {
        }

        public void EndDown(InputArgs args)
        {
            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point3:
                case State.Point2:
                    Reset();
                    Finalize(null);
                    break;
            }
        }

        public void EndUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _quadraticBezier.Point2.X = (double)sx;
                        _quadraticBezier.Point2.Y = (double)sy;
                        _quadraticBezier.Point3.X = (double)sx;
                        _quadraticBezier.Point3.Y = (double)sy;
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
                case State.Point2:
                    {
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _quadraticBezier.Point2.X = (double)sx;
                        _quadraticBezier.Point2.Y = (double)sy;
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        Move(null);
                    }
                    break;
            }
        }

        public void ToStatePoint3()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection?.Reset();
            _selection = new QuadraticBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainerViewModel.HelperLayer,
                _quadraticBezier,
                editor.PageStateViewModel.HelperStyleViewModel);
            _selection.ToStatePoint3();
        }

        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
        }

        public void Move(BaseShapeViewModel shapeViewModel)
        {
            _selection?.Move();
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point3:
                case State.Point2:
                    {
                        pathTool.RemoveLastSegment<QuadraticBezierSegmentViewModel>();
                    }
                    break;
            }

            _currentState = State.Point1;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
