using System;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class LineToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { Start, End }
        private State _currentState = State.Start;
        private LineShapeViewModel _line;
        private LineSelection _selection;

        public string Title => "Line";

        public LineToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void BeginDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            (double x, double y) = args;
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _line = factory.CreateLineShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.GetShapeName<LineShapeViewModel>());
                        if (editor.Project.Options.TryToConnect)
                        {
                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result is { })
                            {
                                _line.Start = result;
                            }
                            else
                            {
                                editor.TryToSplitLine(x, y, _line.Start);
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_line);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStateEnd();
                        Move(_line);
                        _currentState = State.End;
                    }
                    break;
                case State.End:
                    {
                        if (_line is { })
                        {
                            _line.End.X = (double)sx;
                            _line.End.Y = (double)sy;

                            if (editor.Project.Options.TryToConnect)
                            {
                                var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                                if (result is { })
                                {
                                    _line.End = result;
                                }
                                else
                                {
                                    editor.TryToSplitLine(x, y, _line.End);
                                }
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                            Finalize(_line);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _line);

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
                case State.Start:
                    break;
                case State.End:
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
                case State.Start:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.End:
                    {
                        if (_line is { })
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _line.End.X = (double)sx;
                            _line.End.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_line);
                        }
                    }
                    break;
            }
        }

        public void ToStateEnd()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection = new LineSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.PageState.HelperStyle);

            _selection.ToStateEnd();
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
                case State.Start:
                    break;
                case State.End:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.Start;

            if (_selection is { })
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
