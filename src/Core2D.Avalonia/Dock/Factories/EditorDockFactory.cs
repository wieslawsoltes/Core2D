// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Avalonia.Dock.Views;
using Core2D.Avalonia.Dock.Documents;
using Core2D.Avalonia.Dock.Tools;
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
                Proportion = double.NaN,
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
                Proportion = double.NaN,
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
        public override void InitLayout(IView layout, object context)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                // Defaults
                [nameof(IRootDock)] = () => context,
                [nameof(ILayoutDock)] = () => context,
                [nameof(IDocumentDock)] = () => context,
                [nameof(IToolDock)] = () => context,
                [nameof(ISplitterDock)] = () => context,
                [nameof(IDockWindow)] = () => context,
                [nameof(IDocumentTab)] = () => context,
                [nameof(IToolTab)] = () => context,
                // Documents
                [nameof(PageDocument)] = () => context,
                // Tools
                [nameof(ProjectTool)] = () => context,
                [nameof(OptionsTool)] = () => context,
                [nameof(ImagesTool)] = () => context,
                [nameof(GroupsTool)] = () => context,
                [nameof(DatabasesTool)] = () => context,
                [nameof(StylesTool)] = () => context,
                [nameof(TemplatesTool)] = () => context,
                [nameof(ContainerTool)] = () => context,
                [nameof(ZoomTool)] = () => context,
                [nameof(ToolsTool)] = () => context,
                [nameof(ShapeTool)] = () => context,
                [nameof(DataTool)] = () => context,
                [nameof(StyleTool)] = () => context,
                [nameof(TemplateTool)] = () => context,
                // Views
                [nameof(EditorView)] = () => context,
                [nameof(AboutView)] = () => context,
                [nameof(BrowserView)] = () => context,
                [nameof(DocumentView)] = () => context,
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
