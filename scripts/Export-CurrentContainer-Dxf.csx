#r "Core2D.FileWriter.Dxf"
using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Dxf;

var dir = "D:\\";
var page = Editor.Project.CurrentContainer;
var writer = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(DxfWriter));
var path = Path.Combine(dir, page.Name + "." + writer.Extension);
writer.Save(path, page, Editor.Project);
