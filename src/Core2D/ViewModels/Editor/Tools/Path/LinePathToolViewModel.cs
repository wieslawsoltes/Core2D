﻿#nullable disable
using System;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools.Path
{
    public partial class LinePathToolViewModel : ViewModelBase, IPathTool
    {
        public enum State { Start, End }
        private State _currentState;
        private LineShapeViewModel _line;
        private LineSelection _selection;

        public string Title => "Line";

        public LinePathToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentState = State.Start;
            _line = new LineShapeViewModel(serviceProvider);
        }

        public void BeginDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IViewModelFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        editor.IsToolIdle = false;
                        _line.Start = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_line.Start);
                        }
                        else
                        {
                            _line.Start = pathTool.GetLastPathPoint();
                        }

                        _line.End = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.LineTo(_line.End);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                        ToStateEnd();
                        Move(null);
                        _currentState = State.End;
                    }
                    break;
                case State.End:
                    {
                        _line.End.X = (double)sx;
                        _line.End.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (end is { })
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as LineSegmentViewModel;
                                line.Point = end;
                            }
                        }

                        _line.Start = _line.End;
                        _line.End = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.LineTo(_line.End);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                        Move(null);
                        _currentState = State.End;
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
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                        _line.End.X = (double)sx;
                        _line.End.Y = (double)sy;
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                        Move(null);
                    }
                    break;
            }
        }

        public void ToStateEnd()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection?.Reset();
            _selection = new LineSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.PageState.HelperStyle);
            _selection.ToStateEnd();
        }

        public void Move(BaseShapeViewModel shape)
        {
            _selection?.Move();
        }

        public void Finalize(BaseShapeViewModel shape)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();

            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    {
                        pathTool.RemoveLastSegment<LineSegmentViewModel>();
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
