using System;
using System.Collections.Generic;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using static System.Math;

namespace Core2D.Editor.Tools.Path
{
    public class PathToolArc : ViewModelBase, IPathTool
    {
        public enum State { Start, End }

        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Start;
        private LineShape _arc = new LineShape();
        private ToolLineSelection _selection;
        private const double _defaultRotationAngle = 0.0;
        private const bool _defaultIsLargeArc = false;
        private const SweepDirection _defaultSweepDirection = SweepDirection.Clockwise;

        public string Title => "Arc";

        public PathToolArc(IServiceProvider serviceProvider) : base()
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
            var pathTool = _serviceProvider.GetService<ToolPath>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
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
                        editor.IsToolIdle = false;
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

        public void LeftUp(InputArgs args)
        {
        }

        public void RightDown(InputArgs args)
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

        public void RightUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
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
                        var arc = figure.Segments.LastOrDefault() as ArcSegment;
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection?.Reset();
            _selection = new ToolLineSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _arc,
                editor.PageState.HelperStyle);

            _selection.ToStateEnd();
        }

        public void Move(BaseShape shape)
        {
            _selection?.Move();
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();

            switch (_currentState)
            {
                case State.Start:
                    break;

                case State.End:
                    {
                        pathTool.RemoveLastSegment<ArcSegment>();
                    }
                    break;
            }

            _currentState = State.Start;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
