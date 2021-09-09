#nullable disable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class TextToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private State _currentState = State.TopLeft;
        private TextShapeViewModel _text;
        private TextSelection _selection;

        public string Title => "Text";

        public TextToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public void BeginDown(InputArgs args)
        {
            var factory = ServiceProvider.GetService<IViewModelFactory>();
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.ViewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _text = factory.CreateTextShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            "Text",
                            editor.Project.Options.DefaultIsStroked);

                        editor.SetShapeName(_text);

                        var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result is { })
                        {
                            _text.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_text);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                        ToStateBottomRight();
                        Move(_text);
                        _currentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text is { })
                        {
                            _text.BottomRight.X = (double)sx;
                            _text.BottomRight.Y = (double)sy;

                            var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result is { })
                            {
                                _text.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                            Finalize(_text);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _text);

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
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_text is { })
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                selection.TryToHoverShape((double)sx, (double)sy);
                            }
                            _text.BottomRight.X = (double)sx;
                            _text.BottomRight.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                            Move(_text);
                        }
                    }
                    break;
            }
        }

        public void ToStateBottomRight()
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            _selection = new TextSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _text,
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
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();

            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
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
