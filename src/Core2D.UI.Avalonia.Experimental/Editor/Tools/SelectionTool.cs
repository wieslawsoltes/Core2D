// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Editor.Selection;
using Core2D.Shapes;
using Spatial;
using Core2D.Editor.Selection.Helpers;

namespace Core2D.Editor.Tools
{
    public partial class SelectionTool : ToolBase
    {
        private RectangleShape _rectangle;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;

        public enum State
        {
            None,
            Selection,
            Move
        }

        public State CurrentState { get; set; } = State.None;

        public override string Title => "Selection";

        public SelectionToolSettings Settings { get; set; }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            DeHover(context);

            var selected = SelectHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0,
                modifier);
            if (selected == true)
            {
                context.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.Renderer.Selected.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.Style = Settings?.SelectionStyle;
                context.WorkingContainer.Shapes.Add(_rectangle);

                context.Capture?.Invoke();
                context.Invalidate?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            DeHover(context);

            SelectHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                Settings?.HitTestRadius ?? 7.0,
                modifier);

            context.WorkingContainer.Shapes.Remove(_rectangle);
            _rectangle = null;

            CurrentState = State.None;

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (!(_hover == null && context.Renderer.Selected.Count > 0))
            {
                lock (context.Renderer.Selected)
                {
                    var previous = _hover;

                    DeHover(context);

                    var target = new Point2(x, y);
                    var shape = SelectHelper.TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (shape != null)
                    {
                        Hover(context, shape);
                        context.Invalidate?.Invoke();
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.Invalidate?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Invalidate?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (context.Renderer.Selected.Count == 1)
            {
                var shape = context.Renderer.Selected.FirstOrDefault();

                if (shape is PointShape source)
                {
                    if (Settings.ConnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        Connect(context, source);
                    }

                    if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        if (_disconnected == false)
                        {
                            double treshold = Settings.DisconnectTestRadius;
                            double tx = Math.Abs(_originX - source.X);
                            double ty = Math.Abs(_originY - source.Y);
                            if (tx > treshold || ty > treshold)
                            {
                                Disconnect(context, source);
                            }
                        }
                    }
                }

                shape.Move(context.Renderer, dx, dy);
            }
            else
            {
                foreach (var shape in context.Renderer.Selected.ToList())
                {
                    if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        if (!(shape is PointShape) && _disconnected == false)
                        {
                            Disconnect(context, shape);
                        }
                    }
                }

                foreach (var shape in context.Renderer.Selected.ToList())
                {
                    shape.Move(context.Renderer, dx, dy);
                }
            }

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

            DeHover(context);

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.Renderer.Hover = null;
                context.Renderer.Selected.Clear();
            }

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        LeftDownNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        LeftDownSelectionInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftUp(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        LeftUpSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        LeftUpMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        this.Clean(context);
                    }
                    break;
                case State.Move:
                    {
                        RightDownMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        MoveNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        MoveSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        MoveMoveInternal(context, x, y, modifier);
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
