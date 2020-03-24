using System.IO;
using static System.Console;

var document = Editor.Project.CurrentDocument;
var dir = "D:\\";

foreach (var page in document.Pages)
{
    foreach (var writer in Editor.FileWriters)
    {
        try
        {
            var path = Path.Combine(dir, page.Name + "." + writer.Extension);
            using var stream = Editor.FileIO.Create(path);
            writer.Save(stream, page, Editor.Project);
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
            WriteLine(ex.StackTrace);
        }
    }
}
