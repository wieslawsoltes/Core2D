// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Avalonia.Dock.Views;
using Core2D.Avalonia.Dock.Documents;
using Core2D.Avalonia.Dock.Tools;
using Core2D.Editor;
using Dock.Model;
using Dock.Model.Controls;

namespace Core2D.Avalonia.Dock.Factories
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
            // Documents

            var pageDocument = new PageDocument
            {
                Id = nameof(PageDocument),
                Title = "Page"
            };

            // Left / Top

            var projectView = new ProjectTool
            {
                Id = nameof(ProjectTool),
                Title = "Project"
            };

            var optionsView = new OptionsTool
            {
                Id = nameof(OptionsTool),
                Title = "Options"
            };

            var imagesView = new ImagesTool
            {
                Id = nameof(ImagesTool),
                Title = "Images"
            };

            // Left / Bottom

            var groupsView = new GroupsTool
            {
                Id = nameof(GroupsTool),
                Title = "Groups"
            };

            var databasesView = new DatabasesTool
            {
                Id = nameof(DatabasesTool),
                Title = "Databases"
            };

            // Right / Top

            var stylesView = new StylesTool
            {
                Id = nameof(StylesTool),
                Title = "Styles"
            };

            var templatesView = new TemplatesTool
            {
                Id = nameof(TemplatesTool),
                Title = "Templates"
            };

            var containerView = new ContainerTool
            {
                Id = nameof(ContainerTool),
                Title = "Container"
            };

            var zoomView = new ZoomTool
            {
                Id = nameof(ZoomTool),
                Title = "Zoom"
            };

            // Right / Bottom

            var shapeView = new ShapeTool
            {
                Id = nameof(ShapeTool),
                Title = "Shape"
            };

            var toolsView = new ToolsTool
            {
                Id = nameof(ToolsTool),
                Title = "Tools"
            };

            var dataView = new DataTool
            {
                Id = nameof(DataTool),
                Title = "Data"
            };

            var styleView = new StyleTool
            {
                Id = nameof(StyleTool),
                Title = "Style"
            };

            var templateView = new TemplateTool
            {
                Id = nameof(TemplateTool),
                Title = "Template"
            };

            // Left Pane

            var leftPane = new LayoutDock
            {
                Id = nameof(ILayoutDock),
                Title = "EditorLeft",
                Orientation = Orientation.Vertical,
                Proportion = 0.17,
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Title = "EditorLeftTop",
                        Proportion = double.NaN,
                        CurrentView = projectView,
                        Views = new ObservableCollection<IView>
                        {
                            projectView,
                            optionsView,
                            imagesView
                        }
                    },
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Title = "LeftTopSplitter"
                    },
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Title = "EditorLeftBottom",
                        Proportion = double.NaN,
                        CurrentView = groupsView,
                        Views = new ObservableCollection<IView>
                        {
                            groupsView,
                            databasesView
                        }
                    }
                }
            };

            // Right Pane

            var rightPane = new LayoutDock
            {
                Id = nameof(ILayoutDock),
                Title = "EditorRight",
                Orientation = Orientation.Vertical,
                Proportion = 0.17,
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Title = "EditorRightTop",
                        Proportion = double.NaN,
                        CurrentView = stylesView,
                        Views = new ObservableCollection<IView>
                        {
                            stylesView,
                            templatesView,
                            containerView,
                            zoomView
                        }
                    },
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Title = "RightTopSplitter"
                    },
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Title = "EditorRightBottom",
                        Proportion = double.NaN,
                        CurrentView = shapeView,
                        Views = new ObservableCollection<IView>
                        {
                            shapeView,
                            toolsView,
                            dataView,
                            styleView,
                            templateView
                        }
                    }
                }
            };

            // Documents

            var documentsPane = new DocumentDock
            {
                Id = nameof(IDocumentDock),
                Title = "DocumentsPane",
                Proportion = double.NaN,
                CurrentView = pageDocument,
                Views = new ObservableCollection<IView>
                {
                    pageDocument
                }
            };

            // Main

            var editorLayout = new LayoutDock
            {
                Id = nameof(ILayoutDock),
                Title = "EditorLayout",
                Orientation = Orientation.Horizontal,
                Proportion = double.NaN,
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    leftPane,
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Title = "LeftSplitter"
                    },
                    documentsPane,
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Title = "RightSplitter"
                    },
                    rightPane
                }
            };

            var editorView = new EditorView
            {
                Id = nameof(EditorView),
                Title = "Editor",
                CurrentView = editorLayout,
                Views = new ObservableCollection<IView>
                {
                   editorLayout
                }
            };

            // About

            var aboutView = new AboutView
            {
                Id = nameof(AboutView),
                Title = "About"
            };

            // Browser

            var browserView = new BrowserView
            {
                Id = nameof(BrowserView),
                Title = "Browser"
            };

            // Document

            var documentView = new DocumentView
            {
                Id = nameof(DocumentView),
                Title = "Document"
            };

            // Dashboard

            var dashboardView = new DashboardView
            {
                Id = nameof(DashboardView),
                Title = "Dashboard"
            };

            // Main

            var layout = new RootDock
            {
                Id = nameof(IRootDock),
                Title = "Root",
                CurrentView = dashboardView,
                DefaultView = dashboardView,
                Views = new ObservableCollection<IView>
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
        public override void InitLayout(IView layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                // Defaults
                [nameof(IRootDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ILayoutDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(IDocumentDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(IToolDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ISplitterDock)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(IDocumentTab)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(IToolTab)] = () => _serviceProvider.GetService<ProjectEditor>(),
                // Documents
                [nameof(PageDocument)] = () => _serviceProvider.GetService<ProjectEditor>(),
                // Tools
                [nameof(ProjectTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(OptionsTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ImagesTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(GroupsTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(DatabasesTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(StylesTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(TemplatesTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ContainerTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ZoomTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ToolsTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(ShapeTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(DataTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(StyleTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(TemplateTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                // Views
                [nameof(EditorView)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(AboutView)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(BrowserView)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(DocumentView)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(DashboardView)] = () => _serviceProvider.GetService<ProjectEditor>()
            };

            HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<IDockHost>()
            };

            base.InitLayout(layout);
        }
    }
}
