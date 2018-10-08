// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class QuadraticBezierTool : ToolBase
    {
        private QuadraticBezierShape _quadraticBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public override string Title => "QuadraticBezier";

        public QuadraticBezierToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.GetNextPoint(x, y, false, 0.0);
            _quadraticBezier = new QuadraticBezierShape()
            {
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_quadraticBezier);
            context.Renderer.Selected.Add(_quadraticBezier);
            context.Renderer.Selected.Add(_quadraticBezier.StartPoint);
            context.Renderer.Selected.Add(_quadraticBezier.Point1);
            context.Renderer.Selected.Add(_quadraticBezier.Point2);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.Renderer.Selected.Remove(_quadraticBezier);
            context.Renderer.Selected.Remove(_quadraticBezier.StartPoint);
            context.Renderer.Selected.Remove(_quadraticBezier.Point1);
            context.Renderer.Selected.Remove(_quadraticBezier.Point2);
            context.WorkingContainer.Shapes.Remove(_quadraticBezier);

            _quadraticBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
            context.CurrentContainer.Shapes.Add(_quadraticBezier);
            _quadraticBezier = null;

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.Renderer.Selected.Remove(_quadraticBezier.Point2);
            _quadraticBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
            context.Renderer.Selected.Add(_quadraticBezier.Point2);

            CurrentState = State.Point1;

            context.Invalidate?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.Invalidate?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;
            _quadraticBezier.Point2.X = x;
            _quadraticBezier.Point2.Y = y;

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.Renderer.Selected.Remove(_quadraticBezier);
                context.Renderer.Selected.Remove(_quadraticBezier.StartPoint);
                context.Renderer.Selected.Remove(_quadraticBezier.Point1);
                context.Renderer.Selected.Remove(_quadraticBezier.Point2);
                _quadraticBezier = null;
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
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Point1:
                case State.Point2:
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
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
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
