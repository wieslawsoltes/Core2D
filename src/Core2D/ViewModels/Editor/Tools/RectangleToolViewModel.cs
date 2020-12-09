using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class RectangleToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.TopLeft;
        private RectangleShapeViewModel _rectangle;
        private RectangleSelection _selection;

        public string Title => "Rectangle";

        public RectangleToolViewModel(IServiceProvider serviceProvider) : base()
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
                case State.TopLeft:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
                        _rectangle = factory.CreateRectangleShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.OptionsViewModel.DefaultIsStroked,
                            editor.Project.OptionsViewModel.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _rectangle.TopLeft = result;
                        }

                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(_rectangle);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStateBottomRight();
                        Move(_rectangle);
                        _currentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = (double)sx;
                            _rectangle.BottomRight.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _rectangle.BottomRight = result;
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_rectangle);
                            Finalize(_rectangle);
                            editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _rectangle);

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
                case State.TopLeft:
                    break;
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _rectangle.BottomRight.X = (double)sx;
                            _rectangle.BottomRight.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            Move(_rectangle);
                        }
                    }
                    break;
            }
        }

        public void ToStateBottomRight()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection = new RectangleSelection(
                _serviceProvider,
                editor.Project.CurrentContainerViewModel.HelperLayer,
                _rectangle,
                editor.PageStateViewModel.HelperStyleViewModel);

            _selection.ToStateBottomRight();
        }

        public void Move(BaseShapeViewModel shapeViewModel)
        {
            _selection.Move();
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_rectangle);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.TopLeft;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
