using System;
using System.Collections.Generic;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Input;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    public class PathToolLine : ViewModelBase, IPathTool
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Start;
        private LineShape _line = new LineShape();
        private ToolLineSelection _selection;

        public string Title => "Line";

        public PathToolLine(IServiceProvider serviceProvider) : base()
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
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStateEnd();
                        Move(null);
                        _currentState = State.End;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.End:
                    {
                        _line.End.X = (double)sx;
                        _line.End.Y = (double)sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (end != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as LineSegment;
                                line.Point = end;
                            }
                        }

                        _line.Start = _line.End;
                        _line.End = factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.LineTo(_line.End);
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
                _line,
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
                        pathTool.RemoveLastSegment<LineSegment>();
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
