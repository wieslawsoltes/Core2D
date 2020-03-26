#r "System.Linq"
#r "Core2D"
using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Emf;

var dir = "D:\\";
var emf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));

foreach (var page in Editor.Project.CurrentDocument.Pages)
{
    using (var emfStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + emf.Extension)))
    {
        emf.Save(emfStream, page, Editor.Project);
    }
}
