using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Path;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Path tool.
    /// </summary>
    public class ToolPath : ObservableObject, IEditorTool
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
        internal PathShape Path { get; set; }

        /// <summary>
        /// Gets or sets current geometry.
        /// </summary>
        internal PathGeometry Geometry { get; set; }

        /// <summary>
        /// Gets or sets current geometry context.
        /// </summary>
        internal GeometryContext GeometryContext { get; set; }

        /// <summary>
        /// Gets or sets previous path tool.
        /// </summary>
        internal PathTool PreviousPathTool { get; set; }

        /// <inheritdoc/>
        public string Title => "Path";

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
            var figure = Geometry?.Figures.LastOrDefault();
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
                _pathToolLine.Reset();
            }
            else if (PreviousPathTool == _pathToolArc)
            {
                RemoveLastSegment<ArcSegment>();
                _pathToolArc.Reset();
            }
            else if (PreviousPathTool == _pathToolCubicBezier)
            {
                RemoveLastSegment<CubicBezierSegment>();
                _pathToolCubicBezier.Reset();
            }
            else if (PreviousPathTool == _pathToolQuadraticBezier)
            {
                RemoveLastSegment<QuadraticBezierSegment>();
                _pathToolQuadraticBezier.Reset();
            }

            var editor = _serviceProvider.GetService<ProjectEditor>();
            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
            editor.Project.CurrentContainer.HelperLayer.InvalidateLayer();
        }

        /// <summary>
        /// Gets last point in the current path.
        /// </summary>
        /// <returns>The last path point.</returns>
        public PointShape GetLastPathPoint()
        {
            var figure = Geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                return (figure.Segments.LastOrDefault()) switch
                {
                    LineSegment line => line.Point,
                    ArcSegment arc => arc.Point,
                    CubicBezierSegment cubic => cubic.Point3,
                    QuadraticBezierSegment quadratic => quadratic.Point2,
                    _ => figure.StartPoint,
                };
            }
            throw new Exception("Can not find valid last point from path.");
        }

        /// <summary>
        /// Initializes working path.
        /// </summary>
        /// <param name="start">The path start point.</param>
        public void InitializeWorkingPath(PointShape start)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();

            Geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigure>(),
                editor.Project.Options.DefaultFillRule);

            GeometryContext = factory.CreateGeometryContext(Geometry);

            GeometryContext.BeginFigure(
                start,
                editor.Project.Options.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                editor.Project.CurrentStyleLibrary.Selected :
                editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            Path = factory.CreatePathShape(
                "Path",
                (ShapeStyle)style.Copy(null),
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
        public void LeftDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.LeftDown(args);
        }

        /// <inheritdoc/>
        public void LeftUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.LeftUp(args);
        }

        /// <inheritdoc/>
        public void RightDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.RightDown(args);
            Reset();
        }

        /// <inheritdoc/>
        public void RightUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.RightUp(args);
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Move(args);
        }

        /// <inheritdoc/>
        public void Move(BaseShape shape)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Move(shape);
        }

        /// <inheritdoc/>
        public void Finalize(BaseShape shape)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Finalize(shape);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Reset();

            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (Path?.Geometry != null)
            {
                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(Path);
                editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();

                if (!(Path.Geometry.Figures.Length == 1) || !(Path.Geometry.Figures[0].Segments.Length <= 1))
                {
                    editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, Path);
                }
            }

            DeInitializeWorkingPath();
        }
    }
}
