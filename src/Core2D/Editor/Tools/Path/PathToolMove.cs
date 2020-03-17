// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Path.Settings;
using Core2D.Interfaces;
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
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        var pathTool = _serviceProvider.GetService<ToolPath>();
                        editor.CurrentPathTool = pathTool.PreviousPathTool;

                        var start = editor.TryToGetConnectionPoint(sx, sy) ?? factory.CreatePointShape(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.BeginFigure(
                                start,
                                editor.Project.Options.DefaultIsFilled,
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
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
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
