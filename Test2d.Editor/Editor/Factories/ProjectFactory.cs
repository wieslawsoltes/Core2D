// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectFactory : IProjectFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="container"></param>
        private void CreateGrid(Project project, Container container)
        {
            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var settings = LineGrid.Settings.Create(0, 0, container.Width, container.Height, 30, 30);
            var shapes = LineGrid.Create(style, settings, project.Options.PointShape);
            var layer = container.Layers.FirstOrDefault();

            var builder = layer.Shapes.ToBuilder();
            foreach (var shape in shapes)
            {
                builder.Add(shape);
            }
            layer.Shapes = builder.ToImmutable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Container CreateGridTemplate(Project project, string name)
        {
            var container = GetTemplate(project, name);

            CreateGrid(project, container);

            return container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Container GetTemplate(Project project, string name)
        {
            var container = Container.Create(name);

            container.Background = ArgbColor.Create(0xFF, 0xFF, 0xFF, 0xFF);

            foreach (var layer in container.Layers)
            {
                layer.Name = string.Concat("Template", layer.Name);
            }

            return container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Container GetContainer(Project project, string name)
        {
            var container = Container.Create(name);
            container.Template = project.CurrentTemplate;
            container.Width = container.Template.Width;
            container.Height = container.Template.Height;
            return container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Document GetDocument(Project project, string name)
        {
            var document = Document.Create(name);
            return document;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Project GetProject()
        {
            var project = Project.Create();

            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateGridTemplate(project, "Grid"));
            project.Templates = templateBuilder.ToImmutable();

            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "Grid");

            var document = GetDocument(project, "Dcoument");
            var container = GetContainer(project, "Container");

            var containerBuilder = document.Containers.ToBuilder();
            containerBuilder.Add(container);
            document.Containers = containerBuilder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document);
            project.Documents = documentBuilder.ToImmutable();

            project.CurrentDocument = project.Documents.FirstOrDefault();
            project.CurrentContainer = document.Containers.FirstOrDefault();

            return project;
        }
    }
}
