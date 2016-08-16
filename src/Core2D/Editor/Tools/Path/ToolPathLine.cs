// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.Line"/> editor.
    /// </summary>
    internal class ToolPathLine : ToolBase
    {
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        private XPathLine _line = new XPathLine();
        private LineSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathLine"/> class.
        /// </summary>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathLine(ToolPath toolPath)
            : base()
        {
            _toolPath = toolPath;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _line.Start = Editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_line.Start);
                        }
                        else
                        {
                            _line.Start = _toolPath.GetLastPathPoint();
                        }

                        _line.End = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _toolPath._context.LineTo(
                            _line.End,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsSmoothJoin);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        if (Editor.Project.Options.TryToConnect)
                        {
                            var end = Editor.TryToGetConnectionPoint(sx, sy);
                            if (end != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as XLineSegment;
                                line.Point = end;
                            }
                        }

                        _line.Start = _line.End;
                        _line.End = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _toolPath._context.LineTo(_line.End,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsSmoothJoin);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
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
                    {
                        _toolPath.RemoveLastSegment<XLineSegment>();

                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        Editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new LineSelection(
                Editor.Project.CurrentContainer.HelperLayer,
                _line,
                Editor.Project.Options.HelperStyle,
                Editor.Project.Options.PointShape);

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
