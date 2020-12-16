using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Path;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class PathToolViewModel : ViewModelBase, IEditorTool
    {
        private readonly LinePathToolViewModel _linePathTool;
        private readonly ArcPathToolViewModel _arcPathTool;
        private readonly CubicBezierPathToolViewModel _cubicBezierPathTool;
        private readonly QuadraticBezierPathToolViewModel _quadraticBezierPathTool;
        private readonly MovePathToolViewModel _movePathTool;

        internal bool IsInitialized { get; set; }

        internal PathShapeViewModel Path { get; set; }

        internal PathGeometryViewModel Geometry { get; set; }

        internal GeometryContext GeometryContext { get; set; }

        internal IPathTool PreviousPathTool { get; set; }

        public string Title => "Path";

        public PathToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _linePathTool = serviceProvider.GetService<LinePathToolViewModel>();
            _arcPathTool = serviceProvider.GetService<ArcPathToolViewModel>();
            _cubicBezierPathTool = serviceProvider.GetService<CubicBezierPathToolViewModel>();
            _quadraticBezierPathTool = serviceProvider.GetService<QuadraticBezierPathToolViewModel>();
            _movePathTool = serviceProvider.GetService<MovePathToolViewModel>();
            IsInitialized = false;
        }

        public void RemoveLastSegment<T>() where T : PathSegmentViewModel
        {
            var figure = Geometry?.Figures.LastOrDefault();
            if (figure?.Segments.LastOrDefault() is T segment)
            {
                figure.Segments = figure.Segments.Remove(segment);
            }
        }

        public void RemoveLastSegment()
        {
            if (PreviousPathTool == _linePathTool)
            {
                RemoveLastSegment<LineSegmentViewModel>();
                _linePathTool.Reset();
            }
            else if (PreviousPathTool == _arcPathTool)
            {
                RemoveLastSegment<ArcSegmentViewModel>();
                _arcPathTool.Reset();
            }
            else if (PreviousPathTool == _cubicBezierPathTool)
            {
                RemoveLastSegment<CubicBezierSegmentViewModel>();
                _cubicBezierPathTool.Reset();
            }
            else if (PreviousPathTool == _quadraticBezierPathTool)
            {
                RemoveLastSegment<QuadraticBezierSegmentViewModel>();
                _quadraticBezierPathTool.Reset();
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
            editor.Project.CurrentContainer.HelperLayer.InvalidateLayer();
        }

        public PointShapeViewModel GetLastPathPoint()
        {
            var figure = Geometry.Figures.LastOrDefault();
            if (figure is { })
            {
                return (figure.Segments.LastOrDefault()) switch
                {
                    LineSegmentViewModel line => line.Point,
                    ArcSegmentViewModel arc => arc.Point,
                    CubicBezierSegmentViewModel cubic => cubic.Point3,
                    QuadraticBezierSegmentViewModel quadratic => quadratic.Point2,
                    _ => figure.StartPoint,
                };
            }
            throw new Exception("Can not find valid last point from path.");
        }

        public void InitializeWorkingPath(PointShapeViewModel start)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            Geometry = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
                editor.Project.Options.DefaultFillRule);

            GeometryContext = factory.CreateGeometryContext(Geometry);

            GeometryContext.BeginFigure(
                start,
                editor.Project.Options.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                editor.Project.CurrentStyleLibrary.Selected :
                editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            Path = factory.CreatePathShape(
                editor.GetShapeName<PathShapeViewModel>(),
                (ShapeStyleViewModel)style.Copy(null),
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

        public void BeginDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.BeginDown(args);
        }

        public void BeginUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.BeginUp(args);
        }

        public void EndDown(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.EndDown(args);
            Reset();
        }

        public void EndUp(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.EndUp(args);
        }

        public void Move(InputArgs args)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Move(args);
        }

        public void Move(BaseShapeViewModel shape)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Move(shape);
        }

        public void Finalize(BaseShapeViewModel shape)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Finalize(shape);
        }

        public void Reset()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Reset();

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (Path?.Geometry is { })
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
