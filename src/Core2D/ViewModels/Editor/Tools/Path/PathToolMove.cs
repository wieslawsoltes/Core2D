using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Move path tool.
    /// </summary>
    public class PathToolMove : ObservableObject, IPathTool
    {
        public enum State { Move }
        private readonly IServiceProvider _serviceProvider;
        private PathToolSettingsMove _settings;
        private readonly State _currentState = State.Move;

        /// <inheritdoc/>
        public string Title => "Move";

        /// <summary>
        /// Gets or sets the path tool settings.
        /// </summary>
        public PathToolSettingsMove Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathToolMove"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolMove(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new PathToolSettingsMove();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        var pathTool = _serviceProvider.GetService<ToolPath>();
                        editor.CurrentPathTool = pathTool.PreviousPathTool;

                        var start = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.BeginFigure(
                                start,
                                editor.Project.Options.DefaultIsClosed);

                        editor.CurrentPathTool.LeftDown(args);
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
        }

        /// <inheritdoc/>
        public void RightUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void Move(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Finalize(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
        }
    }
}
