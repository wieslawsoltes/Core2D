#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.Model.Renderer;
using Core2D.Spatial;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools;

public partial class SelectionToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { None, Selected }
    private State _currentState = State.None;
    private RectangleShapeViewModel? _rectangleShape;
    private decimal _startX;
    private decimal _startY;
    private decimal _historyX;
    private decimal _historyY;
    private IEnumerable<PointShapeViewModel>? _pointsCache;
    private IEnumerable<BaseShapeViewModel>? _shapesCache;

    public string Title => "Selection";

    public SelectionToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private static bool IsPointMovable(BaseShapeViewModel shape, PointShapeViewModel point)
    {
        if (point.State.HasFlag(ShapeStateFlags.Locked) || (point.Owner is BaseShapeViewModel ower && ower.State.HasFlag(ShapeStateFlags.Locked)))
        {
            return false;
        }

        if (point.State.HasFlag(ShapeStateFlags.Connector) && point.Owner != shape)
        {
            return false;
        }

        return true;
    }

    private static IEnumerable<PointShapeViewModel> GetMovePoints(IEnumerable<BaseShapeViewModel> shapes)
    {
        var points = new List<PointShapeViewModel>();

        foreach (var shape in shapes)
        {
            shape.GetPoints(points);
        }

        return points.Where(p =>
        {
            if (p.Owner is BaseShapeViewModel baseShapeViewModel)
            {
                
                return IsPointMovable(baseShapeViewModel, p);
            }

            return false;
        }).Distinct();
    }

    private void GenerateMoveSelectionCache()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project?.Options is null)
        {
            return;
        }
            
        if (editor.Project.SelectedShapes is { })
        {
            var shapes = editor.Project.SelectedShapes
                .Where(s => !s.State.HasFlag(ShapeStateFlags.Locked)
                            && !s.State.HasFlag(ShapeStateFlags.Connector));

            switch (editor.Project.Options.MoveMode)
            {
                case MoveMode.Point:
                {
                    _pointsCache = GetMovePoints(shapes).ToList();
                    break;
                }
                case MoveMode.Shape:
                {
                    _shapesCache = shapes.ToList();
                    break;
                }
            }
        }
    }

    private void DisposeMoveSelectionCache()
    {
        _pointsCache = null;
        _shapesCache = null;
    }

    private void MoveSelectionCacheTo(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var shapeService = ServiceProvider.GetService<IShapeService>();

        if (editor is null || selection is null || shapeService is null)
        {
            return;
        }

        var (sx, sy) = selection.TryToSnap(args);
        var dx = sx - _startX;
        var dy = sy - _startY;

        _startX = sx;
        _startY = sy;

        if (_pointsCache is { })
        {
            shapeService.MoveShapesBy(_pointsCache, dx, dy);
        }

        if (_shapesCache is { })
        {
            shapeService.MoveShapesBy(_shapesCache, dx, dy);
        }
    }

    private bool TryConnectMovedPoints()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var hitTest = ServiceProvider.GetService<IHitTest>();

        if (editor?.Project is null || hitTest is null || _pointsCache is null)
        {
            return false;
        }

        var project = editor.Project;
        var options = project.Options;
        if (options is null || !options.TryToConnect)
        {
            return false;
        }

        var layer = project.CurrentContainer?.CurrentLayer;
        var pageState = editor.Renderer?.State;
        if (layer is null || pageState is null)
        {
            return false;
        }

        var movingPoints = _pointsCache.ToList();
        if (movingPoints.Count == 0)
        {
            return false;
        }

        var shapes = layer.Shapes.Reverse().ToList();
        var movingSet = new HashSet<PointShapeViewModel>(movingPoints);
        var radius = options.HitThreshold / pageState.ZoomX;
        var scale = pageState.ZoomX;

        var connected = false;

        foreach (var point in movingPoints)
        {
            var target = FindConnectionTarget(point, movingSet, shapes, hitTest, radius, scale);
            if (target is null)
            {
                continue;
            }

            if (TryConnectPoint(project, point, target))
            {
                UpdateSelectionAfterConnect(project, point, target);
                connected = true;
            }
        }

        if (connected)
        {
            layer.RaiseInvalidateLayer();
        }

        return connected;
    }

    private static PointShapeViewModel? FindConnectionTarget(
        PointShapeViewModel point,
        ISet<PointShapeViewModel> movingPoints,
        IList<BaseShapeViewModel> shapes,
        IHitTest hitTest,
        double radius,
        double scale)
    {
        var location = new Point2(point.X, point.Y);

        foreach (var shape in shapes)
        {
            var candidate = hitTest.TryToGetPoint(shape, location, radius, scale);
            if (candidate is null)
            {
                continue;
            }

            if (ReferenceEquals(candidate, point))
            {
                continue;
            }

            if (movingPoints.Contains(candidate))
            {
                continue;
            }

            return candidate;
        }

        return null;
    }

    private bool TryConnectPoint(ProjectContainerViewModel project, PointShapeViewModel movingPoint, PointShapeViewModel target)
    {
        if (ReferenceEquals(movingPoint, target))
        {
            return false;
        }

        var selectionService = ServiceProvider.GetService<ISelectionService>();

        switch (movingPoint.Owner)
        {
            case LineShapeViewModel line:
                if (ReplacePoint(project, movingPoint, target, line.Start, p => line.Start = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, line.End, p => line.End = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case ArcShapeViewModel arc:
                if (ReplacePoint(project, movingPoint, target, arc.Point1, p => arc.Point1 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, arc.Point2, p => arc.Point2 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, arc.Point3, p => arc.Point3 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, arc.Point4, p => arc.Point4 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case QuadraticBezierShapeViewModel quadratic:
                if (ReplacePoint(project, movingPoint, target, quadratic.Point1, p => quadratic.Point1 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, quadratic.Point2, p => quadratic.Point2 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, quadratic.Point3, p => quadratic.Point3 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case CubicBezierShapeViewModel cubic:
                if (ReplacePoint(project, movingPoint, target, cubic.Point1, p => cubic.Point1 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, cubic.Point2, p => cubic.Point2 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, cubic.Point3, p => cubic.Point3 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, cubic.Point4, p => cubic.Point4 = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case RectangleShapeViewModel rectangle:
                if (ReplacePoint(project, movingPoint, target, rectangle.TopLeft, p => rectangle.TopLeft = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, rectangle.BottomRight, p => rectangle.BottomRight = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case EllipseShapeViewModel ellipse:
                if (ReplacePoint(project, movingPoint, target, ellipse.TopLeft, p => ellipse.TopLeft = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, ellipse.BottomRight, p => ellipse.BottomRight = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case TextShapeViewModel text:
                if (ReplacePoint(project, movingPoint, target, text.TopLeft, p => text.TopLeft = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, text.BottomRight, p => text.BottomRight = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
            case ImageShapeViewModel image:
                if (ReplacePoint(project, movingPoint, target, image.TopLeft, p => image.TopLeft = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                if (ReplacePoint(project, movingPoint, target, image.BottomRight, p => image.BottomRight = p))
                {
                    selectionService?.RememberConnectionPoint(target);
                    return true;
                }
                break;
        }

        return false;
    }

    private static bool ReplacePoint(
        ProjectContainerViewModel project,
        PointShapeViewModel movingPoint,
        PointShapeViewModel target,
        PointShapeViewModel? ownerPoint,
        Action<PointShapeViewModel?> setter)
    {
        if (ownerPoint is null || !ReferenceEquals(ownerPoint, movingPoint) || ReferenceEquals(ownerPoint, target))
        {
            return false;
        }

        project.History?.Snapshot(ownerPoint, target, setter);
        setter(target);
        return true;
    }

    private static void UpdateSelectionAfterConnect(ProjectContainerViewModel project, PointShapeViewModel movingPoint, PointShapeViewModel target)
    {
        if (project.SelectedShapes is not { } selected || !selected.Contains(movingPoint))
        {
            return;
        }

        var updated = new HashSet<BaseShapeViewModel>(selected);
        updated.Remove(movingPoint);
        updated.Add(target);
        project.SelectedShapes = updated;
    }

    private bool IsSelectionAvailable()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        return editor?.Project?.SelectedShapes is { };
    }

    private bool HitTestDecorator(InputArgs args, bool isControl, bool isHover)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();

        if (editor?.PageState is null)
        {
            return false;
        }

        if (isControl == false && editor.PageState.Decorator is { } && editor.PageState.Decorator.IsVisible)
        {
            var decoratorResult = editor.PageState.Decorator.HitTest(args);
            if (decoratorResult && isHover == false)
            {
                editor.IsToolIdle = false;
                _currentState = State.Selected;
                return true;
            }
        }

        return false;
    }

    private void Start(InputArgs args)
    {
        var factory = ServiceProvider.GetService<IViewModelFactory>();
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var hitTest = ServiceProvider.GetService<IHitTest>();
        if (factory is null || editor?.Project is null || selection is null || hitTest is null)
        {
            return;
        }

        if (editor.Project.CurrentContainer?.CurrentLayer is null ||
            editor.Project.CurrentContainer?.WorkingLayer is null)
        {
            return;
        }

        var (x, y) = args;
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.None:
            {
                if (editor.PageState is null)
                {
                    return;
                }

                bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                selection.DeHover(editor.Project.CurrentContainer.CurrentLayer);

                if (isControl == false && editor.PageState.DrawDecorators && editor.PageState.Decorator is { } &&
                    editor.PageState.Decorator.IsVisible)
                {
                    if (HitTestDecorator(args, isControl, false))
                    {
                        return;
                    }
                }

                if (isControl)
                {
                    var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse().ToList();
                    var hitThreshold = editor.Project.Options?.HitThreshold ?? 7d;
                    var radius = hitThreshold / editor.PageState.ZoomX;
                    BaseShapeViewModel? result =
                        hitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                    if (result is null)
                    {
                        result = hitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                    }

                    if (result is { })
                    {
                        if (editor.Project.SelectedShapes is null)
                        {
                            editor.Project.SelectedShapes = new HashSet<BaseShapeViewModel>() {result};
                            editor.Project.CurrentContainer.CurrentLayer.RaiseInvalidateLayer();
                            selection.OnShowOrHideDecorator();
                            HitTestDecorator(args, isControl, false);
                            break;
                        }
                        else if (editor.Project.SelectedShapes is { })
                        {
                            if (editor.Project.SelectedShapes.Contains(result))
                            {
                                var selected = new HashSet<BaseShapeViewModel>(editor.Project.SelectedShapes);
                                selected.Remove(result);

                                if (selected.Count == 0)
                                {
                                    editor.Project.SelectedShapes = null;
                                    selection.OnHideDecorator();
                                }
                                else
                                {
                                    editor.Project.SelectedShapes = selected;
                                    selection.OnShowOrHideDecorator();
                                    HitTestDecorator(args, isControl, false);
                                }

                                editor.Project.CurrentContainer.CurrentLayer.RaiseInvalidateLayer();
                                break;
                            }
                            else
                            {
                                var selected = new HashSet<BaseShapeViewModel>(editor.Project.SelectedShapes);
                                selected.Add(result);

                                editor.Project.SelectedShapes = selected;

                                editor.Project.CurrentContainer.CurrentLayer.RaiseInvalidateLayer();
                                selection.OnShowOrHideDecorator();
                                HitTestDecorator(args, isControl, false);
                                break;
                            }
                        }
                    }
                }

                if (isControl == false && editor.PageState.DrawDecorators && editor.PageState.Decorator is { } &&
                    editor.PageState.Decorator.IsVisible)
                {
                    selection.OnHideDecorator();
                }

                if (editor.Project.SelectedShapes is { })
                {
                    var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse().ToList();
                    var hitThreshold = editor.Project.Options?.HitThreshold ?? 7d;
                    var radius = hitThreshold / editor.PageState.ZoomX;
                    BaseShapeViewModel? result =
                        hitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                    if (result is null)
                    {
                        result = hitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                    }

                    if (result is { } && editor.Project.SelectedShapes.Contains(result))
                    {
                        editor.IsToolIdle = false;
                        _startX = sx;
                        _startY = sy;
                        _historyX = _startX;
                        _historyY = _startY;
                        GenerateMoveSelectionCache();
                        _currentState = State.Selected;
                        selection.OnShowOrHideDecorator();
                        HitTestDecorator(args, isControl, false);
                        break;
                    }
                }

                var deselect = !isControl;

                if (selection.TryToSelectShape(editor.Project.CurrentContainer.CurrentLayer, x, y, deselect))
                {
                    editor.IsToolIdle = false;
                    _startX = sx;
                    _startY = sy;
                    _historyX = _startX;
                    _historyY = _startY;
                    GenerateMoveSelectionCache();
                    _currentState = State.Selected;
                    selection.OnShowOrHideDecorator();
                    HitTestDecorator(args, isControl, false);
                    break;
                }

                editor.IsToolIdle = false;
                _rectangleShape = factory.CreateRectangleShape(
                    x, y,
                    editor.PageState.SelectionStyle,
                    true, true);
                _rectangleShape.State |= ShapeStateFlags.Thickness;

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes =
                        editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangleShape);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                _currentState = State.Selected;
                break;
            }
            case State.Selected:
            {
                if (_rectangleShape is { })
                {
                    if (_rectangleShape.BottomRight is { })
                    {
                        _rectangleShape.BottomRight.X = x;
                        _rectangleShape.BottomRight.Y = y;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes =
                            editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangleShape);
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }

                    _currentState = State.None;
                    editor.IsToolIdle = true;
                }

                break;
            }
        }
    }

    private void End(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var shapeService = ServiceProvider.GetService<IShapeService>();
        if (editor?.Project is null || selection is null || shapeService is null)
        {
            return;
        }

        if (editor.Project.CurrentContainer?.CurrentLayer is null ||
            editor.Project.CurrentContainer?.WorkingLayer is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.None:
                break;
            case State.Selected:
            {
                bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                if (_rectangleShape is { })
                {
                    if (_rectangleShape.BottomRight is { })
                    {
                        _rectangleShape.BottomRight.X = args.X;
                        _rectangleShape.BottomRight.Y = args.Y;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes =
                            editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangleShape);
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }
                }

                if (IsSelectionAvailable() && !isControl)
                {
                    var (sx, sy) = selection.TryToSnap(args);
                    if (_historyX != sx || _historyY != sy)
                    {
                        var dx = sx - _historyX;
                        var dy = sy - _historyY;

                        var previous = new
                        {
                            DeltaX = -dx,
                            DeltaY = -dy,
                            Points = _pointsCache,
                            Shapes = _shapesCache
                        };
                        var next = new
                        {
                            DeltaX = dx,
                            DeltaY = dy,
                            Points = _pointsCache,
                            Shapes = _shapesCache
                        };
                        editor.Project?.History?.Snapshot(previous, next,
                            state =>
                            {
                                if (state.Points is { })
                                {
                                    shapeService.MoveShapesBy(state.Points, state.DeltaX, state.DeltaY);
                                }

                                if (state.Shapes is { })
                                {
                                    shapeService.MoveShapesBy(state.Shapes, state.DeltaX, state.DeltaY);
                                }
                            });
                    }

                    var didConnect = TryConnectMovedPoints();
                    DisposeMoveSelectionCache();
                    _currentState = State.None;
                    editor.IsToolIdle = true;

                    if (didConnect)
                    {
                        selection.OnUpdateDecorator();
                    }

                    break;
                }

                var deselect = !isControl;
                var includeSelected = isControl;

                if (editor.Project.CurrentContainer?.CurrentLayer is { } && _rectangleShape is { })
                {
                    _currentState = State.None;
                    selection.TryToSelectShapes(editor.Project.CurrentContainer.CurrentLayer, _rectangleShape, deselect,
                        includeSelected);
                    selection.OnShowOrHideDecorator();
                    editor.IsToolIdle = true;
                }

                break;
            }
        }
    }

    public void BeginDown(InputArgs args)
    {
        Start(args);
    }

    public void BeginUp(InputArgs args)
    {
        End(args);
    }

    public void EndDown(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();

        if (editor?.Project is null || selection is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.None:
            {
                if (editor.Project.CurrentContainer?.CurrentLayer is { })
                {
                    selection.DeHover(editor.Project.CurrentContainer.CurrentLayer);
                }
                break;
            }
            case State.Selected:
            {
                DisposeMoveSelectionCache();
                selection.OnHideDecorator();
                editor.IsToolIdle = true;
                break;
            }
        }
    }

    public void EndUp(InputArgs args)
    {
    }

    public void Move(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        if (editor?.Project?.Options is null || selection is null)
        {
            return;
        }
        switch (_currentState)
        {
            case State.None:
            {
                if (editor.PageState is null)
                {
                    return;
                }

                bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                if (!isControl)
                {
                    selection.TryToHoverShape(args.X, args.Y);
                    HitTestDecorator(args, isControl, true);
                }
                break;
            }
            case State.Selected:
            {
                bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                if (isControl == false 
                    && editor.PageState is { } 
                    && editor.PageState.DrawDecorators 
                    && editor.PageState.Decorator is { } 
                    && editor.PageState.Decorator.IsVisible)
                {
                    editor.PageState.Decorator.Move(args);
                    editor.PageState.Decorator.Update(false);
                    return;
                }

                HitTestDecorator(args, isControl, true);

                if (IsSelectionAvailable() && !isControl)
                {
                    MoveSelectionCacheTo(args);
                    selection.OnUpdateDecorator();
                    if (editor.Project.CurrentContainer?.CurrentLayer is { })
                    {
                        editor.Project.CurrentContainer.CurrentLayer.RaiseInvalidateLayer();
                    }
                    break;
                }

                if (_rectangleShape is { })
                {
                    if (_rectangleShape.BottomRight is { })
                    {
                        _rectangleShape.BottomRight.X = args.X;
                        _rectangleShape.BottomRight.Y = args.Y;
                    }
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                break;
            }
        }
    }

    public void Move(BaseShapeViewModel? shape)
    {
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
    }

    public void Reset()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();

        if (editor is null || selection is null)
        {
            return;
        }

        _currentState = State.None;

        selection.DeHover(editor.Project?.CurrentContainer?.CurrentLayer);

        DisposeMoveSelectionCache();
        selection.OnHideDecorator();

        editor.Project?.CurrentContainer?.CurrentLayer?.RaiseInvalidateLayer();
        editor.Project?.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();

        editor.IsToolIdle = true;
    }
}
