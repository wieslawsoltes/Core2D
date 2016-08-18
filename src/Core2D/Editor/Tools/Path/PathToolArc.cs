// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;
using static System.Math;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.Arc"/> editor.
    /// </summary>
    public class PathToolArc : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ToolPath _toolPath;
        private ToolState _currentState = ToolState.None;
        private XPathArc _arc = new XPathArc();
        private LineSelection _selection;
        private const double _defaultRotationAngle = 0.0;
        private const bool _defaultIsLargeArc = false;
        private const XSweepDirection _defaultSweepDirection = XSweepDirection.Clockwise;

        /// <inheritdoc/>
        public override string Name => "Arc";

        /// <summary>
        /// Initialize new instance of <see cref="PathToolArc"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public PathToolArc(IServiceProvider serviceProvider, ToolPath toolPath) : base()
        {
            _serviceProvider = serviceProvider;
            _toolPath = toolPath;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _arc.Start = editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_arc.Start);
                        }
                        else
                        {
                            _arc.Start = _toolPath.GetLastPathPoint();
                        }

                        _arc.End = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _toolPath._context.ArcTo(
                            _arc.End,
                            XPathSize.Create(
                                Abs(_arc.Start.X - _arc.End.X),
                                Abs(_arc.Start.Y - _arc.End.Y)),
                            _defaultRotationAngle,
                            _defaultIsLargeArc,
                            _defaultSweepDirection,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _arc.End.X = sx;
                        _arc.End.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint(sx, sy);
                            if (end != null)
                            {
                                _arc.End = end;
                            }
                        }
                        _arc.Start = _arc.End;
                        _arc.End = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _toolPath._context.ArcTo(
                            _arc.End,
                            XPathSize.Create(
                                Abs(_arc.Start.X - _arc.End.X),
                                Abs(_arc.Start.Y - _arc.End.Y)),
                            _defaultRotationAngle,
                            _defaultIsLargeArc,
                            _defaultSweepDirection,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();

                        Move(null);
                        _currentState = ToolState.One;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                    {
                        _toolPath.RemoveLastSegment<XArcSegment>();

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _arc.End.X = sx;
                        _arc.End.Y = sy;
                        var figure = _toolPath._geometry.Figures.LastOrDefault();
                        var arc = figure.Segments.LastOrDefault() as XArcSegment;
                        arc.Point = _arc.End;
                        arc.Size.Width = Abs(_arc.Start.X - _arc.End.X);
                        arc.Size.Height = Abs(_arc.Start.Y - _arc.End.Y);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new LineSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _arc,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _currentState = ToolState.None;

            _selection.Remove();
            _selection = null;
        }
    }
}
