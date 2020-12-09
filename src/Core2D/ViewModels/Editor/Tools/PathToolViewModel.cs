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
    public class PathToolViewModel : ViewModelBase, IEditorTool
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LinePathToolViewModel _linePathToolViewModel;
        private readonly ArcPathToolViewModel _arcPathToolViewModel;
        private readonly CubicBezierPathToolViewModel _cubicBezierPathToolViewModel;
        private readonly QuadraticBezierPathToolViewModel _quadraticBezierPathToolViewModel;
        private readonly MovePathToolViewModel _movePathToolViewModel;

        internal bool IsInitialized { get; set; }

        internal PathShapeViewModel Path { get; set; }

        internal PathGeometryViewModel GeometryViewModel { get; set; }

        internal GeometryContext GeometryContext { get; set; }

        internal IPathTool PreviousPathTool { get; set; }

        public string Title => "Path";

        public PathToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _linePathToolViewModel = serviceProvider.GetService<LinePathToolViewModel>();
            _arcPathToolViewModel = serviceProvider.GetService<ArcPathToolViewModel>();
            _cubicBezierPathToolViewModel = serviceProvider.GetService<CubicBezierPathToolViewModel>();
            _quadraticBezierPathToolViewModel = serviceProvider.GetService<QuadraticBezierPathToolViewModel>();
            _movePathToolViewModel = serviceProvider.GetService<MovePathToolViewModel>();
            IsInitialized = false;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void RemoveLastSegment<T>() where T : PathSegmentViewModel
        {
            var figure = GeometryViewModel?.Figures.LastOrDefault();
            if (figure?.Segments.LastOrDefault() is T segment)
            {
                figure.Segments = figure.Segments.Remove(segment);
            }
        }

        public void RemoveLastSegment()
        {
            if (PreviousPathTool == _linePathToolViewModel)
            {
                RemoveLastSegment<LineSegmentViewModel>();
                _linePathToolViewModel.Reset();
            }
            else if (PreviousPathTool == _arcPathToolViewModel)
            {
                RemoveLastSegment<ArcSegmentViewModel>();
                _arcPathToolViewModel.Reset();
            }
            else if (PreviousPathTool == _cubicBezierPathToolViewModel)
            {
                RemoveLastSegment<CubicBezierSegmentViewModel>();
                _cubicBezierPathToolViewModel.Reset();
            }
            else if (PreviousPathTool == _quadraticBezierPathToolViewModel)
            {
                RemoveLastSegment<QuadraticBezierSegmentViewModel>();
                _quadraticBezierPathToolViewModel.Reset();
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
            editor.Project.CurrentContainerViewModel.HelperLayer.InvalidateLayer();
        }

        public PointShapeViewModel GetLastPathPoint()
        {
            var figure = GeometryViewModel.Figures.LastOrDefault();
            if (figure != null)
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

            GeometryViewModel = factory.CreatePathGeometry(
                ImmutableArray.Create<PathFigureViewModel>(),
                editor.Project.OptionsViewModel.DefaultFillRule);

            GeometryContext = factory.CreateGeometryContext(GeometryViewModel);

            GeometryContext.BeginFigure(
                start,
                editor.Project.OptionsViewModel.DefaultIsClosed);

            var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                editor.Project.CurrentStyleLibrary.Selected :
                editor.Factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            Path = factory.CreatePathShape(
                "Path",
                (ShapeStyleViewModel)style.Copy(null),
                GeometryViewModel,
                editor.Project.OptionsViewModel.DefaultIsStroked,
                editor.Project.OptionsViewModel.DefaultIsFilled);

            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(Path);

            PreviousPathTool = editor.CurrentPathTool;
            IsInitialized = true;
        }

        public void DeInitializeWorkingPath()
        {
            IsInitialized = false;
            GeometryViewModel = null;
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

        public void Move(BaseShapeViewModel shapeViewModel)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Move(shapeViewModel);
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Finalize(shapeViewModel);
        }

        public void Reset()
        {
            _serviceProvider.GetService<ProjectEditorViewModel>().CurrentPathTool?.Reset();

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (Path?.GeometryViewModel != null)
            {
                editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(Path);
                editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();

                if (!(Path.GeometryViewModel.Figures.Length == 1) || !(Path.GeometryViewModel.Figures[0].Segments.Length <= 1))
                {
                    editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, Path);
                }
            }

            DeInitializeWorkingPath();
        }
    }
}
