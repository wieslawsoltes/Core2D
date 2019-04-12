using System.IO;

var document = Editor.Project.CurrentDocument;
var dir = "D:\\";

foreach (var page in document.Pages)
{
    foreach (var writer in Editor.FileWriters)
    {
        path = Path.Combine(dir, page.Name + "." + writer.Extension);
        writer.Save(path, page, Editor.Project);
    }
}
