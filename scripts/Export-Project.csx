using System.IO;

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
            var path = Path.Combine(dir, page.Name + "." + writer.Extension);
            writer.Save(path, page, Editor.Project);
        }
    }
}
