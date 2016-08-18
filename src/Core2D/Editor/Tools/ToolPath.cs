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
        private readonly IServiceProvider _serviceProvider;
        private readonly PathToolLine _toolPathLine;
        private readonly PathToolArc _toolPathArc;
        private readonly PathToolCubicBezier _toolPathCubicBezier;
        private readonly PathToolQuadraticBezier _toolPathQuadraticBezier;
        private Type _previousPathTool;
        private Type _movePathTool;
        internal bool _isInitialized;
        internal XPath _path;
        internal XPathGeometry _geometry;
        internal XGeometryContext _context;

        /// <inheritdoc/>
        public override string Name => "Path";

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolPath(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _toolPathLine = new PathToolLine(_serviceProvider, this);
            _toolPathArc = new PathToolArc(_serviceProvider, this);
            _toolPathCubicBezier = new PathToolCubicBezier(_serviceProvider, this);
            _toolPathQuadraticBezier = new PathToolQuadraticBezier(_serviceProvider, this);
            _isInitialized = false;
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
            var editor = _serviceProvider.GetService<ProjectEditor>();

            _geometry = XPathGeometry.Create(
                ImmutableArray.Create<XPathFigure>(),
                editor.Project.Options.DefaultFillRule);

            _context = new XPathGeometryContext(_geometry);

            _context.BeginFigure(
                start,
                editor.Project.Options.DefaultIsFilled,
                editor.Project.Options.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary.Selected;
            _path = XPath.Create(
                "Path",
                editor.Project.Options.CloneStyle ? style.Clone() : style,
                _geometry,
                editor.Project.Options.DefaultIsStroked,
                editor.Project.Options.DefaultIsFilled);

            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_path);

            _previousPathTool = editor.CurrentPathTool;
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
            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (_previousPathTool == typeof(PathToolLine))
            {
                RemoveLastSegment<XLineSegment>();
                _toolPathLine.Remove();
            }
            else if (_previousPathTool == typeof(PathToolArc))
            {
                RemoveLastSegment<XArcSegment>();
                _toolPathArc.Remove();
            }
            else if (_previousPathTool == typeof(PathToolCubicBezier))
            {
                RemoveLastSegment<XCubicBezierSegment>();
                _toolPathCubicBezier.Remove();
            }
            else if (_previousPathTool == typeof(PathToolQuadraticBezier))
            {
                RemoveLastSegment<XQuadraticBezierSegment>();
                _toolPathQuadraticBezier.Remove();
            }

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolMove))
            {
                editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                editor.Project.CurrentContainer.HelperLayer.Invalidate();
            }

            if (editor.CurrentPathTool == typeof(PathToolMove))
            {
                _movePathTool = _previousPathTool;
            }

            _previousPathTool = editor.CurrentPathTool;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (_isInitialized && editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
                return;
            }

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.LeftDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolMove))
            {
                double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
                double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;

                // Start new figure.
                var start = editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                _context.BeginFigure(
                    start,
                    editor.Project.Options.DefaultIsFilled,
                    editor.Project.Options.DefaultIsClosed);

                // Switch to path tool before Move tool.
                editor.CurrentPathTool = _movePathTool;
                SwitchPathTool(x, y);
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.RightDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.RightDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.RightDown(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.RightDown(x, y);
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (_isInitialized && editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
            }

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.Move(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.Move(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.Move(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.Move(x, y);
            }
            else if (editor.CurrentPathTool == typeof(PathToolMove))
            {
                double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
                double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
                if (editor.Project.Options.TryToConnect)
                {
                    editor.TryToHoverShape(sx, sy);
                }
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.ToStateOne();
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.ToStateOne();
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.ToStateOne();
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.ToStateOne();
            }
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.ToStateTwo();
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.ToStateTwo();
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.ToStateTwo();
            }
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.ToStateThree();
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathQuadraticBezier.ToStateThree();
            }
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.Move(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.Move(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.Move(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.Move(shape);
            }
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.Finalize(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.Finalize(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.Finalize(shape);
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.Finalize(shape);
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.CurrentPathTool == typeof(PathToolLine))
            {
                _toolPathLine.Remove();
            }
            else if (editor.CurrentPathTool == typeof(PathToolArc))
            {
                _toolPathArc.Remove();
            }
            else if (editor.CurrentPathTool == typeof(PathToolCubicBezier))
            {
                _toolPathCubicBezier.Remove();
            }
            else if (editor.CurrentPathTool == typeof(PathToolQuadraticBezier))
            {
                _toolPathQuadraticBezier.Remove();
            }
        }
    }
}
