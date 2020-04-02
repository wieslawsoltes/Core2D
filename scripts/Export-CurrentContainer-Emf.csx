#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Emf;

var dir = "D:\\";
var page = Project.CurrentContainer;
var writer = FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));
var path = Path.Combine(dir, page.Name + "." + writer.Extension);
using (var stream = FileIO.Create(path))
{
    writer.Save(stream, page, Project);
}
