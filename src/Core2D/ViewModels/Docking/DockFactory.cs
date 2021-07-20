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
        private IDocumentDock? _pagesDock;
        private IRootDock? _rootDock;

        public IDocumentDock? PagesDock => _pagesDock;

        public IRootDock? RootDock => _rootDock;

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
                Title = "Project"
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

            var propertiesToolDock = new ToolDock
            {
                Id = "PropertiesToolDock",
                Title = "Properties",
                ActiveDockable = pagePropertiesViewModel,
                VisibleDockables = CreateList<IDockable>(
                    pagePropertiesViewModel,
                    shapePropertiesViewModel,
                    stylePropertiesViewModel,
                    dataPropertiesViewModel,
                    statePropertiesViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var librariesToolDock = new ToolDock
            {
                Id = "LibrariesToolDock",
                Title = "Libraries",
                ActiveDockable = styleLibraryViewModel,
                VisibleDockables = CreateList<IDockable>(
                    styleLibraryViewModel,
                    groupLibraryViewModel,
                    databaseLibraryViewModel,
                    templateLibraryViewModel,
                    scriptLibraryViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var optionsToolDock = new ToolDock
            {
                Id = "OptionsToolDock",
                Title = "Options",
                ActiveDockable = styleLibraryViewModel,
                VisibleDockables = CreateList<IDockable>(
                    projectOptionsViewModel,
                    rendererOptionsViewModel,
                    zoomOptionsViewModel,
                    imageOptionsViewModel,
                    objectBrowserViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var projectToolDock = new ToolDock
            {
                Id = "ProjectToolDock",
                Title = "Project",
                ActiveDockable = projectExplorerViewModel,
                VisibleDockables = CreateList<IDockable>(projectExplorerViewModel),
                Alignment = Alignment.Left,
                GripMode = GripMode.Visible
            };

            var leftDock = new ProportionalDock
            {
                Proportion = 0.20,
                Orientation = Orientation.Vertical,
                ActiveDockable = projectToolDock,
                VisibleDockables = CreateList<IDockable>
                (
                    projectToolDock,
                    new SplitterDockable(),
                    optionsToolDock
                )
            };

            var rightDock = new ProportionalDock
            {
                Proportion = 0.20,
                Orientation = Orientation.Vertical,
                ActiveDockable = propertiesToolDock,
                VisibleDockables = CreateList<IDockable>
                (
                    propertiesToolDock,
                    new SplitterDockable(),
                    librariesToolDock
                )
            };

            var pageDocumentDock = new PageDocumentDock
            {
                Id = "PageDocumentDock",
                Title = "Pages",
                IsCollapsable = false,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>(),
                CanCreateDocument = true
            };

            var homeLayout = new ProportionalDock
            {
                Orientation = Orientation.Horizontal,
                VisibleDockables = CreateList<IDockable>
                (
                    leftDock,
                    new SplitterDockable(),
                    pageDocumentDock,
                    new SplitterDockable(),
                    rightDock
                )
            };

            var homeMenuViewModel = new HomeMenuViewModel()
            {
                Id = "HomeMenuView",
                Title = "Home Menu",
                Dock = DockMode.Top
            };

            var homeViewModel = new HomeViewModel
            {
                Id = "HomeView",
                Title = "Home",
                Dock = DockMode.Center,
                ActiveDockable = homeLayout,
                VisibleDockables = CreateList<IDockable>(homeLayout)
            };

            var homeStatusBarViewModel = new HomeStatusBarViewModel()
            {
                Id = "HomeStatusBarView",
                Title = "Home StatusBar",
                Dock = DockMode.Bottom
            };

            var homeDock = new DockDock()
            {
                Id = "HomeDock",
                LastChildFill = true,
                VisibleDockables = CreateList<IDockable>
                (
                    homeMenuViewModel,
                    homeStatusBarViewModel,
                    homeViewModel
                )
            };

            // Dashboard Perspective

            var dashboardMenuViewModel = new DashboardMenuViewModel()
            {
                Id = "DashboardMenuView",
                Title = "Dashboard Menu",
                Dock = DockMode.Top
            };

            var dashboardViewModel = new DashboardViewModel
            {
                Id = "DashboardView",
                Title = "Dashboard",
                Dock = DockMode.Center
            };

            var dashboardDock = new DockDock()
            {
                Id = "DashboardDock",
                Proportion = 1.0,
                LastChildFill = true,
                VisibleDockables = CreateList<IDockable>
                (
                    dashboardMenuViewModel,
                    dashboardViewModel
                )
            };

            // Root Perspective

            var dashboardRootDock = CreateRootDock();
            dashboardRootDock.Id = "Dashboard";
            dashboardRootDock.IsCollapsable = false;
            dashboardRootDock.ActiveDockable = dashboardDock;
            dashboardRootDock.DefaultDockable = dashboardDock;
            dashboardRootDock.VisibleDockables = CreateList<IDockable>(dashboardDock);

            var homeRootDock = CreateRootDock();
            homeRootDock.Id = "Home";
            homeRootDock.IsCollapsable = false;
            homeRootDock.ActiveDockable = homeDock;
            homeRootDock.DefaultDockable = homeDock;
            homeRootDock.VisibleDockables = CreateList<IDockable>(homeDock);

            // Root Dock

            var rootDock = CreateRootDock();
            rootDock.Id = "Root";
            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = dashboardRootDock;
            rootDock.DefaultDockable = dashboardRootDock;
            rootDock.VisibleDockables = CreateList<IDockable>(dashboardRootDock, homeRootDock);

            _pagesDock = pageDocumentDock;
            _rootDock = rootDock;

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
                // Docks
                ["ProjectToolDock"] = () => _projectEditor,
                ["PropertiesToolDock"] = () => _projectEditor,
                ["LibrariesToolDock"] = () => _projectEditor,
                ["OptionsToolDock"] = () => _projectEditor,
                // Home
                ["HomeMenuView"] = () => _projectEditor,
                ["HomeView"] = () => _projectEditor,
                ["HomeStatusBarView"] = () => _projectEditor,
                ["HomeDock"] = () => _projectEditor,
                // Dashboard
                ["DashboardMenuView"] = () => _projectEditor,
                ["DashboardView"] = () => _projectEditor,
                ["DashboardDock"] = () => _projectEditor
            };

            DockableLocator = new Dictionary<string, Func<IDockable?>>
            {
                ["Root"] = () => _rootDock,
                ["Pages"] = () => _pagesDock
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
