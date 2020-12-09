using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class TextToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.TopLeft;
        private TextShapeViewModel _text;
        private TextSelection _selection;

        public string Title => "Text";

        public TextToolViewModel(IServiceProvider serviceProvider) : base()
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
                        _text = factory.CreateTextShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            "Text",
                            editor.Project.OptionsViewModel.DefaultIsStroked);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _text.TopLeft = result;
                        }

                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(_text);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStateBottomRight();
                        Move(_text);
                        _currentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text != null)
                        {
                            _text.BottomRight.X = (double)sx;
                            _text.BottomRight.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _text.BottomRight = result;
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_text);
                            Finalize(_text);
                            editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _text);

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
                    if (editor.Project.OptionsViewModel.TryToConnect)
                    {
                        editor.TryToHoverShape((double)sx, (double)sy);
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _text.BottomRight.X = (double)sx;
                            _text.BottomRight.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            Move(_text);
                        }
                    }
                    break;
            }
        }

        public void ToStateBottomRight()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection = new TextSelection(
                _serviceProvider,
                editor.Project.CurrentContainerViewModel.HelperLayer,
                _text,
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
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_text);
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
