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
        public virtual void UpdateWindows(IList<IDockWindow> windows, IList<IDockView> views, object context)
        {
            foreach (var window in windows)
            {
                var host = (IDockHost)_serviceProvider.GetService(typeof(IDockHost));

                window.Host = host;
                window.Context = context;

                UpdateLayout(window.Layout, views, context);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateViews(IList<IDock> target, IList<IDockView> views, object context)
        {
            for (int i = 0; i < target.Count; i++)
            {
                var original = target[i];
                target[i] = views.FirstOrDefault(v => v.Title == original.Title);
                target[i].Windows = original.Windows;

                if (original.Windows != null)
                {
                    UpdateWindows(original.Windows, views, context);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateLayout(IDockLayout layout, IList<IDockView> views, object context)
        {
            UpdateViews(layout.Children, views, context);

            layout.CurrentView = views.FirstOrDefault(v => v.Title == layout.CurrentView?.Title);
            layout.Factory = this;

            if (layout.Children != null)
            {
                foreach (var child in layout.Children)
                {
                    if (child is IDockLayout childLayout)
                    {
                        UpdateLayout(childLayout, views, context);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual IDockWindow CreateDockWindow(IDockLayout layout, object context, IDockLayout container, int viewIndex, double x, double y)
        {
            var view = container.Children[viewIndex];

            layout.RemoveView(container, viewIndex);

            var dockLayout = new DockLayout
            {
                Dock = "",
                CurrentView = view,
                Children = new ObservableCollection<IDock>
                {
                    new DockLayout
                    {
                        Dock = "",
                        Children = new ObservableCollection<IDock> { view },
                        CurrentView = view,
                        Factory = this
                    }
                },
                Factory = this
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
        public abstract IDockLayout CreateDefaultLayout(IList<IDockView> views);

        /// <inheritdoc/>
        public abstract void CreateOrUpdateLayout();
    }
}
