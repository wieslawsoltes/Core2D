// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Editor.Bounds;
using Core2D.Editor.Tools.Path;
using Core2D.Math;
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
        private ProjectEditor _editor;
        // Path Tool
        internal XPath _path;
        internal XPathGeometry _geometry;
        internal XGeometryContext _context;
        internal bool _isInitialized;
        private PathTool _previousPathTool;
        private PathTool _movePathTool;
        // Path Tools
        private ToolPathLine _toolPathLine;
        private ToolPathArc _toolPathArc;
        private ToolPathCubicBezier _toolPathCubicBezier;
        private ToolPathQuadraticBezier _toolPathQuadraticBezier;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolPath(ProjectEditor editor)
            : base()
        {
            _editor = editor;
            _isInitialized = false;
            _toolPathLine = new ToolPathLine(_editor, this);
            _toolPathArc = new ToolPathArc(_editor, this);
            _toolPathCubicBezier = new ToolPathCubicBezier(_editor, this);
            _toolPathQuadraticBezier = new ToolPathQuadraticBezier(_editor, this);
        }

        /// <summary>
        /// Try to get connection point at specified location.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>The connected point if success.</returns>
        internal XPoint TryToGetConnectionPoint(double x, double y)
        {
            if (_editor.Project.Options.TryToConnect)
            {
                var result = ShapeHitTestPoint.HitTest(
                    _editor.Project.CurrentContainer.CurrentLayer.Shapes,
                    new Vector2(x, y),
                    _editor.Project.Options.HitThreshold);
                if (result != null && result is XPoint)
                {
                    return result as XPoint;
                }
            }
            return null;
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
                    figure.Segments.Remove(segment);
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
                        // TODO: Get point from last arc point.
                        throw new NotSupportedException();
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
                new List<XPathFigure>(),
                _editor.Project.Options.DefaultFillRule);

            _context = new XPathGeometryContext(_geometry);

            _context.BeginFigure(
                start,
                _editor.Project.Options.DefaultIsFilled,
                _editor.Project.Options.DefaultIsClosed);

            var style = _editor.Project.CurrentStyleLibrary.Selected;
            _path = XPath.Create(
                "Path",
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _geometry,
                _editor.Project.Options.DefaultIsStroked,
                _editor.Project.Options.DefaultIsFilled);

            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_path);

            _previousPathTool = _editor.CurrentPathTool;
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

            switch (_editor.CurrentPathTool)
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
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }

            if (_editor.CurrentPathTool == PathTool.Move)
            {
                _movePathTool = _previousPathTool;
            }

            _previousPathTool = _editor.CurrentPathTool;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            if (_isInitialized && _editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
                return;
            }

            switch (_editor.CurrentPathTool)
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
                        double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
                        double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;

                        // Start new figure.
                        var start = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.BeginFigure(
                            start,
                            _editor.Project.Options.DefaultIsFilled,
                            _editor.Project.Options.DefaultIsClosed);

                        // Switch to path tool before Move tool.
                        _editor.CurrentPathTool = _movePathTool;
                        SwitchPathTool(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            switch (_editor.CurrentPathTool)
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

            if (_isInitialized && _editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
            }

            switch (_editor.CurrentPathTool)
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
                        double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
                        double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            switch (_editor.CurrentPathTool)
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

            switch (_editor.CurrentPathTool)
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

            switch (_editor.CurrentPathTool)
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

            switch (_editor.CurrentPathTool)
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
        public override void Remove()
        {
            base.Remove();

            switch (_editor.CurrentPathTool)
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
