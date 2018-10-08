// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class GuideTool : ToolBase
    {
        private LineShape _line = null;

        public enum State
        {
            StartPoint,
            Point
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public override string Title => "Guide";

        public GuideToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line = new LineShape()
            {
                StartPoint = new PointShape(x, y, null),
                Point = new PointShape(x, y, null),
                Style = Settings?.GuideStyle
            };
            context.WorkingContainer.Shapes.Add(_line);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            _line.Point.X = x;
            _line.Point.Y = y;
            context.WorkingContainer.Shapes.Remove(_line);
            context.CurrentContainer.Guides.Add(_line);
            _line = null;

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void MoveStratPointInternal(IToolContext context, double x, double y, Modifier modifier)
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

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.StartPoint;

            Filters?.ForEach(f => f.Clear(context));

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                _line = null;
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
                        MoveStratPointInternal(context, x, y, modifier);
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
