// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dependencies;

namespace Core2D.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateProjectUsingContext();
            CreateProjectUsingEditor();
            CreateProjectUsingEditorWithoutFactories();
        }
        
        static void CreateProjectUsingContext()
        {
            var context = new EditorContext()
            {
                ProjectFactory = new ProjectFactory(),
                Serializer = new NewtonsoftSerializer()
            };

            var project = context.ProjectFactory.GetProject();
            context.Editor = Editor.Create(project, null, false, false);

            var factory = new ShapeFactory(context.Editor);
            factory.Line(30, 30, 60, 30);
            factory.Text(30, 30, 60, 60, "Sample1");

            context.Save("sample1.project");
        }
        
        static void CreateProjectUsingEditor()
        {
            var project = new ProjectFactory().GetProject();
            var editor = Editor.Create(project, null, false, false);

            var factory = new ShapeFactory(editor);
            factory.Line(30, 30, 60, 30);
            factory.Text(30, 30, 60, 60, "Sample2");
            
            Project.Save(project, "sample2.project", new NewtonsoftSerializer());
        }

        static void CreateProjectUsingEditorWithoutFactories()
        {
            var project = Project.Create();
            var editor = Editor.Create(project, null, false, false);

            var document = Document.Create();
            editor.AddDocument(document);
            project.CurrentDocument = document;

            var container = Container.Create();
            container.Template = Container.Create(isTemplate: true);
            editor.AddContainer(container);
            project.CurrentContainer = container;

            var layer = Layer.Create(owner: container);
            editor.AddLayer(layer);
            project.CurrentContainer.CurrentLayer = layer;

            editor.AddStyleLibrary();
            project.CurrentStyleLibrary = project.StyleLibraries.FirstOrDefault();

            editor.AddStyle();
            project.CurrentStyleLibrary.Selected = project.CurrentStyleLibrary.Items.FirstOrDefault();

            var line = XLine.Create(30, 30, 60, 30, project.CurrentStyleLibrary.Selected, project.Options.PointShape);
            editor.AddShape(line);
            var text = XText.Create(30, 30, 60, 60, project.CurrentStyleLibrary.Selected, project.Options.PointShape, "Sample3");
            editor.AddShape(text);

            Project.Save(project, "sample3.project", new NewtonsoftSerializer());
        }
    }
}
