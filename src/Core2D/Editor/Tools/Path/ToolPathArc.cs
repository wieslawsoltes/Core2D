// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Math.Arc;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.Arc"/> editor.
    /// </summary>
    internal class ToolPathArc : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        private XPathArc _arc = new XPathArc();
        private ArcSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathArc"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathArc(ProjectEditor editor, ToolPath toolPath)
            : base()
        {
            _editor = editor;
            _toolPath = toolPath;
        }

        private void NewArc(double sx, double sy)
        {
            _arc.Point1 = _editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            if (!_toolPath._isInitialized)
            {
                _toolPath.InitializeWorkingPath(_arc.Point1);
            }
            else
            {
                _arc.Point1 = _toolPath.GetLastPathPoint();
            }

            _arc.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _arc.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _arc.Point4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
        }

        private void NextArc(double sx, double sy)
        {
            _arc.Point1 = _arc.Point4;
            _arc.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _arc.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _arc.Point4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
        }

        private void InsertArcSegment()
        {
            var a = WpfArc.FromXArc(_arc, 0.0, 0.0);
            _toolPath._context.ArcTo(
                XPoint.Create(a.End.X, a.End.Y),
                XPathSize.Create(a.Radius.Width, a.Radius.Height),
                0.0,
                a.IsLargeArc, XSweepDirection.Clockwise,
                _editor.Project.Options.DefaultIsStroked,
                _editor.Project.Options.DefaultIsSmoothJoin);
            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
        }

        private void UpdateArcSegment()
        {
            var figure = _toolPath._geometry.Figures.LastOrDefault();
            var arc = figure.Segments.LastOrDefault() as XArcSegment;
            var a = WpfArc.FromXArc(_arc, 0.0, 0.0);
            arc.Point = XPoint.Create(a.End.X, a.End.Y);
            arc.Size = XPathSize.Create(a.Radius.Width, a.Radius.Height);
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        NewArc(sx, sy);
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _arc.Point2.X = sx;
                        _arc.Point2.Y = sy;
                        _arc.Point3.X = sx;
                        _arc.Point3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                _arc.Point2 = point2;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _currentState = ToolState.Two;
                    }
                    break;
                case ToolState.Two:
                    {
                        _arc.Point3.X = sx;
                        _arc.Point3.Y = sy;
                        _arc.Point4.X = sx;
                        _arc.Point4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point3 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                _arc.Point3 = point3;
                            }
                        }
                        InsertArcSegment();
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _currentState = ToolState.Three;
                    }
                    break;
                case ToolState.Three:
                    {
                        _arc.Point4.X = sx;
                        _arc.Point4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point4 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point4 != null)
                            {
                                _arc.Point4 = point4;
                            }
                        }
                        UpdateArcSegment();

                        NextArc(sx, sy);
                        Remove();
                        ToStateOne();
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

            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        _toolPath.RemoveLastSegment<XCubicBezierSegment>();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _arc.Point2.X = sx;
                        _arc.Point2.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _arc.Point3.X = sx;
                        _arc.Point3.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _arc.Point4.X = sx;
                        _arc.Point4.Y = sy;
                        UpdateArcSegment();
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new ArcSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _arc,
                _editor.Project.Options.HelperStyle,
                _editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _selection.ToStateTwo();
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _selection.ToStateThree();
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
