#r "Core2D.FileWriter.Dxf"
using System;
using System.Linq;
using Core2D.FileWriter.Dxf;

var dir = "D:\\";
var page = Editor.Project.CurrentContainer;
var writer = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(DxfWriter));
var path = dir + page.Name + "." + writer.Extension;
writer.Save(path, page, Editor.Project);
