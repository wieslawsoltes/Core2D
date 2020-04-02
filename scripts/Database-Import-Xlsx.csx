#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.Editor;
using Core2D.TextFieldReader.OpenXml;

var path = "D:\\Db.xlsx";
var reader = TextFieldReaders.FirstOrDefault(x => x.GetType() == typeof(OpenXmlReader));
using (var stream = FileIO.Open(path))
{
    var db = reader.Read(stream);
    Project.AddDatabase(db);
    Project.SetCurrentDatabase(db);
}
