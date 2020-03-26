#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Emf;

var dir = "D:\\";
var page = Editor.Project.CurrentContainer;
var writer = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));
var path = Path.Combine(dir, page.Name + "." + writer.Extension);
using (var stream = Editor.FileIO.Create(path))
{
    writer.Save(stream, page, Editor.Project);
}
