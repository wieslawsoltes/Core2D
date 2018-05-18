// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dock.Model;

namespace Dock.Avalonia.Factories
{
    /// <summary>
    /// Dock factory base.
    /// </summary>
    public abstract class BaseDockFactory : IDockFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="BaseDockFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public BaseDockFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public virtual void UpdateWindow(IDockWindow window, object context)
        {
            var host = (IDockHost)_serviceProvider.GetService(typeof(IDockHost));

            window.Host = host;
            window.Context = context;

            UpdateView(window.Layout, context);
        }

        /// <inheritdoc/>
        public virtual void UpdateWindows(IList<IDockWindow> windows, object context)
        {
            foreach (var window in windows)
            {
                UpdateWindow(window, context);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateView(IDock view, object context)
        {
            view.CurrentView.Context = context;
            view.Context = context;
            view.Factory = this;

            if (view.Windows != null)
            {
                UpdateWindows(view.Windows, context);
            }

            if (view.Views != null)
            {
                UpdateViews(view.Views, context);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateViews(IList<IDock> views, object context)
        {
            if (view.Views != null)
            {
                UpdateView(view.Views, context);
            }
        }

        /// <inheritdoc/>
        public virtual IDockWindow CreateDockWindow(IDock layout, object context, IDock source, int viewIndex, double x, double y)
        {
            var view = source.Views[viewIndex];

            source.RemoveView(source, viewIndex);

            var dockLayout = new DockLayout
            {
                Dock = "",
                Title = "",
                Context = context,
                Factory = this,
                CurrentView = view,
                Views = new ObservableCollection<IDock> { view }
            };

            var host = (IDockHost)_serviceProvider.GetService(typeof(IDockHost));

            var dockWindow = new DockWindow()
            {
                X = x,
                Y = y,
                Width = 300,
                Height = 400,
                Title = "Dock",
                Context = context,
                Layout = dockLayout,
                Host = host
            };

            if (layout.CurrentView.Windows == null)
            {
                layout.CurrentView.Windows = new ObservableCollection<IDockWindow>();
            }
            layout.CurrentView.AddWindow(dockWindow);

            return dockWindow;
        }

        /// <inheritdoc/>
        public abstract IDock CreateDefaultLayout(object context);

        /// <inheritdoc/>
        public abstract void CreateOrUpdateLayout();
    }
}
