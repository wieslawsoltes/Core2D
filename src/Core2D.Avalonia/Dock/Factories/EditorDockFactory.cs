// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Avalonia.Dock.Views;
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
        public override IDock CreateDefaultLayout(object context)
        {
            // Dashboard

            var dashboardView = new DashboardView
            {
                Dock = "",
                Title = "Dashboard",
                Context = context,
                Factory = this
            };

            // Page

            var pageView = new PageView
            {
                Dock = "",
                Title = "Page",
                Context = context,
                Factory = this
            };

            // Left / Top

            var projectView = new ProjectView
            {
                Dock = "",
                Width = 180,
                Title = "Project",
                Context = context,
                Factory = this
            };

            var optionsView = new OptionsView
            {
                Dock = "",
                Width = 180,
                Title = "Options",
                Context = context,
                Factory = this
            };

            var imagesView = new ImagesView
            {
                Dock = "",
                Width = 180,
                Title = "Images",
                Context = context,
                Factory = this
            };

            // Left / Bottom

            var groupsView = new GroupsView
            {
                Dock = "",
                Width = 180,
                Title = "Groups",
                Context = context,
                Factory = this
            };

            var databasesView = new DatabasesView
            {
                Dock = "",
                Width = 180,
                Title = "Databases",
                Context = context,
                Factory = this
            };

            // Left

            var leftPane = new DockLayout
            {
                Dock = "Left",
                Width = 180,
                Context = context,
                Factory = this,
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    new DockStrip
                    {
                        Dock = "Top",
                        Width = 180,
                        Context = context,
                        Factory = this,
                        CurrentView = projectView,
                        Views = new ObservableCollection<IDock>
                        {
                            projectView,
                            optionsView,
                            imagesView
                        }
                    },
                    new DockStrip
                    {
                        Dock = "Bottom",
                        Width = 180,
                        Context = context,
                        Factory = this,
                        CurrentView = groupsView,
                        Views = new ObservableCollection<IDock>
                        {
                            groupsView,
                            databasesView
                        }
                    }
                }
            };

            // Right / Top

            var stylesView = new StylesView
            {
                Dock = "",
                Width = 240,
                Title = "Styles",
                Context = context,
                Factory = this
            };

            var templatesView = new TemplatesView
            {
                Dock = "",
                Width = 240,
                Title = "Templates",
                Context = context,
                Factory = this
            };

            var containerView = new ContainerView
            {
                Dock = "",
                Width = 240,
                Title = "Container",
                Context = context,
                Factory = this
            };

            var zoomView = new ZoomView
            {
                Dock = "",
                Width = 240,
                Title = "Zoom",
                Context = context,
                Factory = this
            };

            // Right / Bottom

            var toolsView = new ToolsView
            {
                Dock = "",
                Width = 240,
                Title = "Tools",
                Context = context,
                Factory = this
            };

            var shapeView = new ShapeView
            {
                Dock = "",
                Width = 240,
                Title = "Shape",
                Context = context,
                Factory = this
            };

            var dataView = new DataView
            {
                Dock = "",
                Width = 240,
                Title = "Data",
                Context = context,
                Factory = this
            };

            var styleView = new StyleView
            {
                Dock = "",
                Width = 240,
                Title = "Style",
                Context = context,
                Factory = this
            };

            var templateView = new TemplateView
            {
                Dock = "",
                Width = 240,
                Title = "Template",
                Context = context,
                Factory = this
            };

            // Right

            var rightPane = new DockLayout
            {
                Dock = "Right",
                Width = 240,
                Context = context,
                Factory = this,
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    new DockStrip
                    {
                        Dock = "Top",
                        Width = 240,
                        Context = context,
                        Factory = this,
                        CurrentView = stylesView,
                        Views = new ObservableCollection<IDock>
                        {
                            stylesView,
                            templatesView,
                            containerView,
                            zoomView
                        }
                    },
                    new DockStrip
                    {
                        Dock = "Bottom",
                        Width = 240,
                        Context = context,
                        Factory = this,
                        CurrentView = toolsView,
                        Views = new ObservableCollection<IDock>
                        {
                            toolsView,
                            shapeView,
                            dataView,
                            styleView,
                            templateView
                        }
                    }
                }
            };

            // Editor

            var editorLayout = new DockLayout
            {
                Dock = "",
                Context = context,
                Factory = this,
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    leftPane,
                    pageView,
                    rightPane
                }
            };

            var editorView = new EditorView
            {
                Dock = "",
                Title = "Editor",
                Context = context,
                Factory = this,
                CurrentView = editorLayout,
                Views = new ObservableCollection<IDock>
                {
                   editorLayout
                }
            };

            // About

            var aboutView = new AboutView
            {
                Dock = "",
                Title = "About",
                Context = context,
                Factory = this
            };

            // Browser

            var browserView = new BrowserView
            {
                Dock = "",
                Title = "Browser",
                Context = context,
                Factory = this
            };

            // Document

            var documentView = new DocumentView
            {
                Dock = "",
                Title = "Document",
                Context = context,
                Factory = this
            };

            // Main

            var layout = new DockRoot
            {
                Dock = "",
                Context = context,
                Factory = this,
                CurrentView = dashboardView,
                Views = new ObservableCollection<IDock>
                {
                    dashboardView,
                    editorView,
                    aboutView,
                    browserView,
                    documentView
                }
            };

            return layout;
        }

        /// <inheritdoc/>
        public override void CreateOrUpdateLayout()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            if (editor.Layout != null)
            {
                UpdateView(editor.Layout, editor);
            }
            else
            {
                editor.Layout = CreateDefaultLayout(editor);
            }
        }
    }
}
