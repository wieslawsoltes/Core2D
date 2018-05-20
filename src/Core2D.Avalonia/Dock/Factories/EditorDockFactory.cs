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

            ContextLocator = new Dictionary<Type, Func<object>>
            {
                [typeof(IDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [typeof(IDockWindow)] = () => _serviceProvider.GetService<ProjectEditor>()
            };
        }

        /// <inheritdoc/>
        public override IDock CreateDefaultLayout()
        {
            // Dashboard

            var dashboardView = new DashboardView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Dashboard"
            };

            // Page

            var pageView = new PageView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Page"
            };

            // Left / Top

            var projectView = new ProjectView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Project"
            };

            var optionsView = new OptionsView
            {
                Dock = "",
                Width = 200,
                Height = 200,
                Title = "Options"
            };

            var imagesView = new ImagesView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Images"
            };

            // Left / Bottom

            var groupsView = new GroupsView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Groups"
            };

            var databasesView = new DatabasesView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Databases"
            };

            // Right / Top

            var stylesView = new StylesView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Styles"
            };

            var templatesView = new TemplatesView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Templates"
            };

            var containerView = new ContainerView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Container"
            };

            var zoomView = new ZoomView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Zoom"
            };

            // Right / Bottom

            var toolsView = new ToolsView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Tools"
            };

            var shapeView = new ShapeView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Shape"
            };

            var dataView = new DataView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Data"
            };

            var styleView = new StyleView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Style"
            };

            var templateView = new TemplateView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Template"
            };

            // Left

            var leftPane = new DockLayout
            {
                Dock = "Left",
                Width = 200,
                Height = double.NaN,
                Title = "EditorLeft",
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    new DockStrip
                    {
                        Dock = "Top",
                        Width = double.NaN,
                        Height = 340,
                        Title = "EditorLeftTop",
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
                        Width = double.NaN,
                        Height = double.NaN,
                        Title = "EditorLeftBottom",
                        CurrentView = groupsView,
                        Views = new ObservableCollection<IDock>
                        {
                            groupsView,
                            databasesView
                        }
                    }
                }
            };

            // Right

            var rightPane = new DockLayout
            {
                Dock = "Right",
                Width = 240,
                Height = double.NaN,
                Title = "EditorRight",
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    new DockStrip
                    {
                        Dock = "Top",
                        Width = double.NaN,
                        Height = 340,
                        Title = "EditorRightTop",
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
                        Width = double.NaN,
                        Height = double.NaN,
                        Title = "EditorRightBottom",
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
                Width = double.NaN,
                Height = double.NaN,
                Title = "EditorLayout",
                CurrentView = null,
                Views = new ObservableCollection<IDock>
                {
                    leftPane,
                    rightPane,
                    pageView
                }
            };

            var editorView = new EditorView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Editor",
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
                Width = double.NaN,
                Height = double.NaN,
                Title = "About"
            };

            // Browser

            var browserView = new BrowserView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Browser"
            };

            // Document

            var documentView = new DocumentView
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "Document"
            };

            // Main

            var layout = new DockRoot
            {
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
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
                Update(editor.Layout, editor);

                var dashboard = editor.Layout.Views.FirstOrDefault(v => v.Title == "Dashboard");
                if (dashboard != null)
                {
                    editor.Layout.CurrentView = dashboard;
                }
            }
            else
            {
                var layout = CreateDefaultLayout();
                Update(layout, editor);
                editor.Layout = layout;
            }
        }
    }
}
