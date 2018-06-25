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
using Dock.Avalonia.Controls;

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
                Width = double.NaN,
                Height = double.NaN,
                Title = "Page"
            };

            // Left / Top

            var projectView = new ProjectTool
            {
                Id = nameof(ProjectTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Project"
            };

            var optionsView = new OptionsTool
            {
                Id = nameof(OptionsTool),
                Width = 200,
                Height = 200,
                Title = "Options"
            };

            var imagesView = new ImagesTool
            {
                Id = nameof(ImagesTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Images"
            };

            // Left / Bottom

            var groupsView = new GroupsTool
            {
                Id = nameof(GroupsTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Groups"
            };

            var databasesView = new DatabasesTool
            {
                Id = nameof(DatabasesTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Databases"
            };

            // Right / Top

            var stylesView = new StylesTool
            {
                Id = nameof(StylesTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Styles"
            };

            var templatesView = new TemplatesTool
            {
                Id = nameof(TemplatesTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Templates"
            };

            var containerView = new ContainerTool
            {
                Id = nameof(ContainerTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Container"
            };

            var zoomView = new ZoomTool
            {
                Id = nameof(ZoomTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Zoom"
            };

            // Right / Bottom

            var shapeView = new ShapeTool
            {
                Id = nameof(ShapeTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Shape"
            };

            var toolsView = new ToolsTool
            {
                Id = nameof(ToolsTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Tools"
            };

            var dataView = new DataTool
            {
                Id = nameof(DataTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Data"
            };

            var styleView = new StyleTool
            {
                Id = nameof(StyleTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Style"
            };

            var templateView = new TemplateTool
            {
                Id = nameof(TemplateTool),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Template"
            };

            // Left Pane

            var leftPane = new LayoutDock
            {
                Id = nameof(ILayoutDock),
                Dock = "Left",
                Width = 200,
                Height = double.NaN,
                Title = "EditorLeft",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Dock = "Top",
                        Width = double.NaN,
                        Height = 340,
                        Title = "EditorLeftTop",
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
                        Dock = "Top",
                        Title = "LeftTopSplitter"
                    },
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Dock = "Bottom",
                        Width = double.NaN,
                        Height = double.NaN,
                        Title = "EditorLeftBottom",
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
                Dock = "Right",
                Width = 240,
                Height = double.NaN,
                Title = "EditorRight",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Dock = "Top",
                        Width = double.NaN,
                        Height = 340,
                        Title = "EditorRightTop",
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
                        Dock = "Top",
                        Title = "RightTopSplitter"
                    },
                    new ToolDock
                    {
                        Id = nameof(IToolDock),
                        Dock = "Bottom",
                        Width = double.NaN,
                        Height = double.NaN,
                        Title = "EditorRightBottom",
                        CurrentView = toolsView,
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
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "DocumentsPane",
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
                Dock = "",
                Width = double.NaN,
                Height = double.NaN,
                Title = "EditorLayout",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    leftPane,
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Dock = "Left",
                        Title = "LeftSplitter"
                    },
                    rightPane,
                    new SplitterDock()
                    {
                        Id = nameof(ISplitterDock),
                        Dock = "Right",
                        Title = "RightSplitter"
                    },
                    documentsPane
                }
            };

            var editorView = new EditorView
            {
                Id = nameof(EditorView),
                Width = double.NaN,
                Height = double.NaN,
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
                Width = double.NaN,
                Height = double.NaN,
                Title = "About"
            };

            // Browser

            var browserView = new BrowserView
            {
                Id = nameof(BrowserView),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Browser"
            };

            // Document

            var documentView = new DocumentView
            {
                Id = nameof(DocumentView),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Document"
            };

            // Dashboard

            var dashboardView = new DashboardView
            {
                Id = nameof(DashboardView),
                Width = double.NaN,
                Height = double.NaN,
                Title = "Dashboard"
            };

            // Main

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
            this.ContextLocator = new Dictionary<string, Func<object>>
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
            
            this.HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => _serviceProvider.GetService<IDockHost>()
            };

            base.InitLayout(layout, context);
        }
    }
}
