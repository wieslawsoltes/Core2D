
#r "Core2D.Modules"
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;
using Core2D;
using Core2D.FileWriter.Xaml;

var outputPath = @"D:\Xaml";

var writer = FileWriters.FirstOrDefault(x => x.GetType() == typeof(DrawingGroupXamlWriter));

await Task.Run(() => Export(outputPath, writer));

void Export(string outputPath, IFileWriter writer)
{
    var project = Project;
    foreach (var document in project.Documents)
    {
        var dir = Path.Combine(outputPath, document.Name);
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
}
