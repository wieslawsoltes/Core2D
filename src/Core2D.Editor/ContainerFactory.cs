// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Factory used to create containers.
    /// </summary>
    public sealed class ContainerFactory : IContainerFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{IShapeStyle}"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ILibrary{IShapeStyle}"/>.</returns>
        public static ILibrary<IShapeStyle> DefaultStyleLibrary()
        {
            var sgd = Factory.CreateLibrary<IShapeStyle>("Default");

            var builder = sgd.Items.ToBuilder();
            builder.Add(Factory.CreateShapeStyle("Black", 255, 0, 0, 0, 80, 0, 0, 0, 2.0));
            builder.Add(Factory.CreateShapeStyle("Red", 255, 255, 0, 0, 80, 255, 0, 0, 2.0));
            builder.Add(Factory.CreateShapeStyle("Green", 255, 0, 255, 0, 80, 0, 255, 0, 2.0));
            builder.Add(Factory.CreateShapeStyle("Blue", 255, 0, 0, 255, 80, 0, 0, 255, 2.0));
            builder.Add(Factory.CreateShapeStyle("Cyan", 255, 0, 255, 255, 80, 0, 255, 255, 2.0));
            builder.Add(Factory.CreateShapeStyle("Magenta", 255, 255, 0, 255, 80, 255, 0, 255, 2.0));
            builder.Add(Factory.CreateShapeStyle("Yellow", 255, 255, 255, 0, 80, 255, 255, 0, 2.0));
            sgd.Items = builder.ToImmutable();

            sgd.Selected = sgd.Items.FirstOrDefault();

            return sgd;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ILibrary{ShapeStyle}"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ILibrary{ShapeStyle}"/>.</returns>
        public static ILibrary<IShapeStyle> LinesStyleLibrary()
        {
            var sgdl = Factory.CreateLibrary<IShapeStyle>("Lines");

            var solid = Factory.CreateShapeStyle("Solid", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            solid.Dashes = default;
            solid.DashOffset = 0.0;

            var dash = Factory.CreateShapeStyle("Dash", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dash.Dashes = "2 2";
            dash.DashOffset = 1.0;

            var dot = Factory.CreateShapeStyle("Dot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dot.Dashes = "0 2";
            dot.DashOffset = 0.0;

            var dashDot = Factory.CreateShapeStyle("DashDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDot.Dashes = "2 2 0 2";
            dashDot.DashOffset = 1.0;

            var dashDotDot = Factory.CreateShapeStyle("DashDotDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
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
        public static ILibrary<IShapeStyle> TemplateStyleLibrary()
        {
            var sgt = Factory.CreateLibrary<IShapeStyle>("Template");
            var gs = Factory.CreateShapeStyle("Grid", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);

            var builder = sgt.Items.ToBuilder();
            builder.Add(gs);
            sgt.Items = builder.ToImmutable();

            sgt.Selected = sgt.Items.FirstOrDefault();

            return sgt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/> class.
        /// </summary>
        /// <param name="factory">The container factory.</param>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        private IPageContainer CreateGridTemplate(IContainerFactory factory, IProjectContainer project, string name)
        {
            var template = factory.GetTemplate(project, name);

            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Items.FirstOrDefault(s => s.Name == "Grid");
            var layer = template.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = Factory.CreateRectangleShape(
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
            var template = Factory.CreateTemplateContainer(name);
            template.Background = Factory.CreateArgbColor(0xFF, 0xFF, 0xFF, 0xFF);
            return template;
        }

        /// <inheritdoc/>
        IPageContainer IContainerFactory.GetPage(IProjectContainer project, string name)
        {
            var container = Factory.CreatePageContainer(name);
            container.Template = project.CurrentTemplate ?? (this as IContainerFactory).GetTemplate(project, "Empty");
            return container;
        }

        /// <inheritdoc/>
        IDocumentContainer IContainerFactory.GetDocument(IProjectContainer project, string name)
        {
            var document = Factory.CreateDocumentContainer(name);
            return document;
        }

        /// <inheritdoc/>
        IProjectContainer IContainerFactory.GetProject()
        {
            var factory = this as IContainerFactory;
            var project = Factory.CreateProjectContainer();

            // Group Libraries
            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(Factory.CreateLibrary<IGroupShape>("Default"));
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
            templateBuilder.Add(factory.GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateGridTemplate(this, project, "Grid"));
            project.Templates = templateBuilder.ToImmutable();

            project.SetCurrentTemplate(project.Templates.FirstOrDefault(t => t.Name == "Grid"));

            // Documents and Pages
            var document = factory.GetDocument(project, "Document");
            var page = factory.GetPage(project, "Page");

            var pageBuilder = document.Pages.ToBuilder();
            pageBuilder.Add(page);
            document.Pages = pageBuilder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document);
            project.Documents = documentBuilder.ToImmutable();

            project.Selected = document.Pages.FirstOrDefault();

            // Databases
            var db = Factory.CreateDatabase("Db");
            var columnsBuilder = db.Columns.ToBuilder();
            columnsBuilder.Add(Factory.CreateColumn(db, "Column0"));
            columnsBuilder.Add(Factory.CreateColumn(db, "Column1"));
            db.Columns = columnsBuilder.ToImmutable();
            project.Databases = project.Databases.Add(db);

            project.SetCurrentDatabase(db);

            return project;
        }
    }
}
