using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class ToolCubicBezier : ViewModelBase, IEditorTool
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private CubicBezierShape _cubicBezier;
        private ToolCubicBezierSelection _selection;

        public string Title => "CubicBezier";

        public ToolCubicBezier(IServiceProvider serviceProvider) : base()
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
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _cubicBezier = factory.CreateCubicBezierShape(
                            (double)sx, (double)sy,
                            (ShapeStyle)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _cubicBezier.Point1 = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_cubicBezier);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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

                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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

                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                            Finalize(_cubicBezier);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _cubicBezier);

                            Reset();
                        }
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
                        if (_cubicBezier != null)
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
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _cubicBezier.Point2.X = (double)sx;
                            _cubicBezier.Point2.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _cubicBezier.Point3.X = (double)sx;
                            _cubicBezier.Point3.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_cubicBezier);
                        }
                    }
                    break;
            }
        }

        public void ToStatePoint4()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
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

        public void ToStatePoint3()
        {
            _selection.ToStatePoint3();
        }

        public void Move(BaseShape shape)
        {
            _selection.Move();
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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
