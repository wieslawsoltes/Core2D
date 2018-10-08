// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class EllipseTool : ToolBase
    {
        private EllipseShape _ellipse = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        public State CurrentState { get; set; } = State.TopLeft;

        public override string Title => "Ellipse";

        public EllipseToolSettings Settings { get; set; }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _ellipse = new EllipseShape()
            {
                TopLeft = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.GetNextPoint(x, y, false, 0.0),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_ellipse);
            context.Renderer.Selected.Add(_ellipse.TopLeft);
            context.Renderer.Selected.Add(_ellipse.BottomRight);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.Renderer.Selected.Remove(_ellipse.BottomRight);
            _ellipse.BottomRight = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            context.WorkingContainer.Shapes.Remove(_ellipse);
            context.Renderer.Selected.Remove(_ellipse.TopLeft);
            context.CurrentContainer.Shapes.Add(_ellipse);
            _ellipse = null;

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _ellipse.BottomRight.X = x;
            _ellipse.BottomRight.Y = y;

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_ellipse != null)
            {
                context.WorkingContainer.Shapes.Remove(_ellipse);
                context.Renderer.Selected.Remove(_ellipse.TopLeft);
                context.Renderer.Selected.Remove(_ellipse.BottomRight);
                _ellipse = null;
            }

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        BottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        MoveBottomRightInternal(context, x, y, modifier);
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
