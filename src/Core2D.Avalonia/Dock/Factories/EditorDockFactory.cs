// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Editor;
using Dock.Avalonia.Factories;
using Dock.Model;

namespace Core2D.Avalonia.Dock.Factories
{
    /// <summary>
    /// Editor dock factory.
    /// </summary>
    public class EditorDockFactory : BaseDockFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="EditorDockFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public EditorDockFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override IDockLayout CreateDefaultLayout(IList<IDockView> views)
        {
            return new DockLayout
            {
                Dock = "",
                CurrentView = views.FirstOrDefault(v => v.Title == "Dashboard"),
                Children = new ObservableCollection<IDock>
                {
                    views.FirstOrDefault(v => v.Title == "Dashboard"),
                    views.FirstOrDefault(v => v.Title == "Editor"),
                    new DockLayout
                    {
                        Dock = "Left",
                        Children = new ObservableCollection<IDock>
                        {
                            views.FirstOrDefault(v => v.Title == "Project"),
                            views.FirstOrDefault(v => v.Title == "Options"),
                            views.FirstOrDefault(v => v.Title == "Images")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Project"),
                        Factory = this
                    },
                    new DockLayout
                    {
                        Dock = "Left",
                        Children = new ObservableCollection<IDock>
                        {
                            views.FirstOrDefault(v => v.Title == "Groups"),
                            views.FirstOrDefault(v => v.Title == "Databases")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Groups"),
                        Factory = this
                    },
                    new DockLayout
                    {
                        Dock = "Right",
                        Children = new ObservableCollection<IDock>
                        {
                            views.FirstOrDefault(v => v.Title == "Styles"),
                            views.FirstOrDefault(v => v.Title == "Templates"),
                            views.FirstOrDefault(v => v.Title == "Container"),
                            views.FirstOrDefault(v => v.Title == "Zoom")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Styles"),
                        Factory = this
                    },
                    new DockLayout
                    {
                        Dock = "Right",
                        Children = new ObservableCollection<IDock>
                        {
                            views.FirstOrDefault(v => v.Title == "Tools"),
                            views.FirstOrDefault(v => v.Title == "Shape"),
                            views.FirstOrDefault(v => v.Title == "Data"),
                            views.FirstOrDefault(v => v.Title == "Style"),
                            views.FirstOrDefault(v => v.Title == "Template")
                        },
                        CurrentView = views.FirstOrDefault(v => v.Title == "Tools"),
                        Factory = this
                    },
                },
                Factory = this
            };
        }

        /// <inheritdoc/>
        public override void CreateOrUpdateLayout()
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
