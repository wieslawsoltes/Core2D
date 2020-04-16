using System;
using System.Collections.Generic;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Cubic bezier tool.
    /// </summary>
    public class ToolCubicBezier : ObservableObject, IEditorTool
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsCubicBezier _settings;
        private State _currentState = State.Point1;
        private ICubicBezierShape _cubicBezier;
        private ToolCubicBezierSelection _selection;

        /// <inheritdoc/>
        public string Title => "CubicBezier";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsCubicBezier Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolCubicBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolCubicBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsCubicBezier();
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
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _cubicBezier = factory.CreateCubicBezierShape(
                            sx, sy,
                            (IShapeStyle)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _cubicBezier.Point1 = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_cubicBezier);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStatePoint4();
                        Move(_cubicBezier);
                        _currentState = State.Point4;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.Point4:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            _cubicBezier.Point4.X = sx;
                            _cubicBezier.Point4.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point4 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStatePoint2();
                            Move(_cubicBezier);
                            _currentState = State.Point2;
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point2 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStatePoint3();
                            Move(_cubicBezier);
                            _currentState = State.Point3;
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point3 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                            Finalize(_cubicBezier);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _cubicBezier);

                            Reset();
                        }
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
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
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
                case State.Point1:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            _cubicBezier.Point4.X = sx;
                            _cubicBezier.Point4.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_cubicBezier != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point4"/>.
        /// </summary>
        public void ToStatePoint4()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            _selection = new ToolCubicBezierSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.PageState.HelperStyle);

            _selection.ToStatePoint4();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point2"/>.
        /// </summary>
        public void ToStatePoint2()
        {
            _selection.ToStatePoint2();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point3"/>.
        /// </summary>
        public void ToStatePoint3()
        {
            _selection.ToStatePoint3();
        }

        /// <inheritdoc/>
        public void Move(IBaseShape shape)
        {
            _selection.Move();
        }

        /// <inheritdoc/>
        public void Finalize(IBaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point4:
                case State.Point2:
                case State.Point3:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                    }
                    break;
            }

            _currentState = State.Point1;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
