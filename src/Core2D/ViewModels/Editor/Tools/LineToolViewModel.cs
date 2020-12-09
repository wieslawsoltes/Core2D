using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class LineToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Start;
        private LineShapeViewModel _line;
        private LineSelection _selection;

        public string Title => "Line";

        public LineToolViewModel(IServiceProvider serviceProvider) : base()
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
            (double x, double y) = args;
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
                        _line = factory.CreateLineShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.OptionsViewModel.DefaultIsStroked);
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _line.Start = result;
                            }
                            else
                            {
                                editor.TryToSplitLine(x, y, _line.Start);
                            }
                        }
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(_line);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        ToStateEnd();
                        Move(_line);
                        _currentState = State.End;
                    }
                    break;
                case State.End:
                    {
                        if (_line != null)
                        {
                            _line.End.X = (double)sx;
                            _line.End.Y = (double)sy;

                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                                if (result != null)
                                {
                                    _line.End = result;
                                }
                                else
                                {
                                    editor.TryToSplitLine(x, y, _line.End);
                                }
                            }

                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_line);
                            Finalize(_line);
                            editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _line);

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
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.End:
                    {
                        if (_line != null)
                        {
                            if (editor.Project.OptionsViewModel.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _line.End.X = (double)sx;
                            _line.End.Y = (double)sy;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
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
                editor.Project.CurrentContainerViewModel.HelperLayer,
                _line,
                editor.PageStateViewModel.HelperStyleViewModel);

            _selection.ToStateEnd();
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
                case State.Start:
                    break;
                case State.End:
                    {
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_line);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.Start;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
