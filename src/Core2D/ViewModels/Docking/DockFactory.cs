using System;
using System.Collections.Generic;
using Core2D.ViewModels.Docking.Docks;
using Core2D.ViewModels.Docking.Tools;
using Core2D.ViewModels.Docking.Tools.Libraries;
using Core2D.ViewModels.Docking.Tools.Options;
using Core2D.ViewModels.Docking.Tools.Properties;
using Core2D.ViewModels.Docking.Views;
using Core2D.ViewModels.Editor;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;

namespace Core2D.ViewModels.Docking
{
    public class DockFactory : Factory
    {
        private readonly ProjectEditorViewModel _projectEditor;
        private IRootDock? _rootDock;
        private IDocumentDock? _pagesDock;
        private IProportionalDock? _homeDock;

        public IRootDock? RootDock => _rootDock;

        public IDocumentDock? PagesDock => _pagesDock;

        public IProportionalDock? HomeDock => _homeDock;

        public DockFactory(ProjectEditorViewModel projectEditor)
        {
            _projectEditor = projectEditor;
        }

        public override IDocumentDock CreateDocumentDock()
        {
            return new PageDocumentDock();
        }

        public override IRootDock CreateLayout()
        {
            // Tool Windows - Libraries

            var styleLibraryViewModel = new StyleLibraryViewModel()
            {
                Id = "StyleLibrary",
                Title = "Styles"
            };

            var groupLibraryViewModel = new GroupLibraryViewModel()
            {
                Id = "GroupLibrary",
                Title = "Groups"
            };

            var databaseLibraryViewModel = new DatabaseLibraryViewModel()
            {
                Id = "DatabaseLibrary",
                Title = "Databases"
            };

            var templateLibraryViewModel = new TemplateLibraryViewModel()
            {
                Id = "TemplateLibrary",
                Title = "Templates"
            };

            var scriptLibraryViewModel = new ScriptLibraryViewModel()
            {
                Id = "ScriptLibrary",
                Title = "Scripts"
            };

            // Tool Windows - Options

            var projectOptionsViewModel = new ProjectOptionsViewModel()
            {
                Id = "ProjectOptions",
                Title = "Options"
            };

            var rendererOptionsViewModel = new RendererOptionsViewModel()
            {
                Id = "RendererOptions",
                Title = "Renderer"
            };

            var zoomOptionsViewModel = new ZoomOptionsViewModel()
            {
                Id = "ZoomOptions",
                Title = "Zoom"
            };

            var imageOptionsViewModel = new ImageOptionsViewModel()
            {
                Id = "ImageOptions",
                Title = "Images"
            };

            // Tool Windows - Properties

            var pagePropertiesViewModel = new PagePropertiesViewModel()
            {
                Id = "PageProperties",
                Title = "Page"
            };

            var shapePropertiesViewModel = new ShapePropertiesViewModel()
            {
                Id = "ShapeProperties",
                Title = "Shape"
            };

            var stylePropertiesViewModel = new StylePropertiesViewModel()
            {
                Id = "StyleProperties",
                Title = "Style"
            };

            var dataPropertiesViewModel = new DataPropertiesViewModel()
            {
                Id = "DataProperties",
                Title = "Data"
            };

            var statePropertiesViewModel = new StatePropertiesViewModel()
            {
                Id = "StateProperties",
                Title = "State"
            };

            // Tool Windows

            var projectExplorerViewModel = new ProjectExplorerViewModel()
            {
                Id = "ProjectExplorer",
                Title = "Project Explorer"
            };

            var objectBrowserViewModel = new ObjectBrowserViewModel()
            {
                Id = "ObjectBrowser",
                Title = "Object Browser"
            };

            // Home Perspective

            var leftTopToolDock = new ToolDock
            {
                ActiveDockable = projectExplorerViewModel,
                VisibleDockables = CreateList<IDockable>(
                    projectExplorerViewModel,
                    objectBrowserViewModel,
                    scriptLibraryViewModel),
                Alignment = Alignment.Left,
                GripMode = GripMode.Visible
            };

            var leftBottomToolDock = new ToolDock
            {
                Proportion = 0.35,
                ActiveDockable = pagePropertiesViewModel,
                VisibleDockables = CreateList<IDockable>(
                    pagePropertiesViewModel,
                    projectOptionsViewModel,
                    rendererOptionsViewModel,
                    zoomOptionsViewModel,
                    imageOptionsViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var rightTopToolDock = new ToolDock
            {
                Proportion = 0.35,
                ActiveDockable = shapePropertiesViewModel,
                VisibleDockables = CreateList<IDockable>(
                    shapePropertiesViewModel,
                    dataPropertiesViewModel,
                    statePropertiesViewModel,
                    templateLibraryViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var rightBottomToolDock = new ToolDock
            {
                ActiveDockable = stylePropertiesViewModel,
                VisibleDockables = CreateList<IDockable>(
                    stylePropertiesViewModel,
                    styleLibraryViewModel,
                    groupLibraryViewModel,
                    databaseLibraryViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var leftDock = new ProportionalDock
            {
                Proportion = 0.20,
                Orientation = Orientation.Vertical,
                ActiveDockable = leftTopToolDock,
                VisibleDockables = CreateList<IDockable>
                (
                    leftTopToolDock,
                    new SplitterDockable(),
                    leftBottomToolDock
                )
            };

            var rightDock = new ProportionalDock
            {
                Proportion = 0.20,
                Orientation = Orientation.Vertical,
                ActiveDockable = rightTopToolDock,
                VisibleDockables = CreateList<IDockable>
                (
                    rightTopToolDock,
                    new SplitterDockable(),
                    rightBottomToolDock
                )
            };

            var documentDock = new PageDocumentDock
            {
                Id = "PageDocumentDock",
                Title = "Pages",
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>(),
                CanCreateDocument = true,
                IsCollapsable = false
            };

            var homeDock = new ProportionalDock
            {
                Id = "HomeDock",
                Orientation = Orientation.Horizontal,
                VisibleDockables = CreateList<IDockable>
                (
                    leftDock,
                    new SplitterDockable(),
                    documentDock,
                    new SplitterDockable(),
                    rightDock
                ),
                IsCollapsable = false
            };

            var homeMenuViewModel = new HomeMenuViewModel()
            {
                Id = "HomeMenuView",
                Dock = DockMode.Top
            };
 
            var homeStatusBarViewModel = new HomeStatusBarViewModel()
            {
                Id = "HomeStatusBarView",
                Dock = DockMode.Bottom
            };

            var homeViewModel = new HomeViewModel
            {
                Id = "HomeView",
                Dock = DockMode.Center,
                ActiveDockable = homeDock,
                VisibleDockables = CreateList<IDockable>(homeDock),
                IsCollapsable = false
            };

            var homeDockDock = new DockDock()
            {
                LastChildFill = true,
                VisibleDockables = CreateList<IDockable>
                (
                    homeMenuViewModel,
                    homeStatusBarViewModel,
                    homeViewModel
                ),
                IsCollapsable = false
            };

            // Dashboard Perspective

            var dashboardMenuViewModel = new DashboardMenuViewModel()
            {
                Id = "DashboardMenuView",
                Title = "Dashboard Menu",
                Dock = DockMode.Top,
                IsCollapsable = false
            };

            var dashboardViewModel = new DashboardViewModel
            {
                Id = "DashboardView",
                Title = "Dashboard",
                Dock = DockMode.Center,
                IsCollapsable = false
            };

            var dashboardDockDock = new DockDock()
            {
                Id = "DashboardDock",
                Proportion = 1.0,
                LastChildFill = true,
                VisibleDockables = CreateList<IDockable>
                (
                    dashboardMenuViewModel,
                    dashboardViewModel
                ),
                IsCollapsable = false
            };

            // Root Perspective

            var dashboardRootDock = CreateRootDock();
            dashboardRootDock.Id = "Dashboard";
            dashboardRootDock.ActiveDockable = dashboardDockDock;
            dashboardRootDock.DefaultDockable = dashboardDockDock;
            dashboardRootDock.VisibleDockables = CreateList<IDockable>(dashboardDockDock);
            dashboardRootDock.IsCollapsable = false;

            var homeRootDock = CreateRootDock();
            homeRootDock.Id = "Home";
            homeRootDock.ActiveDockable = homeDockDock;
            homeRootDock.DefaultDockable = homeDockDock;
            homeRootDock.VisibleDockables = CreateList<IDockable>(homeDockDock);
            homeRootDock.IsCollapsable = false;

            // Root Dock

            var rootDock = CreateRootDock();
            rootDock.Id = "Root";
            rootDock.ActiveDockable = dashboardRootDock;
            rootDock.DefaultDockable = dashboardRootDock;
            rootDock.VisibleDockables = CreateList<IDockable>(dashboardRootDock, homeRootDock);
            rootDock.IsCollapsable = false;

            _rootDock = rootDock;
            _pagesDock = documentDock;
            _homeDock = homeDock;

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                // Documents
                ["PageDocument"] = () => _projectEditor,
                ["PageDocumentDock"] = () => _projectEditor,
                // Explorers
                ["ProjectExplorer"] = () => _projectEditor,
                ["ObjectBrowser"] = () => _projectEditor,
                // Properties
                ["PageProperties"] = () => _projectEditor,
                ["ShapeProperties"] = () => _projectEditor,
                ["StyleProperties"] = () => _projectEditor,
                ["DataProperties"] = () => _projectEditor,
                ["StateProperties"] = () => _projectEditor,
                // Libraries
                ["StyleLibrary"] = () => _projectEditor,
                ["GroupLibrary"] = () => _projectEditor,
                ["DatabaseLibrary"] = () => _projectEditor,
                ["TemplateLibrary"] = () => _projectEditor,
                ["ScriptLibrary"] = () => _projectEditor,
                // Options
                ["ProjectOptions"] = () => _projectEditor,
                ["RendererOptions"] = () => _projectEditor,
                ["ZoomOptions"] = () => _projectEditor,
                ["ImageOptions"] = () => _projectEditor,
                // Home
                ["HomeMenuView"] = () => _projectEditor,
                ["HomeView"] = () => _projectEditor,
                ["HomeDock"] = () => _projectEditor,
                ["HomeStatusBarView"] = () => _projectEditor,
                // Dashboard
                ["DashboardMenuView"] = () => _projectEditor,
                ["DashboardView"] = () => _projectEditor,
                ["DashboardDock"] = () => _projectEditor
            };

            DockableLocator = new Dictionary<string, Func<IDockable?>>
            {
                ["Root"] = () => _rootDock,
                ["Pages"] = () => _pagesDock,
                ["Home"] = () => _homeDock,
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
