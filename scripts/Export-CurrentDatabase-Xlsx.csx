#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.TextFieldWriter.OpenXml;

var dir = "D:\\";
var db = Editor.Project.CurrentDatabase ;
var writer = Editor.TextFieldWriters.FirstOrDefault(x => x.GetType() == typeof(OpenXmlWriter));
var path = Path.Combine(dir, db.Name + "." + writer.Extension);
using (var stream = Editor.FileIO.Create(path))
{
    writer.Write(stream, db);
}
