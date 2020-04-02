using System.IO;
using static System.Console;

var project = Project;
var output = "D:\\";

foreach (var document in project.Documents)
{
    var dir = Path.Combine(output, document.Name);

    Directory.CreateDirectory(dir);

    foreach (var page in document.Pages)
    {
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
    }
}
