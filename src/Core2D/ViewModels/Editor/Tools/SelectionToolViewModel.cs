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
    public class SelectionToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { None, Selected }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.None;
        private RectangleShapeViewModel _rectangleShapeViewModel;
        private decimal _startX;
        private decimal _startY;
        private decimal _historyX;
        private decimal _historyY;
        private IEnumerable<PointShapeViewModel> _pointsCache;
        private IEnumerable<BaseShapeViewModel> _shapesCache;

        public string Title => "Selection";

        public SelectionToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static bool IsPointMovable(BaseShapeViewModel shapeViewModel, PointShapeViewModel point)
        {
            if (point.State.HasFlag(ShapeStateFlags.Locked) || (point.Owner is BaseShapeViewModel ower && ower.State.HasFlag(ShapeStateFlags.Locked)))
            {
                return false;
            }

            if (point.State.HasFlag(ShapeStateFlags.Connector) && point.Owner != shapeViewModel)
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

            return points.Where(p => IsPointMovable(p.Owner as BaseShapeViewModel, p)).Distinct();
        }

        private void GenerateMoveSelectionCache()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel.SelectedShapes != null)
            {
                var shapes = editor.PageStateViewModel.SelectedShapes
                    .Where(s => !s.State.HasFlag(ShapeStateFlags.Locked)
                             && !s.State.HasFlag(ShapeStateFlags.Connector));

                switch (editor.Project.OptionsViewModel.MoveMode)
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
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
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
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            return editor?.PageStateViewModel?.SelectedShapes != null;
        }

        private bool HitTestDecorator(InputArgs args, bool isControl, bool isHover)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (isControl == false && editor.PageStateViewModel.Decorator != null && editor.PageStateViewModel.Decorator.IsVisible)
            {
                bool decoratorResult = editor.PageStateViewModel.Decorator.HitTest(args);
                if (decoratorResult == true && isHover == false)
                {
                    editor.IsToolIdle = false;
                    _currentState = State.Selected;
                    return true;
                }
            }

            return false;
        }

        public void BeginDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            (double x, double y) = args;
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.None:
                    {
                        if (editor.PageStateViewModel == null)
                        {
                            return;
                        }

                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        editor.Dehover(editor.Project.CurrentContainerViewModel.CurrentLayer);

                        if (isControl == false && editor.PageStateViewModel.DrawDecorators == true && editor.PageStateViewModel.Decorator != null && editor.PageStateViewModel.Decorator.IsVisible == true)
                        {
                            if (HitTestDecorator(args, isControl, false) == true)
                            {
                                return;
                            }
                        }

                        if (isControl == true)
                        {
                            var shapes = editor.Project.CurrentContainerViewModel.CurrentLayer.Shapes.Reverse();
                            double radius = editor.Project.OptionsViewModel.HitThreshold / editor.PageStateViewModel.ZoomX;
                            BaseShapeViewModel result = editor.HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageStateViewModel.ZoomX);
                            if (result == null)
                            {
                                result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageStateViewModel.ZoomX);
                            }

                            if (result != null)
                            {
                                if (editor.PageStateViewModel.SelectedShapes == null)
                                {
                                    editor.PageStateViewModel.SelectedShapes = new HashSet<BaseShapeViewModel>() { result };
                                    editor.Project.CurrentContainerViewModel.CurrentLayer.InvalidateLayer();
                                    editor.OnShowOrHideDecorator();
                                    HitTestDecorator(args, isControl, false);
                                    break;
                                }
                                else if (editor.PageStateViewModel.SelectedShapes != null)
                                {
                                    if (editor.PageStateViewModel.SelectedShapes.Contains(result))
                                    {
                                        var selected = new HashSet<BaseShapeViewModel>(editor.PageStateViewModel.SelectedShapes);
                                        selected.Remove(result);

                                        if (selected.Count == 0)
                                        {
                                            editor.PageStateViewModel.SelectedShapes = null;
                                            editor.OnHideDecorator();
                                        }
                                        else
                                        {
                                            editor.PageStateViewModel.SelectedShapes = selected;
                                            editor.OnShowOrHideDecorator();
                                            HitTestDecorator(args, isControl, false);
                                        }
                                        editor.Project.CurrentContainerViewModel.CurrentLayer.InvalidateLayer();
                                        break;
                                    }
                                    else
                                    {
                                        var selected = new HashSet<BaseShapeViewModel>(editor.PageStateViewModel.SelectedShapes);
                                        selected.Add(result);

                                        editor.PageStateViewModel.SelectedShapes = selected;

                                        editor.Project.CurrentContainerViewModel.CurrentLayer.InvalidateLayer();
                                        editor.OnShowOrHideDecorator();
                                        HitTestDecorator(args, isControl, false);
                                        break;
                                    }
                                }
                            }
                        }

                        if (isControl == false && editor.PageStateViewModel.DrawDecorators == true && editor.PageStateViewModel.Decorator != null && editor.PageStateViewModel.Decorator.IsVisible == true)
                        {
                            editor.OnHideDecorator();
                        }

                        if (editor.PageStateViewModel.SelectedShapes != null)
                        {
                            var shapes = editor.Project.CurrentContainerViewModel.CurrentLayer.Shapes.Reverse();

                            double radius = editor.Project.OptionsViewModel.HitThreshold / editor.PageStateViewModel.ZoomX;
                            BaseShapeViewModel result = editor.HitTest.TryToGetPoint(shapes, new Point2(x, y), radius, editor.PageStateViewModel.ZoomX);
                            if (result == null)
                            {
                                result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), radius, editor.PageStateViewModel.ZoomX);
                            }

                            if (result != null && editor.PageStateViewModel.SelectedShapes.Contains(result))
                            {
                                editor.IsToolIdle = false;
                                _startX = sx;
                                _startY = sy;
                                _historyX = _startX;
                                _historyY = _startY;
                                GenerateMoveSelectionCache();
                                _currentState = State.Selected;
                                editor.OnShowOrHideDecorator();
                                HitTestDecorator(args, isControl, false);
                                break;
                            }
                        }

                        var deselect = !isControl;

                        if (editor.TryToSelectShape(editor.Project.CurrentContainerViewModel.CurrentLayer, x, y, deselect))
                        {
                            editor.IsToolIdle = false;
                            _startX = sx;
                            _startY = sy;
                            _historyX = _startX;
                            _historyY = _startY;
                            GenerateMoveSelectionCache();
                            _currentState = State.Selected;
                            editor.OnShowOrHideDecorator();
                            HitTestDecorator(args, isControl, false);
                            break;
                        }

                        editor.IsToolIdle = false;
                        _rectangleShapeViewModel = factory.CreateRectangleShape(
                            x, y,
                            editor.PageStateViewModel.SelectionStyleViewModel,
                            true, true);
                        _rectangleShapeViewModel.State |= ShapeStateFlags.Thickness;
                        editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Add(_rectangleShapeViewModel);
                        editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        _currentState = State.Selected;
                    }
                    break;
                case State.Selected:
                    {
                        if (_rectangleShapeViewModel != null)
                        {
                            _rectangleShapeViewModel.BottomRight.X = x;
                            _rectangleShapeViewModel.BottomRight.Y = y;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_rectangleShapeViewModel);
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                            _currentState = State.None;
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        public void BeginUp(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.Selected:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (_rectangleShapeViewModel != null)
                        {
                            _rectangleShapeViewModel.BottomRight.X = args.X;
                            _rectangleShapeViewModel.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes = editor.Project.CurrentContainerViewModel.WorkingLayer.Shapes.Remove(_rectangleShapeViewModel);
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
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

                        if (_rectangleShapeViewModel != null)
                        {
                            _currentState = State.None;
                            editor.TryToSelectShapes(editor.Project.CurrentContainerViewModel.CurrentLayer, _rectangleShapeViewModel, deselect, includeSelected);
                            editor.OnShowOrHideDecorator();
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        public void EndDown(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            switch (_currentState)
            {
                case State.None:
                    {
                        editor.Dehover(editor.Project.CurrentContainerViewModel.CurrentLayer);
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

        public void EndUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            switch (_currentState)
            {
                case State.None:
                    {
                        if (editor.PageStateViewModel == null)
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

                        if (isControl == false && editor.PageStateViewModel.DrawDecorators == true && editor.PageStateViewModel.Decorator != null && editor.PageStateViewModel.Decorator.IsVisible == true)
                        {
                            editor.PageStateViewModel.Decorator.Move(args);
                            editor.PageStateViewModel.Decorator.Update(false);
                            return;
                        }

                        HitTestDecorator(args, isControl, true);

                        if (IsSelectionAvailable() && !isControl)
                        {
                            MoveSelectionCacheTo(args);
                            editor.OnUpdateDecorator();
                            editor.Project.CurrentContainerViewModel.CurrentLayer.InvalidateLayer();
                            break;
                        }

                        if (_rectangleShapeViewModel != null)
                        {
                            _rectangleShapeViewModel.BottomRight.X = args.X;
                            _rectangleShapeViewModel.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainerViewModel.WorkingLayer.InvalidateLayer();
                        }
                    }
                    break;
            }
        }

        public void Move(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            _currentState = State.None;

            editor.Dehover(editor.Project?.CurrentContainerViewModel?.CurrentLayer);

            DisposeMoveSelectionCache();
            editor.OnHideDecorator();

            editor.Project?.CurrentContainerViewModel?.CurrentLayer?.InvalidateLayer();
            editor.Project?.CurrentContainerViewModel?.WorkingLayer?.InvalidateLayer();

            editor.IsToolIdle = true;
        }
    }
}
