// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Spatial.Arc;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Arc tool.
    /// </summary>
    public class ToolArc : ToolBase
    {
        public enum State { Point1, Point2, Point3, Point4 }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsArc _settings;
        private State _currentState = State.Point1;
        private XArc _arc;
        private bool _connectedPoint3;
        private bool _connectedPoint4;
        private ToolArcSelection _selection;

        /// <inheritdoc/>
        public override string Name => "Arc";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsArc Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolArc"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolArc(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsArc();
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
                        _connectedPoint3 = false;
                        _connectedPoint4 = false;
                        _arc = XArc.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _arc.Point1 = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStatePoint2();
                        Move(_arc);
                        _currentState = State.Point2;
                        editor.CancelAvailable = true;
                    }
                    break;
                case State.Point2:
                    {
                        if (_arc != null)
                        {
                            _arc.Point2.X = sx;
                            _arc.Point2.Y = sy;
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point2 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStatePoint3();
                            Move(_arc);
                            _currentState = State.Point3;
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_arc != null)
                        {
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point3 = result;
                                _connectedPoint3 = true;
                            }
                            else
                            {
                                _connectedPoint3 = false;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_arc);
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStatePoint4();
                            Move(_arc);
                            _currentState = State.Point4;
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (_arc != null)
                        {
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _arc.Point4 = result;
                                _connectedPoint4 = true;
                            }
                            else
                            {
                                _connectedPoint4 = false;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                            Remove();
                            Finalize(_arc);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _arc);
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
                case State.Point2:
                case State.Point3:
                case State.Point4:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
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
                case State.Point2:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point2.X = sx;
                            _arc.Point2.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point3.X = sx;
                            _arc.Point3.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _arc.Point4.X = sx;
                            _arc.Point4.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_arc);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point2"/>.
        /// </summary>
        public void ToStatePoint2()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolArcSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _arc,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStatePoint2();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point3"/>.
        /// </summary>
        public void ToStatePoint3()
        {
            _selection.ToStatePoint3();
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.Point4"/>.
        /// </summary>
        public void ToStatePoint4()
        {
            _selection.ToStatePoint4();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);

            var arc = shape as XArc;
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (!_connectedPoint3)
            {
                arc.Point3.X = a.Start.X;
                arc.Point3.Y = a.Start.Y;
            }

            if (!_connectedPoint4)
            {
                arc.Point4.X = a.End.X;
                arc.Point4.Y = a.End.Y;
            }
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
