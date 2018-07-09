// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Wpf.Dock.Views;
using Dock.Model;
using Dock.Model.Controls;

namespace Core2D.Wpf.Dock.Factories
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
                Width = double.NaN,
                Height = double.NaN,
                Title = "Editor"
            };

            var dashboardView = new DashboardView
            {
                Id = nameof(DashboardView),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Dashboard"
            };

            var layout = new RootDock
            {
                Id = nameof(IRootDock),
                Width = double.NaN,
                Height = double.NaN,
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
        public override void InitLayout(IView layout, object context)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                [nameof(IRootDock)] = () => context,
                [nameof(ILayoutDock)] = () => context,
                [nameof(IDocumentDock)] = () => context,
                [nameof(IToolDock)] = () => context,
                [nameof(ISplitterDock)] = () => context,
                [nameof(IDockWindow)] = () => context,
                [nameof(IDocumentTab)] = () => context,
                [nameof(IToolTab)] = () => context,
                [nameof(EditorView)] = () => context,
                [nameof(DashboardView)] = () => context
            };

            HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<IDockHost>()
            };

            base.InitLayout(layout, context);
        }
    }
}
