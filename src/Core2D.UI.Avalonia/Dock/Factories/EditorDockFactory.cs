// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.UI.Avalonia.Dock.Views;
using Core2D.UI.Avalonia.Dock.Documents;
using Core2D.UI.Avalonia.Dock.Tools;
using Core2D.Editor;
using Dock.Model;
using Dock.Model.Controls;

namespace Core2D.UI.Avalonia.Dock.Factories
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

            // Tools

            var projectTool = new ProjectTool
            {
                Id = nameof(ProjectTool),
                Title = "Project"
            };

            var optionsTool = new OptionsTool
            {
                Id = nameof(OptionsTool),
                Title = "Options"
            };

            var imagesTool = new ImagesTool
            {
                Id = nameof(ImagesTool),
                Title = "Images"
            };

            var groupsTool = new GroupsTool
            {
                Id = nameof(GroupsTool),
                Title = "Groups"
            };

            var databasesTool = new DatabasesTool
            {
                Id = nameof(DatabasesTool),
                Title = "Databases"
            };

            var scriptTool = new ScriptTool
            {
                Id = nameof(ScriptTool),
                Title = "Script"
            };

            var browserTool = new BrowserTool
            {
                Id = nameof(BrowserTool),
                Title = "Browser"
            };

            var stylesTool = new StylesTool
            {
                Id = nameof(StylesTool),
                Title = "Styles"
            };

            var templatesTool = new TemplatesTool
            {
                Id = nameof(TemplatesTool),
                Title = "Templates"
            };

            var containerTool = new ContainerTool
            {
                Id = nameof(ContainerTool),
                Title = "Container"
            };

            var zoomTool = new ZoomTool
            {
                Id = nameof(ZoomTool),
                Title = "Zoom"
            };

            var shapeTool = new ShapeTool
            {
                Id = nameof(ShapeTool),
                Title = "Shape"
            };

            var toolsTool = new ToolsTool
            {
                Id = nameof(ToolsTool),
                Title = "Tools"
            };

            var dataTool = new DataTool
            {
                Id = nameof(DataTool),
                Title = "Data"
            };

            var styleTool = new StyleTool
            {
                Id = nameof(StyleTool),
                Title = "Style"
            };

            var templateTool = new TemplateTool
            {
                Id = nameof(TemplateTool),
                Title = "Template"
            };

            // Panes

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
                        CurrentView = projectTool,
                        Views = new ObservableCollection<IView>
                        {
                            projectTool,
                            optionsTool,
                            imagesTool
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
                        CurrentView = groupsTool,
                        Views = new ObservableCollection<IView>
                        {
                            groupsTool,
                            databasesTool,
                            scriptTool,
                            browserTool
                        }
                    }
                }
            };

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
                        CurrentView = stylesTool,
                        Views = new ObservableCollection<IView>
                        {
                            stylesTool,
                            templatesTool,
                            containerTool,
                            zoomTool
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
                        CurrentView = shapeTool,
                        Views = new ObservableCollection<IView>
                        {
                            shapeTool,
                            toolsTool,
                            dataTool,
                            styleTool,
                            templateTool
                        }
                    }
                }
            };

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

            // Editor

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

            // Views

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

            var aboutView = new AboutView
            {
                Id = nameof(AboutView),
                Title = "About"
            };

            var browserView = new BrowserView
            {
                Id = nameof(BrowserView),
                Title = "Browser"
            };

            var scriptView = new ScriptView
            {
                Id = nameof(ScriptView),
                Title = "Script"
            };

            var documentView = new DocumentView
            {
                Id = nameof(DocumentView),
                Title = "Document"
            };

            var dashboardView = new DashboardView
            {
                Id = nameof(DashboardView),
                Title = "Dashboard"
            };

            // Root

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
                    scriptView,
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
                [nameof(ScriptTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
                [nameof(BrowserTool)] = () => _serviceProvider.GetService<ProjectEditor>(),
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
                [nameof(ScriptView)] = () => _serviceProvider.GetService<ProjectEditor>(),
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
