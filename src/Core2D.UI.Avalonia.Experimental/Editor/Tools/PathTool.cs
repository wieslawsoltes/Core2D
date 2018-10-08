// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public partial class PathTool : ToolBase
    {
        private PathShape _path;
        private FigureShape _figure;

        public override string Title => "Path";

        public PathToolSettings Settings { get; set; }

        public PointShape GetLastPoint()
        {
            if (_path?.Figures.Count > 0)
            {
                var shapes = _path.Figures[_path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[shapes.Count - 1])
                    {
                        case LineShape line:
                            return line.Point;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.Point3;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.Point2;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public void Create(IToolContext context)
        {
            _path = new PathShape()
            {
                FillRule = Settings.FillRule,
                Style = context.CurrentStyle
            };

            context.WorkingContainer.Shapes.Add(_path);
            context.Renderer.Selected.Add(_path);
        }

        public void Move()
        {
            _figure = new FigureShape()
            {
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };
            _path.Figures.Add(_figure);

            if (Settings.PreviousTool != null)
            {
                Settings.CurrentTool = Settings.PreviousTool;
            }
        }

        public void CleanCurrentTool(IToolContext context)
        {
            SetContext(context);
            Settings.CurrentTool?.Clean(this);
            SetContext(null);
            UpdateCache(context);
        }

        public void UpdateCache(IToolContext context)
        {
            if (_path != null)
            {
                _figure.MarkAsDirty(true);
                _path.Invalidate(context.Renderer, 0.0, 0.0);
            }
        }

        private void DownInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            if (_path == null)
            {
                Create(context);
                Move();
            }

            SetContext(context);
            Settings.CurrentTool?.LeftDown(this, x, y, modifier);

            switch (Settings.CurrentTool)
            {
                case LineTool lineTool:
                    {
                        if (lineTool.CurrentState == LineTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case CubicBezierTool cubicBezierTool:
                    {
                        if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case QuadraticBezierTool quadraticBezierTool:
                    {
                        if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
            }

            SetContext(null);
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            SetContext(context);
            Settings.CurrentTool.Move(this, x, y, modifier);
            SetContext(null);
        }

        private void CleanInternal(IToolContext context)
        {
            CleanCurrentTool(context);

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.WorkingContainer.Shapes.Remove(_path);
                context.Renderer.Selected.Remove(_path);

                if (_path.Validate(true) == true)
                {
                    context.CurrentContainer.Shapes.Add(_path);
                }

                Settings.PreviousTool = null;
                SetNextPoint(null);
                SetContext(null);

                _path = null;
                _figure = null;
            }
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            DownInternal(context, x, y, modifier);
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            this.Clean(context);
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            MoveInternal(context, x, y, modifier);
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
