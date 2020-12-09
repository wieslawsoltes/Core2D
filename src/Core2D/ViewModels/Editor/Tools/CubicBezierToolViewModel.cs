using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class CubicBezierToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private CubicBezierShapeViewModel _cubicBezier;
        private BezierSelectionSelection _selectionSelection;

        public string Title => "CubicBezier";

        public CubicBezierToolViewModel(IServiceProvider serviceProvider) : base()
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
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
                        _cubicBezier = factory.CreateCubicBezierShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.OptionsViewModel.DefaultIsStroked,
                            editor.Project.OptionsViewModel.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _cubicBezier.Point1 = result;
                        }

                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(_cubicBezier);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStatePoint4();
                        Move(_cubicBezier);
                        _currentState = State.Point4;
                    }
                    break;
                case State.Point4:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = (double)sx;
                            _cubicBezier.Point3.Y = (double)sy;
                            _cubicBezier.Point4.X = (double)sx;
                            _cubicBezier.Point4.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _cubicBezier.Point4 = result;
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            ToStatePoint2();
                            Move(_cubicBezier);
                            _currentState = State.Point2;
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point2.X = (double)sx;
                            _cubicBezier.Point2.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _cubicBezier.Point2 = result;
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            ToStatePoint3();
                            Move(_cubicBezier);
                            _currentState = State.Point3;
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = (double)sx;
                            _cubicBezier.Point3.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _cubicBezier.Point3 = result;
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_cubicBezier);
                            Finalize(_cubicBezier);
                            editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _cubicBezier);

                            Reset();
                        }
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
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    Reset();
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
                case State.Point4:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _cubicBezier.Point2.X = (double)sx;
                            _cubicBezier.Point2.Y = (double)sy;
                            _cubicBezier.Point3.X = (double)sx;
                            _cubicBezier.Point3.Y = (double)sy;
                            _cubicBezier.Point4.X = (double)sx;
                            _cubicBezier.Point4.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _cubicBezier.Point2.X = (double)sx;
                            _cubicBezier.Point2.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _cubicBezier.Point3.X = (double)sx;
                            _cubicBezier.Point3.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            Move(_cubicBezier);
                        }
                    }
                    break;
            }
        }

        public void ToStatePoint4()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selectionSelection = new BezierSelectionSelection(
                _serviceProvider,
                editor.Project.CurrentContainerViewModel.HelperLayer,
                _cubicBezier,
                editor.PageStateViewModel.HelperStyleViewModel);

            _selectionSelection.ToStatePoint4();
        }

        public void ToStatePoint2()
        {
            _selectionSelection.ToStatePoint2();
        }

        public void ToStatePoint3()
        {
            _selectionSelection.ToStatePoint3();
        }

        public void Move(BaseShapeViewModel shapeViewModel)
        {
            _selectionSelection.Move();
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    {
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_cubicBezier);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.Point1;

            if (_selectionSelection != null)
            {
                _selectionSelection.Reset();
                _selectionSelection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
