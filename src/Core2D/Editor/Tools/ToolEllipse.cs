using System;
using System.Collections.Generic;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;
using static System.Math;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Ellipse tool.
    /// </summary>
    public class ToolEllipse : ObservableObject, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        public enum Mode { Rectangle, Circle }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsEllipse _settings;
        private State _currentState = State.TopLeft;
        private Mode _currentMode = Mode.Rectangle;
        private IEllipseShape _ellipse;
        private ToolEllipseSelection _selection;
        private double _centerX;
        private double _centerY;

        /// <inheritdoc/>
        public string Title => "Ellipse";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsEllipse Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolEllipse"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolEllipse(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsEllipse();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static void CircleConstrain(IPointShape tl, IPointShape br, double cx, double cy, double px, double py)
        {
            double r = Max(Abs(cx - px), Abs(cy - py));
            tl.X = cx - r;
            tl.Y = cy - r;
            br.X = cx + r;
            br.Y = cy + r;
        }

        /// <inheritdoc/>
        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        if (_currentMode == Mode.Circle)
                        {
                            _centerX = sx;
                            _centerY = sy;
                        }

                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _ellipse = factory.CreateEllipseShape(
                            sx, sy,
                            (IShapeStyle)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _ellipse.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_ellipse);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateBottomRight();
                        Move(_ellipse);
                        _currentState = State.BottomRight;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_ellipse != null)
                        {
                            if (_currentMode == Mode.Circle)
                            {
                                CircleConstrain(_ellipse.TopLeft, _ellipse.BottomRight, _centerX, _centerY, sx, sy);
                            }
                            else
                            {
                                _ellipse.BottomRight.X = sx;
                                _ellipse.BottomRight.Y = sy;
                            }

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _ellipse.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
                            Finalize(_ellipse);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _ellipse);

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
                case State.TopLeft:
                    break;
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_ellipse != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }

                            if (_currentMode == Mode.Circle)
                            {
                                CircleConstrain(_ellipse.TopLeft, _ellipse.BottomRight, _centerX, _centerY, sx, sy);
                            }
                            else
                            {
                                _ellipse.BottomRight.X = sx;
                                _ellipse.BottomRight.Y = sy;
                            }
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_ellipse);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.BottomRight"/>.
        /// </summary>
        public void ToStateBottomRight()
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            _selection = new ToolEllipseSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _ellipse,
                editor.PageState.HelperStyle);

            _selection.ToStateBottomRight();
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
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                    }
                    break;
            }

            _currentState = State.TopLeft;
            editor.IsToolIdle = true;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
        }
    }
}
