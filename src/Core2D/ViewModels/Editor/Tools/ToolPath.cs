using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
using Core2D.Editor.Tools.Path;
using Core2D.Input;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class ToolPath : ViewModelBase, IEditorTool
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PathToolLine _pathToolLine;
        private readonly PathToolArc _pathToolArc;
        private readonly PathToolCubicBezier _pathToolCubicBezier;
        private readonly PathToolQuadraticBezier _pathToolQuadraticBezier;
        private readonly PathToolMove _pathToolMove;

        internal bool IsInitialized { get; set; }

        internal PathShape Path { get; set; }

        internal PathGeometry Geometry { get; set; }

        internal GeometryContext GeometryContext { get; set; }

        internal IPathTool PreviousPathTool { get; set; }

        public string Title => "Path";

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

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

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

        public void DeInitializeWorkingPath()
        {
            IsInitialized = false;
            Geometry = null;
            GeometryContext = null;
            Path = null;
        }

        public void LeftDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.LeftDown(args);
        }

        public void LeftUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.LeftUp(args);
        }

        public void RightDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.RightDown(args);
            Reset();
        }

        public void RightUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.RightUp(args);
        }

        public void Move(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Move(args);
        }

        public void Move(BaseShape shape)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Move(shape);
        }

        public void Finalize(BaseShape shape)
        {
            _serviceProvider.GetService<ProjectEditor>().CurrentPathTool?.Finalize(shape);
        }

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
