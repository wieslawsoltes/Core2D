#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.TextFieldWriter.OpenXml;

var dir = "D:\\";
var db = Project.CurrentDatabase;
var writer = TextFieldWriters.FirstOrDefault(x => x.GetType() == typeof(OpenXmlWriter));
var path = Path.Combine(dir, db.Name + "." + writer.Extension);
using (var stream = FileIO.Create(path))
{
    writer.Write(stream, db);
}
