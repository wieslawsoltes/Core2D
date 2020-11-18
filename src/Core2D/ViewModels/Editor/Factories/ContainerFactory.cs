using System;
using System.Linq;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Factories
{
    public sealed class ContainerFactory : IContainerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ContainerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private Library<ShapeStyle> DefaultStyleLibrary()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var sgd = factory.CreateLibrary<ShapeStyle>("Default");

            var builder = sgd.Items.ToBuilder();

            builder.Add(factory.CreateShapeStyle("Solid"));

            sgd.Items = builder.ToImmutable();
            sgd.Selected = sgd.Items.FirstOrDefault();

            return sgd;
        }

        private PageContainer CreateDefaultTemplate(IContainerFactory containerFactory, ProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var template = containerFactory.GetTemplate(project, name);

            template.IsGridEnabled = false;
            template.IsBorderEnabled = false;
            template.GridOffsetLeft = 30.0;
            template.GridOffsetTop = 30.0;
            template.GridOffsetRight = -30.0;
            template.GridOffsetBottom = -30.0;
            template.GridCellWidth = 30.0;
            template.GridCellHeight = 30.0;
            template.GridStrokeColor = factory.CreateArgbColor(0xFF, 0xDE, 0xDE, 0xDE);
            template.GridStrokeThickness = 1.0;

            return template;
        }

        PageContainer IContainerFactory.GetTemplate(ProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var template = factory.CreateTemplateContainer(name);
            template.Background = factory.CreateArgbColor(0xFF, 0xFF, 0xFF, 0xFF);
            return template;
        }

        PageContainer IContainerFactory.GetPage(ProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var container = factory.CreatePageContainer(name);
            container.Template = project.CurrentTemplate ?? (this as IContainerFactory).GetTemplate(project, "Default");
            return container;
        }

        DocumentContainer IContainerFactory.GetDocument(ProjectContainer project, string name)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var document = factory.CreateDocumentContainer(name);
            return document;
        }

        ProjectContainer IContainerFactory.GetProject()
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var containerFactory = this as IContainerFactory;
            var project = factory.CreateProjectContainer("Project1");

            // Group Libraries
            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(factory.CreateLibrary<GroupShape>("Default"));
            project.GroupLibraries = glBuilder.ToImmutable();

            project.SetCurrentGroupLibrary(project.GroupLibraries.FirstOrDefault());

            // Style Libraries
            var sgBuilder = project.StyleLibraries.ToBuilder();
            sgBuilder.Add(DefaultStyleLibrary());
            project.StyleLibraries = sgBuilder.ToImmutable();

            project.SetCurrentStyleLibrary(project.StyleLibraries.FirstOrDefault());

            // Templates
            var templateBuilder = project.Templates.ToBuilder();
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
            var db = factory.CreateDatabase("Default");
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
