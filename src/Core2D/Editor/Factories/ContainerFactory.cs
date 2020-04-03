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

        private ILibrary<IShapeStyle> DefaultStyleLibrary()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var sgd = factory.CreateLibrary<IShapeStyle>("Default");

            var builder = sgd.Items.ToBuilder();

            builder.Add(factory.CreateShapeStyle("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            builder.Add(factory.CreateShapeStyle("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));

            var dash = factory.CreateShapeStyle("Dash", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dash.Dashes = "2 2";
            dash.DashOffset = 1.0;
            builder.Add(dash);

            var dot = factory.CreateShapeStyle("Dot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dot.Dashes = "0 2";
            dot.DashOffset = 0.0;
            builder.Add(dot);

            var dashDot = factory.CreateShapeStyle("DashDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDot.Dashes = "2 2 0 2";
            dashDot.DashOffset = 1.0;
            builder.Add(dashDot);

            var dashDotDot = factory.CreateShapeStyle("DashDotDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDotDot.Dashes = "2 2 0 2 0 2";
            dashDotDot.DashOffset = 1.0;
            builder.Add(dashDotDot);

            sgd.Items = builder.ToImmutable();
            sgd.Selected = sgd.Items.FirstOrDefault();

            return sgd;
        }

        private IPageContainer CreateDefaultTemplate(IContainerFactory containerFactory, IProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var template = containerFactory.GetTemplate(project, name);

            var style = factory.CreateShapeStyle("Default", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);
            var layer = template.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = factory.CreateRectangleShape(
                30.0, 30.0,
                template.Width - 30.0, template.Height - 30.0,
                style);
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
            var project = factory.CreateProjectContainer("Project1");

            // Group Libraries
            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(factory.CreateLibrary<IGroupShape>("Default"));
            project.GroupLibraries = glBuilder.ToImmutable();

            project.SetCurrentGroupLibrary(project.GroupLibraries.FirstOrDefault());

            // Style Libraries
            var sgBuilder = project.StyleLibraries.ToBuilder();
            sgBuilder.Add(DefaultStyleLibrary());
            project.StyleLibraries = sgBuilder.ToImmutable();

            project.SetCurrentStyleLibrary(project.StyleLibraries.FirstOrDefault());

            // Templates
            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(containerFactory.GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateDefaultTemplate(this, project, "Default"));
            project.Templates = templateBuilder.ToImmutable();

            project.SetCurrentTemplate(project.Templates.FirstOrDefault(t => t.Name == "Default"));

            // Scripts
            var scriptBuilder = project.Scripts.ToBuilder();
            scriptBuilder.Add(factory.CreateScript());
            project.Scripts = scriptBuilder.ToImmutable();

            project.SetCurrentScript(project.Scripts.FirstOrDefault());

            // Documents and Pages
            var document = containerFactory.GetDocument(project, "Document1");
            var page = containerFactory.GetPage(project, "Page1");

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
