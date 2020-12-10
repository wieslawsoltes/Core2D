using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.Model.Path;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using static System.Math;

namespace Core2D.ViewModels.Editor.Tools.Path
{
    public class ArcPathToolViewModel : ViewModelBase, IPathTool
    {
        public enum State { Start, End }

        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Start;
        private LineShapeViewModel _arc = new LineShapeViewModel();
        private LineSelection _selection;
        private const double _defaultRotationAngle = 0.0;
        private const bool _defaultIsLargeArc = false;
        private const SweepDirection _defaultSweepDirection = SweepDirection.Clockwise;

        public string Title => "Arc";

        public ArcPathToolViewModel(IServiceProvider serviceProvider) : base()
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
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        editor.IsToolIdle = false;
                        _arc.Start = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_arc.Start);
                        }
                        else
                        {
                            _arc.Start = pathTool.GetLastPathPoint();
                        }

                        _arc.End = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.ArcTo(
                            _arc.End,
                            factory.CreatePathSize(
                                Abs(_arc.Start.X - _arc.End.X),
                                Abs(_arc.Start.Y - _arc.End.Y)),
                            _defaultRotationAngle,
                            _defaultIsLargeArc,
                            _defaultSweepDirection);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStateEnd();
                        Move(null);
                        _currentState = State.End;
                    }
                    break;

                case State.End:
                    {
                        _arc.End.X = (double)sx;
                        _arc.End.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (end != null)
                            {
                                _arc.End = end;
                            }
                        }
                        _arc.Start = _arc.End;
                        _arc.End = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.ArcTo(
                            _arc.End,
                            factory.CreatePathSize(
                                Abs(_arc.Start.X - _arc.End.X),
                                Abs(_arc.Start.Y - _arc.End.Y)),
                            _defaultRotationAngle,
                            _defaultIsLargeArc,
                            _defaultSweepDirection);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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
            var pathTool = _serviceProvider.GetService<PathToolViewModel>();
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
                        _arc.End.X = (double)sx;
                        _arc.End.Y = (double)sy;
                        var figure = pathTool.Geometry.Figures.LastOrDefault();
                        var arc = figure.Segments.LastOrDefault() as ArcSegmentViewModel;
                        arc.Point = _arc.End;
                        arc.Size.Width = Abs(_arc.Start.X - _arc.End.X);
                        arc.Size.Height = Abs(_arc.Start.Y - _arc.End.Y);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
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
                _arc,
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
                        pathTool.RemoveLastSegment<ArcSegmentViewModel>();
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
