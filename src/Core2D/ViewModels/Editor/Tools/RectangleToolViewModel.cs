using System;
using System.Linq;
using Avalonia;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class RectangleToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private State _currentState = State.TopLeft;
        private RectangleShapeViewModel _rectangle;
        private RectangleSelection _selection;

        public string Title => "Rectangle";

        public RectangleToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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
                        var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                        _rectangle = factory.CreateRectangleShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        editor.SetShapeName(_rectangle);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result is { })
                        {
                            _rectangle.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStateBottomRight();
                        Move(_rectangle);
                        _currentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle is { })
                        {
                            _rectangle.BottomRight.X = (double)sx;
                            _rectangle.BottomRight.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result is { })
                            {
                                _rectangle.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            Finalize(_rectangle);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _rectangle);

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
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle is { })
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _rectangle.BottomRight.X = (double)sx;
                            _rectangle.BottomRight.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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
                editor.Project.CurrentContainer.HelperLayer,
                _rectangle,
                editor.PageState.HelperStyle);

            _selection.ToStateBottomRight();
        }

        public void Move(BaseShapeViewModel shape)
        {
            _selection.Move();
        }

        public void Finalize(BaseShapeViewModel shape)
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
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.TopLeft;

            if (_selection is { })
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
