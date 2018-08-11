// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Settings;
using Core2D.Shapes.Interfaces;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Point tool.
    /// </summary>
    public class ToolPoint : ToolBase
    {
        public enum State { Point }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsPoint _settings;
        private State _currentState = State.Point;
        private IPointShape _point;

        /// <inheritdoc/>
        public override string Title => "Point";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsPoint Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
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
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point:
                    {
                        _point = PointShape.Create(sx, sy, editor.Project.Options.PointShape);

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
        public override void Move(InputArgs args)
        {
            base.Move(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Point:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
            }
        }
    }
}
