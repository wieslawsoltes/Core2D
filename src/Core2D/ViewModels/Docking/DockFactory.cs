using System;
using System.Collections.Generic;
using Core2D.ViewModels.Docking.Docks;
using Core2D.ViewModels.Docking.Tools;
using Core2D.ViewModels.Docking.Tools.Libraries;
using Core2D.ViewModels.Docking.Tools.Options;
using Core2D.ViewModels.Docking.Tools.Properties;
using Core2D.ViewModels.Docking.Views;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;

namespace Core2D.ViewModels.Docking
{
    public class DockFactory : Factory
    {
        public override IDocumentDock CreateDocumentDock()
        {
            return new PageDocumentDock();
        }

        public override IRootDock CreateLayout()
        {
            // Left Dock

            var projectViewModel = new ProjectViewModel()
            {
                Id = "Project",
                Title = "Project"
            };

            var leftProportionalDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ToolDock
                    {
                        ActiveDockable = projectViewModel,
                        VisibleDockables = CreateList<IDockable>(projectViewModel),
                        Alignment = Alignment.Left,
                        GripMode = GripMode.Hidden
                    }
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
                ActiveDockable = pagePropertiesViewModel,
                VisibleDockables = CreateList<IDockable>(
                    pagePropertiesViewModel, 
                    shapePropertiesViewModel,
                    stylePropertiesViewModel,
                    dataPropertiesViewModel,
                    statePropertiesViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Hidden
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
                ActiveDockable = styleLibraryViewModel,
                VisibleDockables = CreateList<IDockable>(
                    styleLibraryViewModel, 
                    groupLibraryViewModel,
                    databaseLibraryViewModel,
                    templateLibraryViewModel,
                    scriptLibraryViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Hidden
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
                ActiveDockable = styleLibraryViewModel,
                VisibleDockables = CreateList<IDockable>(
                    projectOptionsViewModel, 
                    rendererOptionsViewModel,
                    zoomOptionsViewModel,
                    imageOptionsViewModel,
                    browserOptionsViewModel),
                Alignment = Alignment.Right,
                GripMode = GripMode.Hidden
            };

            // Right Dock

            var rightProportionalDock = new ProportionalDock
            {
                Proportion = 0.25,
                Orientation = Orientation.Vertical,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
                (
                    new ToolDock
                    {
                        ActiveDockable = propertiesToolDock,
                        VisibleDockables = CreateList<IDockable>(
                            propertiesToolDock, 
                            librariesToolDock,
                            optionsToolDock),
                        Alignment = Alignment.Right,
                        GripMode = GripMode.Hidden
                    }
                )
            };

            // Document Dock

            var documentDock = new PageDocumentDock
            {
                IsCollapsable = false,
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>(),
                CanCreateDocument = true
            };
            
            // Home

            var homeLayout = new ProportionalDock
            {
                Proportion = 1,
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
                Proportion = 0
            };

            var homeViewModel = new HomeViewModel
            {
                Id = "HomeView",
                Title = "Home",
                Proportion = 1,
                ActiveDockable = homeLayout,
                VisibleDockables = CreateList<IDockable>(homeLayout)
            };

            var homeStatusBarViewModel = new HomeStatusBarViewModel()
            {
                Id = "HomeStatusBarView",
                Title = "Home StatusBar",
                Proportion = 0
            };

            var homeDock = new ProportionalDock
            {
                Id = "HomeDock",
                Proportion = 1,
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>
                (                    
                    homeMenuViewModel,
                    new SplitterDockable(),
                    homeViewModel,
                    new SplitterDockable(),
                    homeStatusBarViewModel
                )
            };
            
            // Dashboard
            
            var dashboardMenuViewModel = new DashboardMenuViewModel()
            {
                Id = "DashboardMenuView",
                Title = "Dashboard Menu",
                Proportion = 0.1
            };

            var dashboardViewModel = new DashboardViewModel
            {
                Id = "DashboardView",
                Title = "Dashboard",
                Proportion = 0.9
            };

            var dashboardDock = new ProportionalDock
            {
                Id = "DashboardDock",
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>
                (                    
                    dashboardMenuViewModel,
                    new SplitterDockable(),
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
            homeRootDock.Id = "Root";
            rootDock.IsCollapsable = false;
            rootDock.ActiveDockable = dashboardRootDock;
            rootDock.DefaultDockable = dashboardRootDock;
            rootDock.VisibleDockables = CreateList<IDockable>(dashboardRootDock, homeRootDock);

            return rootDock;
        }

        public override void InitLayout(IDockable layout)
        {
            ContextLocator = new Dictionary<string, Func<object>>
            {
                // TODO:
            };

            DockableLocator = new Dictionary<string, Func<IDockable?>>()
            {
                // TODO:
            };

            HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
