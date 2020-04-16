using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Rectangle tool.
    /// </summary>
    public class ToolRectangle : ObservableObject, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsRectangle _settings;
        private State _currentState = State.TopLeft;
        private IRectangleShape _rectangle;
        private ToolRectangleSelection _selection;

        /// <inheritdoc/>
        public string Title => "Rectangle";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsRectangle Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolRectangle"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolRectangle(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsRectangle();
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
                case State.TopLeft:
                    {
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        _rectangle = factory.CreateRectangleShape(
                            sx, sy,
                            (IShapeStyle)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _rectangle.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateBottomRight();
                        Move(_rectangle);
                        _currentState = State.BottomRight;
                        editor.IsToolIdle = false;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = sx;
                            _rectangle.BottomRight.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _rectangle.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            Finalize(_rectangle);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _rectangle);

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
                        if (_rectangle != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _rectangle.BottomRight.X = sx;
                            _rectangle.BottomRight.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_rectangle);
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
            _selection = new ToolRectangleSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _rectangle,
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
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
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
