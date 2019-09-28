// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.UI.Avalonia.Dock.Views;
using Core2D.UI.Avalonia.Dock.Documents;
using Core2D.UI.Avalonia.Dock.Tools;
using Core2D.Editor;
using DM = Dock.Model;
using DMC = Dock.Model.Controls;

namespace Core2D.UI.Avalonia.Dock.Factories
{
    /// <summary>
    /// Editor dock factory.
    /// </summary>
    public class EditorDockFactory : DM.Factory
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
        public override DM.IDock CreateLayout()
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

            var documentTool = new DocumentTool
            {
                Id = nameof(DocumentTool),
                Title = "Document"
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

            var leftPane = new DMC.ProportionalDock
            {
                Id = nameof(DMC.IProportionalDock),
                Title = "EditorLeft",
                Orientation = DM.Orientation.Vertical,
                Proportion = 0.17,
                ActiveDockable = null,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    new DMC.ToolDock
                    {
                        Id = nameof(DMC.IToolDock),
                        Title = "EditorLeftTop",
                        Proportion = double.NaN,
                        ActiveDockable = projectTool,
                        VisibleDockables = CreateList<DM.IDockable>
                        (
                            projectTool,
                            optionsTool,
                            imagesTool
                        )
                    },
                    new DMC.SplitterDock()
                    {
                        Id = nameof(DMC.ISplitterDock),
                        Title = "LeftTopSplitter"
                    },
                    new DMC.ToolDock
                    {
                        Id = nameof(DMC.IToolDock),
                        Title = "EditorLeftBottom",
                        Proportion = double.NaN,
                        ActiveDockable = groupsTool,
                        VisibleDockables = CreateList<DM.IDockable>
                        (
                            groupsTool,
                            databasesTool
                        )
                    }
                )
            };

            var rightPane = new DMC.ProportionalDock
            {
                Id = nameof(DMC.IProportionalDock),
                Title = "EditorRight",
                Orientation = DM.Orientation.Vertical,
                Proportion = 0.17,
                ActiveDockable = null,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    new DMC.ToolDock
                    {
                        Id = nameof(DMC.IToolDock),
                        Title = "EditorRightTop",
                        Proportion = double.NaN,
                        ActiveDockable = stylesTool,
                        VisibleDockables = CreateList<DM.IDockable>
                        (
                            stylesTool,
                            templatesTool,
                            containerTool,
                            zoomTool
                        )
                    },
                    new DMC.SplitterDock()
                    {
                        Id = nameof(DMC.ISplitterDock),
                        Title = "RightTopSplitter"
                    },
                    new DMC.ToolDock
                    {
                        Id = nameof(DMC.IToolDock),
                        Title = "EditorRightBottom",
                        Proportion = double.NaN,
                        ActiveDockable = shapeTool,
                        VisibleDockables = CreateList<DM.IDockable>
                        (
                            shapeTool,
                            toolsTool,
                            dataTool,
                            styleTool,
                            templateTool
                        )
                    }
                )
            };

            var documentsPane = new DMC.DocumentDock
            {
                Id = nameof(DMC.IDocumentDock),
                Title = "DocumentsPane",
                Proportion = double.NaN,
                ActiveDockable = pageDocument,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    pageDocument,
                    documentTool,
                    scriptTool,
                    browserTool
                )
            };

            // Editor

            var editorLayout = new DMC.ProportionalDock
            {
                Id = nameof(DMC.IProportionalDock),
                Title = "EditorLayout",
                Orientation = DM.Orientation.Horizontal,
                Proportion = double.NaN,
                ActiveDockable = null,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    leftPane,
                    new DMC.SplitterDock()
                    {
                        Id = nameof(DMC.ISplitterDock),
                        Title = "LeftSplitter"
                    },
                    documentsPane,
                    new DMC.SplitterDock()
                    {
                        Id = nameof(DMC.ISplitterDock),
                        Title = "RightSplitter"
                    },
                    rightPane
                )
            };

            // Views

            var editorView = new EditorView
            {
                Id = nameof(EditorView),
                Title = "Editor",
                ActiveDockable = editorLayout,
                VisibleDockables = CreateList<DM.IDockable>
                (
                   editorLayout
                )
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

            var layout = new DMC.RootDock
            {
                Id = nameof(DMC.IRootDock),
                Title = "Root",
                ActiveDockable = dashboardView,
                DefaultDockable = dashboardView,
                VisibleDockables = CreateList<DM.IDockable>
                (
                    dashboardView,
                    editorView,
                    aboutView,
                    browserView,
                    scriptView,
                    documentView
                )
            };

            return layout;
        }

        /// <inheritdoc/>
        public override void InitLayout(DM.IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                // Defaults
                [nameof(DMC.IRootDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IPinDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IProportionalDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IDocumentDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IToolDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.ISplitterDock)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DM.IDockWindow)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.IDocument)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DMC.ITool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                // Documents
                [nameof(PageDocument)] = () => _serviceProvider.GetService<IProjectEditor>(),
                // Tools
                [nameof(ScriptTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(BrowserTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DocumentTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ProjectTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(OptionsTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ImagesTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(GroupsTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DatabasesTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(StylesTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(TemplatesTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ContainerTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ZoomTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ToolsTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ShapeTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DataTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(StyleTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(TemplateTool)] = () => _serviceProvider.GetService<IProjectEditor>(),
                // Views
                [nameof(EditorView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(AboutView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(ScriptView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(BrowserView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DocumentView)] = () => _serviceProvider.GetService<IProjectEditor>(),
                [nameof(DashboardView)] = () => _serviceProvider.GetService<IProjectEditor>()
            };

            this.HostWindowLocator = new Dictionary<string, Func<DM.IHostWindow>>
            {
                [nameof(DM.IDockWindow)] = () => _serviceProvider.GetService<DM.IHostWindow>()
            };

            base.InitLayout(layout);
        }
    }
}
