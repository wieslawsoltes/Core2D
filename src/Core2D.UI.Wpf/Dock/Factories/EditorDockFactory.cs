// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Editor;
using Core2D.UI.Wpf.Dock.Views;
using Dock.Model;
using Dock.Model.Controls;

namespace Core2D.UI.Wpf.Dock.Factories
{
    /// <summary>
    /// Editor dock factory.
    /// </summary>
    public class EditorDockFactory : DockFactory
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
        public override IDock CreateLayout()
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

            var layout = new RootDock
            {
                Id = nameof(IRootDock),
                Title = "Root",
                CurrentView = dashboardView,
                DefaultView = dashboardView,
                Views = new ObservableCollection<IView>
                {
                    dashboardView,
                    editorView
                }
            };

            return layout;
        }

        /// <inheritdoc/>
        public override void InitLayout(IView layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ILayoutDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(IDocumentDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(IToolDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ISplitterDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(IDocumentTab)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(IToolTab)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(EditorView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DashboardView)] = () => _serviceProvider.GetService<IProjectEditor>()
            };

            HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<IDockHost>()
            };

            base.InitLayout(layout);
        }
    }
}
