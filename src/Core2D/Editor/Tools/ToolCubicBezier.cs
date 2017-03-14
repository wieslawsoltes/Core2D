// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Cubic bezier tool.
    /// </summary>
    public class ToolCubicBezier : ToolBase
    {
        public enum State { Point1, Point4, Point2, Point3 }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsCubicBezier _settings;
        private State _currentState = State.Point1;
        private XCubicBezier _cubicBezier;
        private ToolCubicBezierSelection _selection;

        /// <inheritdoc/>
        public override string Name => "CubicBezier";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsCubicBezier Settings
        {
            get { return _settings; }
            set { Update(ref _settings, value); }
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
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point1:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _cubicBezier = XCubicBezier.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
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
                        editor.CancelAvailable = true;
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
                            Remove();
                            base.Finalize(_cubicBezier);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _cubicBezier);
                            _currentState = State.Point1;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
                        Remove();
                        _currentState = State.Point1;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(InputArgs args)
        {
            base.Move(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolCubicBezierSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

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
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _selection.Remove();
            _selection = null;
        }
    }
}
