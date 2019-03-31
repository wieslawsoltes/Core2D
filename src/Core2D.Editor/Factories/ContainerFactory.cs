// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Factories
{
    /// <summary>
    /// Factory used to create containers.
    /// </summary>
    public sealed class ContainerFactory : IContainerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="ContainerFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ContainerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{IShapeStyle}"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ILibrary{IShapeStyle}"/>.</returns>
        public ILibrary<IShapeStyle> DefaultStyleLibrary()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var sgd = factory.CreateLibrary<IShapeStyle>("Default");

            var builder = sgd.Items.ToBuilder();
            builder.Add(factory.CreateShapeStyle("Black", 255, 0, 0, 0, 80, 0, 0, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Red", 255, 255, 0, 0, 80, 255, 0, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Green", 255, 0, 255, 0, 80, 0, 255, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Blue", 255, 0, 0, 255, 80, 0, 0, 255, 2.0));
            builder.Add(factory.CreateShapeStyle("Cyan", 255, 0, 255, 255, 80, 0, 255, 255, 2.0));
            builder.Add(factory.CreateShapeStyle("Magenta", 255, 255, 0, 255, 80, 255, 0, 255, 2.0));
            builder.Add(factory.CreateShapeStyle("Yellow", 255, 255, 255, 0, 80, 255, 255, 0, 2.0));
            sgd.Items = builder.ToImmutable();

            sgd.Selected = sgd.Items.FirstOrDefault();

            return sgd;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{ShapeStyle}"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ILibrary{ShapeStyle}"/>.</returns>
        public ILibrary<IShapeStyle> LinesStyleLibrary()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var sgdl = factory.CreateLibrary<IShapeStyle>("Lines");

            var solid = factory.CreateShapeStyle("Solid", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            solid.Dashes = default;
            solid.DashOffset = 0.0;

            var dash = factory.CreateShapeStyle("Dash", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dash.Dashes = "2 2";
            dash.DashOffset = 1.0;

            var dot = factory.CreateShapeStyle("Dot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dot.Dashes = "0 2";
            dot.DashOffset = 0.0;

            var dashDot = factory.CreateShapeStyle("DashDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDot.Dashes = "2 2 0 2";
            dashDot.DashOffset = 1.0;

            var dashDotDot = factory.CreateShapeStyle("DashDotDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDotDot.Dashes = "2 2 0 2 0 2";
            dashDotDot.DashOffset = 1.0;

            var builder = sgdl.Items.ToBuilder();
            builder.Add(solid);
            builder.Add(dash);
            builder.Add(dot);
            builder.Add(dashDot);
            builder.Add(dashDotDot);
            sgdl.Items = builder.ToImmutable();

            sgdl.Selected = sgdl.Items.FirstOrDefault();

            return sgdl;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{ShapeStyle}"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ILibrary{ShapeStyle}"/>.</returns>
        public ILibrary<IShapeStyle> TemplateStyleLibrary()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var sgt = factory.CreateLibrary<IShapeStyle>("Template");
            var gs = factory.CreateShapeStyle("Grid", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);

            var builder = sgt.Items.ToBuilder();
            builder.Add(gs);
            sgt.Items = builder.ToImmutable();

            sgt.Selected = sgt.Items.FirstOrDefault();

            return sgt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/> class.
        /// </summary>
        /// <param name="containerFactory">The container factory.</param>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        private IPageContainer CreateGridTemplate(IContainerFactory containerFactory, IProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var template = containerFactory.GetTemplate(project, name);

            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Items.FirstOrDefault(s => s.Name == "Grid");
            var layer = template.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = factory.CreateRectangleShape(
                0, 0,
                template.Width, template.Height,
                style,
                project.Options.PointShape);
            grid.IsStroked = true;
            grid.IsFilled = false;
            grid.IsGrid = true;
            grid.OffsetX = 30.0;
            grid.OffsetY = 30.0;
            grid.CellWidth = 30.0;
            grid.CellHeight = 30.0;
            grid.State.Flags &= ~ShapeStateFlags.Printable;
            builder.Add(grid);
            layer.Shapes = builder.ToImmutable();

            return template;
        }

        /// <inheritdoc/>
        IPageContainer IContainerFactory.GetTemplate(IProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var template = factory.CreateTemplateContainer(name);
            template.Background = factory.CreateArgbColor(0xFF, 0xFF, 0xFF, 0xFF);
            return template;
        }

        /// <inheritdoc/>
        IPageContainer IContainerFactory.GetPage(IProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var container = factory.CreatePageContainer(name);
            container.Template = project.CurrentTemplate ?? (this as IContainerFactory).GetTemplate(project, "Empty");
            return container;
        }

        /// <inheritdoc/>
        IDocumentContainer IContainerFactory.GetDocument(IProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var document = factory.CreateDocumentContainer(name);
            return document;
        }

        /// <inheritdoc/>
        IProjectContainer IContainerFactory.GetProject()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var containerFactory = this as IContainerFactory;
            var project = factory.CreateProjectContainer();

            // Group Libraries
            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(factory.CreateLibrary<IGroupShape>("Default"));
            project.GroupLibraries = glBuilder.ToImmutable();

            project.SetCurrentGroupLibrary(project.GroupLibraries.FirstOrDefault());

            // Style Libraries
            var sgBuilder = project.StyleLibraries.ToBuilder();
            sgBuilder.Add(DefaultStyleLibrary());
            sgBuilder.Add(LinesStyleLibrary());
            sgBuilder.Add(TemplateStyleLibrary());
            project.StyleLibraries = sgBuilder.ToImmutable();

            project.SetCurrentStyleLibrary(project.StyleLibraries.FirstOrDefault());

            // Templates
            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(containerFactory.GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateGridTemplate(this, project, "Grid"));
            project.Templates = templateBuilder.ToImmutable();

            project.SetCurrentTemplate(project.Templates.FirstOrDefault(t => t.Name == "Grid"));

            // Documents and Pages
            var document = containerFactory.GetDocument(project, "Document");
            var page = containerFactory.GetPage(project, "Page");

            var pageBuilder = document.Pages.ToBuilder();
            pageBuilder.Add(page);
            document.Pages = pageBuilder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document);
            project.Documents = documentBuilder.ToImmutable();

            project.Selected = document.Pages.FirstOrDefault();

            // Databases
            var db = factory.CreateDatabase("Db");
            var columnsBuilder = db.Columns.ToBuilder();
            columnsBuilder.Add(factory.CreateColumn(db, "Column0"));
            columnsBuilder.Add(factory.CreateColumn(db, "Column1"));
            db.Columns = columnsBuilder.ToImmutable();
            project.Databases = project.Databases.Add(db);

            project.SetCurrentDatabase(db);

            return project;
        }
    }
}
