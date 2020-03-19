using System.IO;
using static System.Console;

var project = Editor.Project;
var output = "D:\\";

foreach (var document in project.Documents)
{
    var dir = Path.Combine(output, document.Name);

    Directory.CreateDirectory(dir);

    foreach (var page in document.Pages)
    {
        foreach (var writer in Editor.FileWriters)
        {
            try
            {
                var path = Path.Combine(dir, page.Name + "." + writer.Extension);
                writer.Save(path, page, Editor.Project);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                WriteLine(ex.StackTrace);
            }
        }
    }
}
