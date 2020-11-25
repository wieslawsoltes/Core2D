using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class ToolQuadraticBezier : ViewModelBase, IEditorTool
    {
        public enum State { Point1, Point3, Point2 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private QuadraticBezierShape _quadraticBezier;
        private ToolQuadraticBezierSelection _selection;

        public string Title => "QuadraticBezier";

        public ToolQuadraticBezier(IServiceProvider serviceProvider) : base()
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
                        _quadraticBezier = factory.CreateQuadraticBezierShape(
                            (double)sx, (double)sy,
                            (ShapeStyle)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _quadraticBezier.Point1 = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_quadraticBezier);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint3();
                        Move(_quadraticBezier);
                        _currentState = State.Point3;
                    }
                    break;
                case State.Point3:
                    {
                        if (_quadraticBezier != null)
                        {
                            _quadraticBezier.Point2.X = (double)sx;
                            _quadraticBezier.Point2.Y = (double)sy;
                            _quadraticBezier.Point3.X = (double)sx;
                            _quadraticBezier.Point3.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _quadraticBezier.Point3 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            ToStatePoint2();
                            Move(_quadraticBezier);
                            _currentState = State.Point2;
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_quadraticBezier != null)
                        {
                            _quadraticBezier.Point2.X = (double)sx;
                            _quadraticBezier.Point2.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _quadraticBezier.Point2 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_quadraticBezier);
                            Finalize(_quadraticBezier);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _quadraticBezier);

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
                case State.Point3:
                case State.Point2:
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
                case State.Point3:
                    {
                        if (_quadraticBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _quadraticBezier.Point2.X = (double)sx;
                            _quadraticBezier.Point2.Y = (double)sy;
                            _quadraticBezier.Point3.X = (double)sx;
                            _quadraticBezier.Point3.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_quadraticBezier);
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_quadraticBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _quadraticBezier.Point2.X = (double)sx;
                            _quadraticBezier.Point2.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_quadraticBezier);
                        }
                    }
                    break;
            }
        }

        public void ToStatePoint3()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolQuadraticBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _quadraticBezier,
                editor.PageState.HelperStyle);

            _selection.ToStatePoint3();
        }

        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
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
                case State.Point3:
                case State.Point2:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_quadraticBezier);
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
