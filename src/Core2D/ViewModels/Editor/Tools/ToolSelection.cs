using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Containers;
using Core2D.Input;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Tools
{
    public class ToolSelection : ObservableObject, IEditorTool
    {
        public enum State { None, Selected }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.None;
        private RectangleShape _rectangleShape;
        private decimal _startX;
        private decimal _startY;
        private decimal _historyX;
        private decimal _historyY;
        private IEnumerable<PointShape> _pointsCache;
        private IEnumerable<BaseShape> _shapesCache;

        public string Title => "Selection";

        public ToolSelection(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static bool IsPointMovable(BaseShape shape, PointShape point)
        {
            if (point.State.Flags.HasFlag(ShapeStateFlags.Locked) || (point.Owner is BaseShape ower && ower.State.Flags.HasFlag(ShapeStateFlags.Locked)))
            {
                return false;
            }

            if (point.State.Flags.HasFlag(ShapeStateFlags.Connector) && point.Owner != shape)
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<PointShape> GetMovePoints(IEnumerable<BaseShape> shapes)
        {
            var points = new List<PointShape>();

            foreach (var shape in shapes)
            {
                shape.GetPoints(points);
            }

            return points.Where(p => IsPointMovable(p.Owner as BaseShape, p)).Distinct();
        }

        private void GenerateMoveSelectionCache()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (editor.PageState.SelectedShapes != null)
            {
                var shapes = editor.PageState.SelectedShapes
                    .Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked)
                             && !s.State.Flags.HasFlag(ShapeStateFlags.Connector));

                switch (editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            _pointsCache = GetMovePoints(shapes).ToList();
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            _shapesCache = shapes.ToList();
                        }
                        break;
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            decimal dx = sx - _startX;
            decimal dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            if (_pointsCache != null)
            {
                editor.MoveShapesBy(_pointsCache, dx, dy);
            }

            if (_shapesCache != null)
            {
                editor.MoveShapesBy(_shapesCache, dx, dy);
            }
        }

        private bool IsSelectionAvailable()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            return editor?.PageState?.SelectedShapes != null;
        }

        private bool HitTestDecorator(InputArgs args, bool isControl, bool isHover)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();

            if (isControl == false && editor.PageState.Decorator != null && editor.PageState.Decorator.IsVisible)
            {
                bool decoratorResult = editor.PageState.Decorator.HitTest(args);
                if (decoratorResult == true && isHover == false)
                {
                    _currentState = State.Selected;
                    editor.IsToolIdle = false;
                    return true;
                }
            }

            return false;
        }

        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double x, double y) = args;
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.None:
                    {
                        if (editor.PageState == null)
                        {
                            return;
                        }

                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        editor.Dehover(editor.Project.CurrentContainer.CurrentLayer);

                        if (isControl == false && editor.PageState.DrawDecorators == true && editor.PageState.Decorator != null && editor.PageState.Decorator.IsVisible == true)
                        {
                            if (HitTestDecorator(args, isControl, false) == true)
                            {
                                return;
                            }
                        }

                        if (isControl == true)
                        {
                            var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
                            double radius = editor.Project.Options.HitThreshold / editor.PageState.ZoomX;
                            BaseShape result = editor.HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                            if (result == null)
                            {
                                result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                            }

                            if (result != null)
                            {
                                if (editor.PageState.SelectedShapes == null)
                                {
                                    editor.PageState.SelectedShapes = new HashSet<BaseShape>() { result };
                                    editor.Project.CurrentContainer.CurrentLayer.InvalidateLayer();
                                    editor.OnShowOrHideDecorator();
                                    HitTestDecorator(args, isControl, false);
                                    break;
                                }
                                else if (editor.PageState.SelectedShapes != null)
                                {
                                    if (editor.PageState.SelectedShapes.Contains(result))
                                    {
                                        var selected = new HashSet<BaseShape>(editor.PageState.SelectedShapes);
                                        selected.Remove(result);

                                        if (selected.Count == 0)
                                        {
                                            editor.PageState.SelectedShapes = null;
                                            editor.OnHideDecorator();
                                        }
                                        else
                                        {
                                            editor.PageState.SelectedShapes = selected;
                                            editor.OnShowOrHideDecorator();
                                            HitTestDecorator(args, isControl, false);
                                        }
                                        editor.Project.CurrentContainer.CurrentLayer.InvalidateLayer();
                                        break;
                                    }
                                    else
                                    {
                                        var selected = new HashSet<BaseShape>(editor.PageState.SelectedShapes);
                                        selected.Add(result);

                                        editor.PageState.SelectedShapes = selected;

                                        editor.Project.CurrentContainer.CurrentLayer.InvalidateLayer();
                                        editor.OnShowOrHideDecorator();
                                        HitTestDecorator(args, isControl, false);
                                        break;
                                    }
                                }
                            }
                        }

                        if (isControl == false && editor.PageState.DrawDecorators == true && editor.PageState.Decorator != null && editor.PageState.Decorator.IsVisible == true)
                        {
                            editor.OnHideDecorator();
                        }

                        if (editor.PageState.SelectedShapes != null)
                        {
                            var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse();

                            double radius = editor.Project.Options.HitThreshold / editor.PageState.ZoomX;
                            BaseShape result = editor.HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                            if (result == null)
                            {
                                result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageState.ZoomX);
                            }

                            if (result != null && editor.PageState.SelectedShapes.Contains(result))
                            {
                                _startX = sx;
                                _startY = sy;
                                _historyX = _startX;
                                _historyY = _startY;
                                GenerateMoveSelectionCache();
                                _currentState = State.Selected;
                                editor.OnShowOrHideDecorator();
                                HitTestDecorator(args, isControl, false);
                                editor.IsToolIdle = false;
                                break;
                            }
                        }

                        var deselect = !isControl;

                        if (editor.TryToSelectShape(editor.Project.CurrentContainer.CurrentLayer, x, y, deselect))
                        {
                            _startX = sx;
                            _startY = sy;
                            _historyX = _startX;
                            _historyY = _startY;
                            GenerateMoveSelectionCache();
                            _currentState = State.Selected;
                            editor.OnShowOrHideDecorator();
                            HitTestDecorator(args, isControl, false);
                            editor.IsToolIdle = false;
                            break;
                        }

                        _rectangleShape = factory.CreateRectangleShape(
                            x, y,
                            editor.PageState.SelectionStyle,
                            true, true);
                        _rectangleShape.State.Flags |= ShapeStateFlags.Thickness;
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangleShape);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        _currentState = State.Selected;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Selected:
                    {
                        if (_rectangleShape != null)
                        {
                            _rectangleShape.BottomRight.X = x;
                            _rectangleShape.BottomRight.Y = y;
                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangleShape);
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            _currentState = State.None;
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        public void LeftUp(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.Selected:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (_rectangleShape != null)
                        {
                            _rectangleShape.BottomRight.X = args.X;
                            _rectangleShape.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangleShape);
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        }

                        if (IsSelectionAvailable() && !isControl)
                        {
                            (decimal sx, decimal sy) = editor.TryToSnap(args);
                            if (_historyX != sx || _historyY != sy)
                            {
                                decimal dx = sx - _historyX;
                                decimal dy = sy - _historyY;

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
                                    (state) =>
                                    {
                                        if (state.Points != null)
                                        {
                                            editor.MoveShapesBy(state.Points, state.DeltaX, state.DeltaY);
                                        }

                                        if (state.Shapes != null)
                                        {
                                            editor.MoveShapesBy(state.Shapes, state.DeltaX, state.DeltaY);
                                        }
                                    });
                            }

                            DisposeMoveSelectionCache();
                            _currentState = State.None;
                            editor.IsToolIdle = true;
                            break;
                        }

                        var deselect = !isControl;
                        var includeSelected = isControl;

                        if (_rectangleShape != null)
                        {
                            _currentState = State.None;
                            editor.TryToSelectShapes(editor.Project.CurrentContainer.CurrentLayer, _rectangleShape, deselect, includeSelected);
                            editor.OnShowOrHideDecorator();
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        public void RightDown(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.None:
                    {
                        editor.Dehover(editor.Project.CurrentContainer.CurrentLayer);
                    }
                    break;
                case State.Selected:
                    {
                        DisposeMoveSelectionCache();
                        editor.OnHideDecorator();
                        editor.IsToolIdle = true;
                    }
                    break;
            }
        }

        public void RightUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.None:
                    {
                        if (editor.PageState == null)
                        {
                            return;
                        }

                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (!isControl)
                        {
                            editor.TryToHoverShape(args.X, args.Y);
                            HitTestDecorator(args, isControl, true);
                        }
                    }
                    break;
                case State.Selected:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (isControl == false && editor.PageState.DrawDecorators == true && editor.PageState.Decorator != null && editor.PageState.Decorator.IsVisible == true)
                        {
                            editor.PageState.Decorator.Move(args);
                            editor.PageState.Decorator.Update(false);
                            return;
                        }

                        HitTestDecorator(args, isControl, true);

                        if (IsSelectionAvailable() && !isControl)
                        {
                            MoveSelectionCacheTo(args);
                            editor.OnUpdateDecorator();
                            editor.Project.CurrentContainer.CurrentLayer.InvalidateLayer();
                            break;
                        }

                        if (_rectangleShape != null)
                        {
                            _rectangleShape.BottomRight.X = args.X;
                            _rectangleShape.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        }
                    }
                    break;
            }
        }

        public void Move(BaseShape shape)
        {
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();

            _currentState = State.None;

            editor.IsToolIdle = true;

            editor.Dehover(editor.Project?.CurrentContainer?.CurrentLayer);

            DisposeMoveSelectionCache();
            editor.OnHideDecorator();

            editor.Project?.CurrentContainer?.CurrentLayer?.InvalidateLayer();
            editor.Project?.CurrentContainer?.WorkingLayer?.InvalidateLayer();
        }
    }
}
