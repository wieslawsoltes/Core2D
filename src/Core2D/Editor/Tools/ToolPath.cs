// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Editor.Tools.Path;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Path"/> editor.
    /// </summary>
    public class ToolPath : ToolBase
    {
        internal XPath _path;
        internal XPathGeometry _geometry;
        internal XGeometryContext _context;
        internal bool _isInitialized;
        private PathTool _previousPathTool;
        private PathTool _movePathTool;
        private ToolPathLine _toolPathLine;
        private ToolPathArc _toolPathArc;
        private ToolPathCubicBezier _toolPathCubicBezier;
        private ToolPathQuadraticBezier _toolPathQuadraticBezier;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        public ToolPath()
            : base()
        {
            _isInitialized = false;
            _toolPathLine = new ToolPathLine(this);
            _toolPathArc = new ToolPathArc(this);
            _toolPathCubicBezier = new ToolPathCubicBezier(this);
            _toolPathQuadraticBezier = new ToolPathQuadraticBezier(this);
        }

        /// <summary>
        /// Remove last <see cref="XPathSegment"/> segment from the last figure.
        /// </summary>
        /// <typeparam name="T">The type of the path segment to remove.</typeparam>
        internal void RemoveLastSegment<T>() where T : XPathSegment
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as T;
                if (segment != null)
                {
                    figure.Segments = figure.Segments.Remove(segment);
                }
            }
        }

        /// <summary>
        /// Gets last point in the current path.
        /// </summary>
        /// <returns>The last path point.</returns>
        internal XPoint GetLastPathPoint()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        return (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        return (segment as XArcSegment).Point;
                    }
                    else if (segment is XCubicBezierSegment)
                    {
                        return (segment as XCubicBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        return (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    return figure.StartPoint;
                }
            }

            throw new Exception("Can not find valid last point from path.");
        }

        internal void InitializeWorkingPath(XPoint start)
        {
            _geometry = XPathGeometry.Create(
                ImmutableArray.Create<XPathFigure>(),
                Editor.Project.Options.DefaultFillRule);

            _context = new XPathGeometryContext(_geometry);

            _context.BeginFigure(
                start,
                Editor.Project.Options.DefaultIsFilled,
                Editor.Project.Options.DefaultIsClosed);

            var style = Editor.Project.CurrentStyleLibrary.Selected;
            _path = XPath.Create(
                "Path",
                Editor.Project.Options.CloneStyle ? style.Clone() : style,
                _geometry,
                Editor.Project.Options.DefaultIsStroked,
                Editor.Project.Options.DefaultIsFilled);

            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_path);

            _previousPathTool = Editor.CurrentPathTool;
            _isInitialized = true;
        }

        internal void DeInitializeWorkingPath()
        {
            _isInitialized = false;
            _geometry = null;
            _context = null;
            _path = null;
        }

        private void SwitchPathTool(double x, double y)
        {
            switch (_previousPathTool)
            {
                case PathTool.Line:
                    {
                        RemoveLastSegment<XLineSegment>();
                        _toolPathLine.Remove();
                    }
                    break;
                case PathTool.Arc:
                    {
                        RemoveLastSegment<XArcSegment>();
                        _toolPathArc.Remove();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        RemoveLastSegment<XCubicBezierSegment>();
                        _toolPathCubicBezier.Remove();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        RemoveLastSegment<XQuadraticBezierSegment>();
                        _toolPathQuadraticBezier.Remove();
                    }
                    break;
                case PathTool.Move:
                    break;
            }

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.LeftDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.LeftDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.LeftDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.LeftDown(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }

            if (Editor.CurrentPathTool == PathTool.Move)
            {
                _movePathTool = _previousPathTool;
            }

            _previousPathTool = Editor.CurrentPathTool;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            if (_isInitialized && Editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
                return;
            }

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.LeftDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.LeftDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.LeftDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.LeftDown(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
                        double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;

                        // Start new figure.
                        var start = Editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _context.BeginFigure(
                            start,
                            Editor.Project.Options.DefaultIsFilled,
                            Editor.Project.Options.DefaultIsClosed);

                        // Switch to path tool before Move tool.
                        Editor.CurrentPathTool = _movePathTool;
                        SwitchPathTool(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.RightDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.RightDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.RightDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.RightDown(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            if (_isInitialized && Editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
            }

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.Move(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.Move(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.Move(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.Move(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
                        double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.ToStateOne();
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.ToStateOne();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.ToStateOne();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.ToStateOne();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.ToStateTwo();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.ToStateTwo();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.ToStateTwo();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.ToStateThree();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathQuadraticBezier.ToStateThree();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.Move(shape);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.Move(shape);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.Move(shape);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.Move(shape);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.Finalize(shape);
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.Finalize(shape);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.Finalize(shape);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.Finalize(shape);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            switch (Editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        _toolPathLine.Remove();
                    }
                    break;
                case PathTool.Arc:
                    {
                        _toolPathArc.Remove();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        _toolPathCubicBezier.Remove();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        _toolPathQuadraticBezier.Remove();
                    }
                    break;
            }
        }
    }
}
