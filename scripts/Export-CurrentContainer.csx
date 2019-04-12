using System.IO;

var page = Editor.Project.CurrentContainer;
var dir = "D:\\";

foreach (var writer in Editor.FileWriters)
{
    var path = Path.Combine(dir, page.Name + "." + writer.Extension);
    writer.Save(path, page, Editor.Project);
}
