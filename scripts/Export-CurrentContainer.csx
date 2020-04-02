using System.IO;
using static System.Console;

var page = Project.CurrentContainer;
var dir = "D:\\";

foreach (var writer in FileWriters)
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
