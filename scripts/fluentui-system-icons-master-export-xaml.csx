
#r "Core2D.Modules"
using System.IO;
using System.Linq;
using static System.Console;
using Core2D.FileWriter.Xaml;

var project = Project;
var output = @"D:\Xaml";

var writer = FileWriters.FirstOrDefault(x => x.GetType() == typeof(DrawingGroupXamlWriter));

foreach (var document in project.Documents)
{
    var dir = Path.Combine(output, document.Name);

    Directory.CreateDirectory(dir);

    foreach (var page in document.Pages)
    {
        try
        {
            var path = Path.Combine(dir, page.Name + "." + writer.Extension);
            using (var stream = FileIO.Create(path))
            {
                writer.Save(stream, page, Project);
            }
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
            WriteLine(ex.StackTrace);
        }
    }
}
