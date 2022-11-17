#nullable enable
using System;
using System.Collections.Generic;
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

namespace Core2D.ViewModels.Editor.Tools;

public partial class PathToolViewModel : ViewModelBase, IEditorTool
{
    private readonly LinePathToolViewModel? _linePathTool;
    private readonly ArcPathToolViewModel? _arcPathTool;
    private readonly CubicBezierPathToolViewModel? _cubicBezierPathTool;
    private readonly QuadraticBezierPathToolViewModel? _quadraticBezierPathTool;
    private readonly MovePathToolViewModel? _movePathTool;

    internal bool IsInitialized { get; set; }

    internal PathShapeViewModel? Path { get; set; }

    internal GeometryContext? GeometryContext { get; set; }

    internal IPathTool? PreviousPathTool { get; set; }

    public string Title => "Path";

    public PathToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _linePathTool = serviceProvider.GetService<LinePathToolViewModel>();
        _arcPathTool = serviceProvider.GetService<ArcPathToolViewModel>();
        _cubicBezierPathTool = serviceProvider.GetService<CubicBezierPathToolViewModel>();
        _quadraticBezierPathTool = serviceProvider.GetService<QuadraticBezierPathToolViewModel>();
        _movePathTool = serviceProvider.GetService<MovePathToolViewModel>();
        IsInitialized = false;
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public void RemoveLastSegment<T>() where T : PathSegmentViewModel
    {
        var figure = Path?.Figures.LastOrDefault();
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
            _linePathTool?.Reset();
        }
        else if (PreviousPathTool == _arcPathTool)
        {
            RemoveLastSegment<ArcSegmentViewModel>();
            _arcPathTool?.Reset();
        }
        else if (PreviousPathTool == _cubicBezierPathTool)
        {
            RemoveLastSegment<CubicBezierSegmentViewModel>();
            _cubicBezierPathTool?.Reset();
        }
        else if (PreviousPathTool == _quadraticBezierPathTool)
        {
            RemoveLastSegment<QuadraticBezierSegmentViewModel>();
            _quadraticBezierPathTool?.Reset();
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        editor?.Project?.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
        editor?.Project?.CurrentContainer?.HelperLayer?.RaiseInvalidateLayer();
    }

    public PointShapeViewModel GetLastPathPoint()
    {
        var figure = Path?.Figures.LastOrDefault();
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
        var factory = ServiceProvider.GetService<IViewModelFactory>();
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

        var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
            editor.Project.CurrentStyleLibrary.Selected :
            viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        Path = factory.CreatePathShape(
            "",
            (ShapeStyleViewModel)style.Copy(null),
            ImmutableArray.Create<PathFigureViewModel>(),
            editor.Project.Options.DefaultFillRule,
            editor.Project.Options.DefaultIsStroked,
            editor.Project.Options.DefaultIsFilled);

        GeometryContext = factory.CreateGeometryContext(Path);

        GeometryContext.BeginFigure(
            start,
            editor.Project.Options.DefaultIsClosed);

        editor.SetShapeName(Path);

        if (editor.Project.CurrentContainer?.WorkingLayer is { })
        {
            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(Path);
        }

        PreviousPathTool = editor.CurrentPathTool;
        IsInitialized = true;
    }

    public void DeInitializeWorkingPath()
    {
        IsInitialized = false;
        GeometryContext = null;
        Path = null;
    }

    public void BeginDown(InputArgs args)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.BeginDown(args);
    }

    public void BeginUp(InputArgs args)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.BeginUp(args);
    }

    public void EndDown(InputArgs args)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.EndDown(args);
        Reset();
    }

    public void EndUp(InputArgs args)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.EndUp(args);
    }

    public void Move(InputArgs args)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.Move(args);
    }

    public void Move(BaseShapeViewModel? shape)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.Move(shape);
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.Finalize(shape);
    }

    public void Reset()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CurrentPathTool?.Reset();

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        if (Path is { })
        {
            if (editor.Project.CurrentContainer?.WorkingLayer is { })
            {
                editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(Path);
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
            }

            if (Path.Figures.Length != 1 || !(Path.Figures[0].Segments.Length <= 1))
            {
                if (editor.Project.CurrentContainer is { })
                {
                    editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, Path);
                }
            }
        }

        DeInitializeWorkingPath();
    }
}
