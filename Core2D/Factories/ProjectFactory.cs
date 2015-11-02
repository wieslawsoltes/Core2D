// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Factory used to create new projects, documents and containers.
    /// </summary>
    public class ProjectFactory : IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StyleLibrary"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="StyleLibrary"/>.</returns>
        public static StyleLibrary DefaultStyleLibrary()
        {
            var sgd = StyleLibrary.Create("Default");

            var builder = sgd.Styles.ToBuilder();
            builder.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 80, 0, 0, 0, 2.0));
            builder.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 80, 255, 255, 0, 2.0));
            builder.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 80, 255, 0, 0, 2.0));
            builder.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 80, 0, 255, 0, 2.0));
            builder.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 80, 0, 0, 255, 2.0));
            sgd.Styles = builder.ToImmutable();

            sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

            return sgd;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StyleLibrary"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="StyleLibrary"/>.</returns>
        public static StyleLibrary LinesStyleLibrary()
        {
            var sgdl = StyleLibrary.Create("Lines");

            var solid = ShapeStyle.Create("Solid", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            solid.Dashes = default(string);
            solid.DashOffset = 0.0;

            var dash = ShapeStyle.Create("Dash", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dash.Dashes = "2 2";
            dash.DashOffset = 1.0;

            var dot = ShapeStyle.Create("Dot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dot.Dashes = "0 2";
            dot.DashOffset = 0.0;

            var dashDot = ShapeStyle.Create("DashDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDot.Dashes = "2 2 0 2";
            dashDot.DashOffset = 1.0;

            var dashDotDot = ShapeStyle.Create("DashDotDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDotDot.Dashes = "2 2 0 2 0 2";
            dashDotDot.DashOffset = 1.0;

            var builder = sgdl.Styles.ToBuilder();
            builder.Add(solid);
            builder.Add(dash);
            builder.Add(dot);
            builder.Add(dashDot);
            builder.Add(dashDotDot);
            sgdl.Styles = builder.ToImmutable();

            sgdl.CurrentStyle = sgdl.Styles.FirstOrDefault();

            return sgdl;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StyleLibrary"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="StyleLibrary"/>.</returns>
        public static StyleLibrary TemplateStyleLibrary()
        {
            var sgt = StyleLibrary.Create("Template");
            var gs = ShapeStyle.Create("Grid", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);

            var builder = sgt.Styles.ToBuilder();
            builder.Add(gs);
            sgt.Styles = builder.ToImmutable();

            sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

            return sgt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="Container"/>.</returns>
        private Container CreateGridTemplate(Project project, string name)
        {
            var template = GetTemplate(project, name);

            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var layer = template.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = XRectangle.Create(
                0, 0,
                template.Width, template.Height,
                style,
                project.Options.PointShape);
            grid.IsStroked = false;
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

        /// <summary>
        /// Creates a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="Container"/>.</returns>
        public Container GetTemplate(Project project, string name)
        {
            var container = Container.Create(name, true);

            container.Background = ArgbColor.Create(0xFF, 0xFF, 0xFF, 0xFF);

            foreach (var layer in container.Layers)
            {
                layer.Name = string.Concat("Template", layer.Name);
            }

            return container;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="Container"/>.</returns>
        public Container GetContainer(Project project, string name)
        {
            var container = Container.Create(name);

            if (project.CurrentTemplate == null)
            {
                var template = GetTemplate(project, "Empty");
                var templateBuilder = project.Templates.ToBuilder();
                templateBuilder.Add(template);
                project.Templates = templateBuilder.ToImmutable();
                project.CurrentTemplate = template;
            }

            container.Template = project.CurrentTemplate;

            return container;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="Document"/>.</returns>
        public Document GetDocument(Project project, string name)
        {
            var document = Document.Create(name);
            return document;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="Project"/>.</returns>
        public Project GetProject()
        {
            var project = Project.Create();

            var glBuilder = project.GroupLibraries.ToBuilder();
            glBuilder.Add(GroupLibrary.Create("Default"));
            project.GroupLibraries = glBuilder.ToImmutable();

            var sgBuilder = project.StyleLibraries.ToBuilder();
            sgBuilder.Add(DefaultStyleLibrary());
            sgBuilder.Add(LinesStyleLibrary());
            sgBuilder.Add(TemplateStyleLibrary());
            project.StyleLibraries = sgBuilder.ToImmutable();

            project.CurrentGroupLibrary = project.GroupLibraries.FirstOrDefault();
            project.CurrentStyleLibrary = project.StyleLibraries.FirstOrDefault();

            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(GetTemplate(project, "Empty"));
            templateBuilder.Add(CreateGridTemplate(project, "Grid"));
            project.Templates = templateBuilder.ToImmutable();

            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "Grid");

            var document = GetDocument(project, "Document");
            var container = GetContainer(project, "Container");

            var containerBuilder = document.Containers.ToBuilder();
            containerBuilder.Add(container);
            document.Containers = containerBuilder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document);
            project.Documents = documentBuilder.ToImmutable();

            project.Selected = document.Containers.FirstOrDefault();

            return project;
        }
    }
}
