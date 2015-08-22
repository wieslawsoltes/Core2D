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
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static StyleLibrary LinesStyleLibrary()
        {
            var sgdl = StyleLibrary.Create("Lines");

            var solid = ShapeStyle.Create("Solid", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            solid.Dashes = default(double[]);
            solid.DashOffset = 0.0;

            var dash = ShapeStyle.Create("Dash", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dash.Dashes = new double[] { 2, 2 };
            dash.DashOffset = 1.0;

            var dot = ShapeStyle.Create("Dot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dot.Dashes = new double[] { 0, 2 };
            dot.DashOffset = 0.0;

            var dashDot = ShapeStyle.Create("DashDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDot.Dashes = new double[] { 2, 2, 0, 2 };
            dashDot.DashOffset = 1.0;

            var dashDotDot = ShapeStyle.Create("DashDotDot", 255, 0, 0, 0, 80, 0, 0, 0, 2.0);
            dashDotDot.Dashes = new double[] { 2, 2, 0, 2, 0, 2 };
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="container"></param>
        private void CreateGrid(Project project, Container container)
        {
            var style = project
                .StyleLibraries.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var layer = container.Layers.FirstOrDefault();
            var builder = layer.Shapes.ToBuilder();
            var grid = XRectangle.Create(
                0, 0,
                container.Width, container.Height,
                style, 
                project.Options.PointShape);
            grid.IsStroked = false;
            grid.IsFilled = false;
            grid.IsGrid = true;
            grid.OffsetX = 30.0;
            grid.OffsetY = 30.0;
            grid.CellWidth = 30.0;
            grid.CellHeight = 30.0;
            grid.State &= ~ShapeState.Printable;
            builder.Add(grid);
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

            container.IsTemplate = true;
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

            if (project.CurrentTemplate == null)
            {
                var template = GetTemplate(project, "Empty");
                var templateBuilder = project.Templates.ToBuilder();
                templateBuilder.Add(template);
                project.Templates = templateBuilder.ToImmutable();
                project.CurrentTemplate = template;
            }
            
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

            project.CurrentDocument = project.Documents.FirstOrDefault();
            project.CurrentContainer = document.Containers.FirstOrDefault();

            return project;
        }
    }
}
