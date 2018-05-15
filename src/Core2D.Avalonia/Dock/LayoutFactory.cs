// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Editor;
using Dock.Model;

namespace Core2D.Avalonia.Dock
{
    /// <summary>
    /// Layout factory.
    /// </summary>
    public class LayoutFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LayoutFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void UpdateWindows(IList<IDockWindow> windows, IList<IDockView> views, object context)
        {
            foreach (var window in windows)
            {
                var host = _serviceProvider.GetService<IDockHost>();

                window.Host = host;
                window.Context = context;

                UpdateLayout(window.Layout, views, context);
            }
        }

        public void UpdateViews(IList<IDockView> source, IList<IDockView> views, object context)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var original = source[i];
                source[i] = views.FirstOrDefault(v => v.Title == original.Title);
                source[i].Windows = original.Windows;

                if (original.Windows != null)
                {
                    UpdateWindows(original.Windows, views, context);
                }
            }
        }

        public void UpdateLayout(IDockLayout layout, IList<IDockView> views, object context)
        {
            UpdateViews(layout.Views, views, context);

            layout.CurrentView = views.FirstOrDefault(v => v.Title == layout.CurrentView?.Title);

            if (layout.Children != null)
            {
                foreach (var child in layout.Children)
                {
                    UpdateLayout(child, views, context);
                }
            }
        }

        public IDockLayout CreateDefaultLayout(IList<IDockView> views)
        {
            return new DockLayout
            {
                Row = 0,
                Column = 0,
                Views = new ObservableCollection<IDockView>(views),
                CurrentView = views.FirstOrDefault(v => v.Title == "Dashboard"),
                Children = new ObservableCollection<IDockLayout>
                {
                    new DockLayout
                    {
                        Row = 0,
                        Column = 0,
                        Views = new ObservableCollection<IDockView>
                        {
                            views.FirstOrDefault(v => v.Title == "Project"),
                            views.FirstOrDefault(v => v.Title == "Options"),
                            views.FirstOrDefault(v => v.Title == "Images")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Project")
                    },
                    new DockLayout
                    {
                        Row = 2,
                        Column = 0,
                        Views = new ObservableCollection<IDockView>
                        {
                            views.FirstOrDefault(v => v.Title == "Groups"),
                            views.FirstOrDefault(v => v.Title == "Databases")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Groups")
                    },
                    new DockLayout
                    {
                        Row = 0,
                        Column = 0,
                        Views = new ObservableCollection<IDockView>
                        {
                            views.FirstOrDefault(v => v.Title == "Styles"),
                            views.FirstOrDefault(v => v.Title == "Templates"),
                            views.FirstOrDefault(v => v.Title == "Container"),
                            views.FirstOrDefault(v => v.Title == "Zoom")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Styles")
                    },
                    new DockLayout
                    {
                        Row = 2,
                        Column = 0,
                        Views = new ObservableCollection<IDockView>
                        {
                            views.FirstOrDefault(v => v.Title == "Tools"),
                            views.FirstOrDefault(v => v.Title == "Shape"),
                            views.FirstOrDefault(v => v.Title == "Data"),
                            views.FirstOrDefault(v => v.Title == "Style"),
                            views.FirstOrDefault(v => v.Title == "Template")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Tools")
                    },
                }
            };
        }

        public void CreateOrUpdateLayout()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var views = _serviceProvider.GetService<IDockView[], IList<IDockView>>((c) => new ObservableCollection<IDockView>(c));

            if (editor.Layout != null)
            {
                var layout = editor.Layout;

                UpdateLayout(layout, views, editor);

                layout.CurrentView = views.FirstOrDefault(v => v.Title == "Dashboard");
            }
            else
            {
                editor.Layout = CreateDefaultLayout(views);
            }
        }
    }
}
