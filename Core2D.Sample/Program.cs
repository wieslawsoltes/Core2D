// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dependencies;

namespace Core2D.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateProjectUsingContext();
            CreateProjectUsingEditor();
        }
        
        static void CreateProjectUsingContext()
        {
            var context = new EditorContext()
            {
                ProjectFactory = new ProjectFactory(),
                Serializer = new NewtonsoftSerializer()
            };

            var project = context.ProjectFactory.GetProject();
            context.Editor = Editor.Create(project);

            var factory = new ShapeFactory(context.Editor);
            factory.Line(30, 30, 60, 30);
            factory.Text(30, 30, 60, 60, "Sample1");

            context.Save("sample1.project");
        }
        
        static void CreateProjectUsingEditor()
        {
            var project = new ProjectFactory().GetProject();
            var editor = Editor.Create(project);

            var factory = new ShapeFactory(editor);
            factory.Line(30, 30, 60, 30);
            factory.Text(30, 30, 60, 60, "Sample2");
            
            Project.Save(project, "sample2.project", new NewtonsoftSerializer());
        }
    }
}
