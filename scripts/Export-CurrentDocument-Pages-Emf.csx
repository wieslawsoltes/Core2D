#r "System.Linq"
#r "Core2D"
using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Emf;

var dir = "D:\\";
var emf = FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));

foreach (var page in Project.CurrentDocument.Pages)
{
    using (var emfStream = FileIO.Create(Path.Combine(dir, page.Name + "." + emf.Extension)))
    {
        emf.Save(emfStream, page, Project);
    }
}
