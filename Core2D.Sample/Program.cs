// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
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
            CreateSampleProject();
        }

        static EditorContext CreateContext()
        {
            var context = new EditorContext()
            {
                View = null,
                Renderers = null,
                ProjectFactory = new ProjectFactory(),
                TextClipboard = null,
                Serializer = new NewtonsoftSerializer(),
                PdfWriter = null,
                DxfWriter = null,
                CsvReader = null,
                CsvWriter = null
            };

            var project = context.ProjectFactory.GetProject();

            context.Editor = Editor.Create(project);

            return context;
        }

        static void CreateSampleProject()
        {
            try
            {
                var context = CreateContext();

                var factory = new ShapeFactory(context);

                factory.Line(30, 30, 60, 30);
                factory.Text(30, 30, 60, 60, "Sample");

                context.Save("sample.project");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey(true);
            }
        }
    }
}
