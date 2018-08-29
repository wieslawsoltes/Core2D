// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Path;
using Core2D.Editor.Tools.Settings;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Path tool.
    /// </summary>
    public class ToolPath : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsPath _settings;
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
        internal IPathShape Path { get; set; }

        /// <summary>
        /// Gets or sets current geometry.
        /// </summary>
        internal IPathGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets current geometry context.
        /// </summary>
        internal IGeometryContext GeometryContext { get; set; }

        /// <summary>
        /// Gets or sets previous path tool.
        /// </summary>
        internal PathToolBase PreviousPathTool { get; set; }

        /// <inheritdoc/>
        public override string Title => "Path";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsPath Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolPath(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsPath();
            _pathToolLine = serviceProvider.GetService<PathToolLine>();
            _pathToolArc = serviceProvider.GetService<PathToolArc>();
            _pathToolCubicBezier = serviceProvider.GetService<PathToolCubicBezier>();
            _pathToolQuadraticBezier = serviceProvider.GetService<PathToolQuadraticBezier>();
            _pathToolMove = serviceProvider.GetService<PathToolMove>();
            IsInitialized = false;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove last <see cref="PathSegment"/> segment from the previous figure.
        /// </summary>
        /// <typeparam name="T">The type of the path segment to remove.</typeparam>
        public void RemoveLastSegment<T>() where T : PathSegment
        {
            var figure = Geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                if (figure.Segments.LastOrDefault() is T segment)
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
                RemoveLastSegment<LineSegment>();
                _pathToolLine.Remove();
            }
            else if (PreviousPathTool == _pathToolArc)
            {
                RemoveLastSegment<ArcSegment>();
                _pathToolArc.Remove();
            }
            else if (PreviousPathTool == _pathToolCubicBezier)
            {
                RemoveLastSegment<CubicBezierSegment>();
                _pathToolCubicBezier.Remove();
            }
            else if (PreviousPathTool == _pathToolQuadraticBezier)
            {
                RemoveLastSegment<QuadraticBezierSegment>();
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
        public IPointShape GetLastPathPoint()
        {
            var figure = Geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                switch (figure.Segments.LastOrDefault())
                {
                    case LineSegment line:
                        return line.Point;
                    case ArcSegment arc:
                        return arc.Point;
                    case CubicBezierSegment cubic:
                        return cubic.Point3;
                    case QuadraticBezierSegment quadratic:
                        return quadratic.Point2;
                    default:
                    case null:
                        return figure.StartPoint;
                }
            }
            throw new Exception("Can not find valid last point from path.");
        }

        /// <summary>
        /// Initializes working path.
        /// </summary>
        /// <param name="start">The path start point.</param>
        public void InitializeWorkingPath(IPointShape start)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();

            Geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<IPathFigure>(),
                editor.Project.Options.DefaultFillRule);

            GeometryContext = new PathGeometryContext(Geometry);

            GeometryContext.BeginFigure(
                start,
                editor.Project.Options.DefaultIsFilled,
                editor.Project.Options.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary.Selected;
            Path = factory.CreatePathShape(
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
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.LeftDown(args);
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.RightDown(args);
        }

        /// <inheritdoc/>
        public override void Move(InputArgs args)
        {
            base.Move(args);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Move(args);
        }

        /// <inheritdoc/>
        public override void Move(IBaseShape shape)
        {
            base.Move(shape);
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool.Move(shape);
        }

        /// <inheritdoc/>
        public override void Finalize(IBaseShape shape)
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
