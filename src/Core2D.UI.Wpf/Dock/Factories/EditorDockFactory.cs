// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor;
using Core2D.UI.Wpf.Dock.Views;
using DM = Dock.Model;
using DMC = Dock.Model.Controls;

namespace Core2D.UI.Wpf.Dock.Factories
{
    /// <summary>
    /// Editor dock factory.
    /// </summary>
    public class EditorDockFactory : DM.Factory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="EditorDockFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public EditorDockFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override DM.IDock CreateLayout()
        {
            var editorView = new EditorView
            {
                Id = nameof(EditorView),
                Title = "Editor"
            };

            var dashboardView = new DashboardView
            {
                Id = nameof(DashboardView),
                Title = "Dashboard"
            };

            var layout = new DMC.RootDock
            {
                Id = nameof(DMC.IRootDock),
                Title = "Root",
                ActiveDockable = dashboardView,
                DefaultDockable = dashboardView,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    dashboardView,
                    editorView
                )
            };

            return layout;
        }

        /// <inheritdoc/>
        public override void InitLayout(DM.IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(DMC.IRootDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IPinDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IProportionalDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IDocumentDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IToolDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.ISplitterDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DM.IDockWindow)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IDocument)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.ITool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(EditorView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DashboardView)] = () => _serviceProvider.GetService<IProjectEditor>()
            };

            this.HostWindowLocator = new Dictionary<string, Func<DM.IHostWindow>>
            {
                [nameof(DM.IDockWindow)] = () => _serviceProvider.GetService<DM.IHostWindow>()
            };

            base.InitLayout(layout);
        }
    }
}
