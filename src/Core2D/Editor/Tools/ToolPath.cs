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
    /// Path tool.
    /// </summary>
    public class ToolPath : ToolBase
    {
        public enum ToolState { None, One, Two, Three }
        private readonly IServiceProvider _serviceProvider;
        private readonly PathToolLine _pathToolLine;
        private readonly PathToolArc _pathToolArc;
        private readonly PathToolCubicBezier _pathToolCubicBezier;
        private readonly PathToolQuadraticBezier _pathToolQuadraticBezier;
        private readonly PathToolMove _pathToolMove;

        /// <summary>
        /// Gets or sets flag indicating whether path was initialized.
        /// </summary>
        internal bool IsInitialized { get; set; }

        /// <summary>
        /// Gets or sets current path.
        /// </summary>
        internal XPath Path { get; set; }

        /// <summary>
        /// Gets or sets current geometry.
        /// </summary>
        internal XPathGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets current geometry context.
        /// </summary>
        internal XGeometryContext GeometryContext { get; set; }

        /// <summary>
        /// Gets or sets previous path tool.
        /// </summary>
        internal PathToolBase PreviousPathTool { get; set; }

        /// <inheritdoc/>
        public override string Name => "Path";

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolPath(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _pathToolLine = serviceProvider.GetService<PathToolLine>();
            _pathToolArc = serviceProvider.GetService<PathToolArc>();
            _pathToolCubicBezier = serviceProvider.GetService<PathToolCubicBezier>();
            _pathToolQuadraticBezier = serviceProvider.GetService<PathToolQuadraticBezier>();
            _pathToolMove = serviceProvider.GetService<PathToolMove>();
            IsInitialized = false;
        }

        /// <summary>
        /// Remove last <see cref="XPathSegment"/> segment from the previous figure.
        /// </summary>
        /// <typeparam name="T">The type of the path segment to remove.</typeparam>
        public void RemoveLastSegment<T>() where T : XPathSegment
        {
            var figure = Geometry.Figures.LastOrDefault();
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
        /// Remove last segment from the previous figure.
        /// </summary>
        public void RemoveLastSegment()
        {
            if (PreviousPathTool == _pathToolLine)
            {
                RemoveLastSegment<XLineSegment>();
                _pathToolLine.Remove();
            }
            else if (PreviousPathTool == _pathToolArc)
            {
                RemoveLastSegment<XArcSegment>();
                _pathToolArc.Remove();
            }
            else if (PreviousPathTool == _pathToolCubicBezier)
            {
                RemoveLastSegment<XCubicBezierSegment>();
                _pathToolCubicBezier.Remove();
            }
            else if (PreviousPathTool == _pathToolQuadraticBezier)
            {
                RemoveLastSegment<XQuadraticBezierSegment>();
                _pathToolQuadraticBezier.Remove();
            }

            var editor = _serviceProvider.GetService<ProjectEditor>();
            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
            editor.Project.CurrentContainer.HelperLayer.Invalidate();
        }

        /// <summary>
        /// Gets last point in the current path.
        /// </summary>
        /// <returns>The last path point.</returns>
        public XPoint GetLastPathPoint()
        {
            var figure = Geometry.Figures.LastOrDefault();
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

        /// <summary>
        /// Initializes working path.
        /// </summary>
        /// <param name="start">The path start point.</param>
        public void InitializeWorkingPath(XPoint start)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();

            Geometry = XPathGeometry.Create(
                ImmutableArray.Create<XPathFigure>(),
                editor.Project.Options.DefaultFillRule);

            GeometryContext = new XPathGeometryContext(Geometry);

            GeometryContext.BeginFigure(
                start,
                editor.Project.Options.DefaultIsFilled,
                editor.Project.Options.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary.Selected;
            Path = XPath.Create(
                "Path",
                editor.Project.Options.CloneStyle ? style.Clone() : style,
                Geometry,
                editor.Project.Options.DefaultIsStroked,
                editor.Project.Options.DefaultIsFilled);

            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(Path);

            PreviousPathTool = editor.CurrentPathTool;
            IsInitialized = true;
        }

        /// <summary>
        ///  De-initializes working path.
        /// </summary>
        public void DeInitializeWorkingPath()
        {
            IsInitialized = false;
            Geometry = null;
            GeometryContext = null;
            Path = null;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.LeftDown(x, y);
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.RightDown(x, y);
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Move(x, y);
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.One"/>.
        /// </summary>
        public void ToStateOne()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (editor.CurrentPathTool)
            {
                case PathToolLine tool:
                    tool.ToStateOne();
                    break;
                case PathToolCubicBezier tool:
                    tool.ToStateOne();
                    break;
                case PathToolQuadraticBezier tool:
                    tool.ToStateOne();
                    break;
                case PathToolArc tool:
                    tool.ToStateOne();
                    break;
                case PathToolMove tool:
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.Two"/>.
        /// </summary>
        public void ToStateTwo()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (editor.CurrentPathTool)
            {
                case PathToolLine tool:
                    break;
                case PathToolCubicBezier tool:
                    tool.ToStateTwo();
                    break;
                case PathToolQuadraticBezier tool:
                    tool.ToStateTwo();
                    break;
                case PathToolArc tool:
                    break;
                case PathToolMove tool:
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.Three"/>.
        /// </summary>
        public void ToStateThree()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (editor.CurrentPathTool)
            {
                case PathToolLine tool:
                    break;
                case PathToolCubicBezier tool:
                    tool.ToStateThree();
                    break;
                case PathToolQuadraticBezier tool:
                    break;
                case PathToolArc tool:
                    break;
                case PathToolMove tool:
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Move(shape);
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Finalize(shape);
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Remove();
        }
    }
}
