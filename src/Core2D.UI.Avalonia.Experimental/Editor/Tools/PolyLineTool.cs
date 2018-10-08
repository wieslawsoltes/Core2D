// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class PolyLineTool : ToolBase
    {
        private LineShape _line = null;
        private List<PointShape> _points = null;

        public enum State
        {
            StartPoint,
            Point
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public override string Title => "PolyLine";

        public PolyLineToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _points = new List<PointShape>();
            _line = new LineShape()
            {
                StartPoint = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                Point = context.GetNextPoint(x, y, false, 0.0),
                Style = context?.CurrentStyle
            };
            _points.Add(_line.StartPoint);
            _points.Add(_line.Point);
            context.WorkingContainer.Shapes.Add(_line);
            context.Renderer.Selected.Add(_line.StartPoint);
            context.Renderer.Selected.Add(_line.Point);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Renderer.Selected.Remove(_line.Point);
            _line.Point = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _points[_points.Count - 1] = _line.Point;

            if (!context.Renderer.Selected.Contains(_line.Point))
            {
                context.Renderer.Selected.Add(_line.Point);
            }

            context.WorkingContainer.Shapes.Remove(_line);
            context.CurrentContainer.Shapes.Add(_line);

            _line = new LineShape()
            {
                StartPoint = _points.Last(),
                Point = context.GetNextPoint(x, y, false, 0.0),
                Style = context.CurrentStyle
            };
            _points.Add(_line.Point);
            context.WorkingContainer.Shapes.Add(_line);
            context.Renderer.Selected.Add(_line.Point);

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            context.Invalidate?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line.Point.X = x;
            _line.Point.Y = y;

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                _line = null;
            }

            if (_points != null)
            {
                _points.ForEach(point => context.Renderer.Selected.Remove(point));
                _points = null;
            }

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
