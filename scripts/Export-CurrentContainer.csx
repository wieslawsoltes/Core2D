using System.IO;
using static System.Console;

var page = Editor.Project.CurrentContainer;
var dir = "D:\\";

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
