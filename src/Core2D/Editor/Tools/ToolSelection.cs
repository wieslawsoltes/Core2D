using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Containers;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Settings;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Selection tool.
    /// </summary>
    public class ToolSelection : ObservableObject, IEditorTool
    {
        public enum State { None, Selected }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsSelection _settings;
        private State _currentState = State.None;
        private IRectangleShape _rectangle;
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;
        private IEnumerable<IPointShape> _pointsCache;
        private IEnumerable<IBaseShape> _shapesCache;

        /// <inheritdoc/>
        public string Title => "Selection";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsSelection Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolSelection(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsSelection();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate if point can move.
        /// </summary>
        /// <remarks>Do not move points if they are (1) connector and have owner or (2) are locked.</remarks>
        /// <param name="shape">The shape object.</param>
        /// <param name="point">The point to validate.</param>
        /// <returns>True if point is valid, otherwise false.</returns>
        private static bool Validate(IBaseShape shape, IPointShape point)
        {
            if (point.State.Flags.HasFlag(ShapeStateFlags.Locked))
            {
                return false;
            }

            if (point.State.Flags.HasFlag(ShapeStateFlags.Connector) && point.Owner != shape)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get all valid points in the shapes.
        /// </summary>
        /// <param name="shapes">The shapes to scan.</param>
        /// <returns>All points in the shape.</returns>
        private static IEnumerable<IPointShape> GetMovePoints(IEnumerable<IBaseShape> shapes)
        {
            return shapes.SelectMany(s => s.GetPoints().Where(p => Validate(s, p))).Distinct();
        }

        /// <summary>
        /// Generate selected shapes cache.
        /// </summary>
        private void GenerateMoveSelectionCache()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            if (editor.PageState.SelectedShape != null)
            {
                var state = editor.PageState.SelectedShape.State;

                switch (editor.Project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked)
                                && !state.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var shape = editor.PageState.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1);
                                _pointsCache = GetMovePoints(shapes).ToList();
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked)
                                && !state.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var shape = editor.PageState.SelectedShape;
                                var shapes = Enumerable.Repeat(shape, 1).ToList();
                                _shapesCache = shapes;
                            }
                        }
                        break;
                }
            }

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

        /// <summary>
        /// Dispose selected shapes cache.
        /// </summary>
        private void DisposeMoveSelectionCache()
        {
            _pointsCache = null;
            _shapesCache = null;
        }

        /// <summary>
        /// Move selected shapes to new location.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        private void MoveSelectionCacheTo(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            double dx = sx - _startX;
            double dy = sy - _startY;

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
            var editor = _serviceProvider.GetService<IProjectEditor>();
            return editor?.PageState?.SelectedShape != null
                || editor?.PageState?.SelectedShapes != null;
        }

        /// <inheritdoc/>
        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double x, double y) = args;
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.None:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        editor.Dehover(editor.Project.CurrentContainer.CurrentLayer);

                        if (isControl == true)
                        {
                            var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
                            var result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), editor.Project.Options.HitThreshold);
                            if (result != null)
                            {
                                if (editor.PageState.SelectedShape == null && editor.PageState.SelectedShapes == null)
                                {
                                    editor.PageState.SelectedShape = result;
                                    editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                                    break;
                                }
                                else if (editor.PageState.SelectedShape != null && editor.PageState.SelectedShapes == null)
                                {
                                    if (editor.PageState.SelectedShape == result)
                                    {
                                        editor.PageState.SelectedShape = null;
                                        editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                                        break;
                                    }
                                    else
                                    {
                                        var selected = editor.PageState.SelectedShape;
                                        editor.PageState.SelectedShape = null;
                                        editor.PageState.SelectedShapes = new HashSet<IBaseShape>() { selected, result };
                                        editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                                        break;
                                    }
                                }
                                else if (editor.PageState.SelectedShape == null && editor.PageState.SelectedShapes != null)
                                {
                                    if (editor.PageState.SelectedShapes.Contains(result))
                                    {
                                        editor.PageState.SelectedShapes.Remove(result);
                                        if (editor.PageState.SelectedShapes.Count == 0)
                                        {
                                            editor.PageState.SelectedShape = null;
                                            editor.PageState.SelectedShapes = null;
                                        }
                                        else if (editor.PageState.SelectedShapes.Count == 1)
                                        {
                                            var selected = editor.PageState.SelectedShapes.FirstOrDefault();
                                            editor.PageState.SelectedShape = selected;
                                            editor.PageState.SelectedShapes = null;
                                        }
                                        editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                                        break;
                                    }
                                    else
                                    {
                                        editor.PageState.SelectedShapes.Add(result);
                                        editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                                        break;
                                    }
                                }
                            }
                        }

                        if (editor.PageState.SelectedShape == null && editor.PageState.SelectedShapes != null)
                        {
                            var shapes = editor.Project.CurrentContainer.CurrentLayer.Shapes.Reverse();
                            var result = editor.HitTest.TryToGetShape(shapes, new Point2(x, y), editor.Project.Options.HitThreshold);
                            if (result != null && editor.PageState.SelectedShapes.Contains(result))
                            {
                                _startX = sx;
                                _startY = sy;
                                _historyX = _startX;
                                _historyY = _startY;
                                GenerateMoveSelectionCache();
                                _currentState = State.Selected;
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
                            editor.IsToolIdle = false;
                            break;
                        }

                        _rectangle = factory.CreateRectangleShape(
                            x, y,
                            editor.PageState.SelectionStyle,
                            true, true);
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _currentState = State.Selected;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Selected:
                    {
                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _currentState = State.None;
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void LeftUp(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.Selected:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = args.X;
                            _rectangle.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }

                        if (IsSelectionAvailable() && !isControl)
                        {
                            (double sx, double sy) = editor.TryToSnap(args);
                            if (_historyX != sx || _historyY != sy)
                            {
                                double dx = sx - _historyX;
                                double dy = sy - _historyY;

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

                        if (_rectangle != null)
                        {
                            _currentState = State.None;
                            editor.TryToSelectShapes(editor.Project.CurrentContainer.CurrentLayer, _rectangle, deselect, includeSelected);
                            editor.IsToolIdle = true;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void RightDown(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
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
                        editor.IsToolIdle = true;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void RightUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            switch (_currentState)
            {
                case State.None:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (!isControl)
                        {
                            editor.TryToHoverShape(args.X, args.Y);
                        }
                    }
                    break;
                case State.Selected:
                    {
                        bool isControl = args.Modifier.HasFlag(ModifierFlags.Control);

                        if (IsSelectionAvailable() && !isControl)
                        {
                            MoveSelectionCacheTo(args);
                            editor.Project.CurrentContainer.CurrentLayer.Invalidate();
                            break;
                        }

                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = args.X;
                            _rectangle.BottomRight.Y = args.Y;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void Move(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Finalize(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
        }
    }
}
