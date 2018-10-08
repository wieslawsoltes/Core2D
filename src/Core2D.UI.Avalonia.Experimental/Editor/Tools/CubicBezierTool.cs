// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class CubicBezierTool : ToolBase
    {
        private CubicBezierShape _cubicBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2,
            Point3
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public override string Title => "CubicBezier";

        public CubicBezierToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.GetNextPoint(x, y, false, 0.0);
            _cubicBezier = new CubicBezierShape()
            {
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Point3 = (PointShape)next.Copy(null),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_cubicBezier);
            context.Renderer.Selected.Add(_cubicBezier);
            context.Renderer.Selected.Add(_cubicBezier.StartPoint);
            context.Renderer.Selected.Add(_cubicBezier.Point1);
            context.Renderer.Selected.Add(_cubicBezier.Point2);
            context.Renderer.Selected.Add(_cubicBezier.Point3);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point3;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.Renderer.Selected.Remove(_cubicBezier);
            context.Renderer.Selected.Remove(_cubicBezier.StartPoint);
            context.Renderer.Selected.Remove(_cubicBezier.Point1);
            context.Renderer.Selected.Remove(_cubicBezier.Point2);
            context.Renderer.Selected.Remove(_cubicBezier.Point3);
            context.WorkingContainer.Shapes.Remove(_cubicBezier);

            _cubicBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
            context.CurrentContainer.Shapes.Add(_cubicBezier);
            _cubicBezier = null;

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.Renderer.Selected.Remove(_cubicBezier.Point2);
            _cubicBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
            context.Renderer.Selected.Add(_cubicBezier.Point2);

            CurrentState = State.Point1;

            context.Invalidate?.Invoke();
        }

        private void Point3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.Renderer.Selected.Remove(_cubicBezier.Point3);
            _cubicBezier.Point3 = context.GetNextPoint(x, y, false, 0.0);
            context.Renderer.Selected.Add(_cubicBezier.Point3);

            CurrentState = State.Point2;

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

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.Invalidate?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;
            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.Invalidate?.Invoke();
        }

        private void MovePoint3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;
            _cubicBezier.Point3.X = x;
            _cubicBezier.Point3.Y = y;

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_cubicBezier != null)
            {
                context.WorkingContainer.Shapes.Remove(_cubicBezier);
                context.Renderer.Selected.Remove(_cubicBezier);
                context.Renderer.Selected.Remove(_cubicBezier.StartPoint);
                context.Renderer.Selected.Remove(_cubicBezier.Point1);
                context.Renderer.Selected.Remove(_cubicBezier.Point2);
                context.Renderer.Selected.Remove(_cubicBezier.Point3);
                _cubicBezier = null;
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
                case State.Point3:
                    {
                        Point3Internal(context, x, y, modifier);
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
                case State.Point3:
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
                case State.Point3:
                    {
                        MovePoint3Internal(context, x, y, modifier);
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
