using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Point tool.
    /// </summary>
    public class ToolPoint : ObservableObject, IEditorTool
    {
        public enum State { Point }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsPoint _settings;
        private State _currentState = State.Point;
        private PointShape _point;

        /// <inheritdoc/>
        public string Title => "Point";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsPoint Settings
        {
            get => _settings;
            set => RaiseAndSetIfChanged(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolPoint"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolPoint(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsPoint();
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
                case State.Point:
                    {
                        _point = factory.CreatePointShape((double)sx, (double)sy);

                        if (editor.Project.Options.TryToConnect)
                        {
                            if (!editor.TryToSplitLine(args.X, args.Y, _point, true))
                            {
                                editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
                            }
                        }
                        else
                        {
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
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
                case State.Point:
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
