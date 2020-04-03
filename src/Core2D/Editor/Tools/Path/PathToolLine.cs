using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Editor.Tools.Selection;
using Core2D.Interfaces;
using Core2D.Path.Segments;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Line path tool.
    /// </summary>
    public class PathToolLine : ObservableObject, IPathTool
    {
        public enum State { Start, End }
        private readonly IServiceProvider _serviceProvider;
        private PathToolSettingsLine _settings;
        private State _currentState = State.Start;
        private LineShape _line = new LineShape();
        private ToolLineSelection _selection;

        /// <inheritdoc/>
        public string Title => "Line";

        /// <summary>
        /// Gets or sets the path tool settings.
        /// </summary>
        public PathToolSettingsLine Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathToolLine"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolLine(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new PathToolSettingsLine();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        _line.Start = editor.TryToGetConnectionPoint(sx, sy) ?? factory.CreatePointShape(sx, sy);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_line.Start);
                        }
                        else
                        {
                            _line.Start = pathTool.GetLastPathPoint();
                        }

                        _line.End = factory.CreatePointShape(sx, sy);
                        pathTool.GeometryContext.LineTo(
                            _line.End,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateEnd();
                        Move(null);
                        _currentState = State.End;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.End:
                    {
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var end = editor.TryToGetConnectionPoint(sx, sy);
                            if (end != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as LineSegment;
                                line.Point = end;
                            }
                        }

                        _line.Start = _line.End;
                        _line.End = factory.CreatePointShape(sx, sy);
                        pathTool.GeometryContext.LineTo(_line.End,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _currentState = State.End;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void LeftUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void RightDown(InputArgs args)
        {
            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    Reset();
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
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Start:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.End:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _line.End.X = sx;
                        _line.End.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.End"/>.
        /// </summary>
        public void ToStateEnd()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            _selection?.Reset();
            _selection = new ToolLineSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.Project.Options.HelperStyle);
            _selection.ToStateEnd();
        }

        /// <inheritdoc/>
        public void Move(IBaseShape shape)
        {
            if (_selection != null)
            {
                _selection.Move();
            }
        }

        /// <inheritdoc/>
        public void Finalize(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();

            switch (_currentState)
            {
                case State.Start:
                    break;
                case State.End:
                    {
                        pathTool.RemoveLastSegment<LineSegment>();
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(pathTool.Path);
                        if (pathTool.Path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, pathTool.Path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                    }
                    break;
            }

            _currentState = State.Start;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
