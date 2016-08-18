// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.CubicBezier"/> editor.
    /// </summary>
    public class ToolCubicBezier : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolState _currentState = ToolState.None;
        private XCubicBezier _cubicBezier;
        private CubicBezierSelection _selection;

        /// <inheritdoc/>
        public override string Name => "CubicBezier";

        /// <summary>
        /// Initialize new instance of <see cref="ToolCubicBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolCubicBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
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
                        ToStateOne();
                        Move(_cubicBezier);
                        _currentState = ToolState.One;
                        editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
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
                            ToStateTwo();
                            Move(_cubicBezier);
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
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
                            ToStateThree();
                            Move(_cubicBezier);
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
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
                            _currentState = ToolState.None;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = ToolState.None;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
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
                case ToolState.Two:
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
                case ToolState.Three:
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

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new CubicBezierSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _selection.ToStateTwo();
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _selection.ToStateThree();
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
