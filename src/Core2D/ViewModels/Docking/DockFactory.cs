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
            // Left Dock

            var projectViewModel = new ProjectViewModel()
            {
                Id = "ProjectExplorer",
                Title = "Project Explorer"
            };

            var projectDock = new ToolDock
            {
                Id = "ProjectDock",
                Title = "Project",
                ActiveDockable = projectViewModel,
                VisibleDockables = CreateList<IDockable>(projectViewModel),
                Alignment = Alignment.Left,
                GripMode = GripMode.Visible
            };

            var leftProportionalDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    projectDock
                )
            };

            // Properties Dock

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

            // Libraries Dock

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

            // Options Dock

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

            var browserOptionsViewModel = new BrowserOptionsViewModel()
            {
                Id = "BrowserOptions",
                Title = "Browser"
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
                    browserOptionsViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            // Right Dock

            var propertiesDock = new ToolDock
            {
                Id = "PropertiesDock",
                Title = "Properties",
                ActiveDockable = propertiesToolDock,
                VisibleDockables = CreateList<IDockable>(
                    propertiesToolDock,
                    librariesToolDock,
                    optionsToolDock),
                Alignment = Alignment.Right,
                GripMode = GripMode.Visible
            };

            var rightProportionalDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    propertiesDock
                )
            };

            // Document Dock

            var documentDock = new PageDocumentDock
            {
                Id = "PagesDock",
                Title = "Pages",
                IsCollapsable = false,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>(),
                CanCreateDocument = true
            };

            // Home

            var homeLayout = new ProportionalDock
            {
                Orientation = Orientation.Horizontal,
                VisibleDockables = CreateList<IDockable>
                (
                    leftProportionalDock,
                    new SplitterDockable(),
                    documentDock,
                    new SplitterDockable(),
                    rightProportionalDock
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

            // Dashboard

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

            // Root

            var rootDock = CreateRootDock();
            rootDock.Id = "Root";
            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = dashboardRootDock;
            rootDock.DefaultDockable = dashboardRootDock;
            rootDock.VisibleDockables = CreateList<IDockable>(dashboardRootDock, homeRootDock);

            _pagesDock = documentDock;
            _rootDock = rootDock;

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                ["PageDocument"] = () => _projectEditor,
                ["ProjectExplorer"] = () => _projectEditor,
                ["ProjectDock"] = () => _projectEditor,
                ["PageProperties"] = () => _projectEditor,
                ["ShapeProperties"] = () => _projectEditor,
                ["StyleProperties"] = () => _projectEditor,
                ["DataProperties"] = () => _projectEditor,
                ["StateProperties"] = () => _projectEditor,
                ["PropertiesToolDock"] = () => _projectEditor,
                ["StyleLibrary"] = () => _projectEditor,
                ["GroupLibrary"] = () => _projectEditor,
                ["DatabaseLibrary"] = () => _projectEditor,
                ["TemplateLibrary"] = () => _projectEditor,
                ["ScriptLibrary"] = () => _projectEditor,
                ["LibrariesToolDock"] = () => _projectEditor,
                ["ProjectOptions"] = () => _projectEditor,
                ["RendererOptions"] = () => _projectEditor,
                ["ZoomOptions"] = () => _projectEditor,
                ["ImageOptions"] = () => _projectEditor,
                ["BrowserOptions"] = () => _projectEditor,
                ["OptionsToolDock"] = () => _projectEditor,
                ["PropertiesDock"] = () => _projectEditor,
                ["PagesDock"] = () => _projectEditor,
                ["HomeMenuView"] = () => _projectEditor,
                ["HomeView"] = () => _projectEditor,
                ["HomeStatusBarView"] = () => _projectEditor,
                ["HomeDock"] = () => _projectEditor,
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
