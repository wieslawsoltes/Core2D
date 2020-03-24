#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Dxf;
using Core2D.FileWriter.PdfSharp;
using Core2D.FileWriter.SkiaSharpPng;
using Core2D.FileWriter.SkiaSharpSvg;

var dir = "D:\\";
var page = Editor.Project.CurrentContainer;

var dxf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(DxfWriter));
dxf.Save(Path.Combine(dir, page.Name + "." + dxf.Extension), page, Editor.Project);

var pdf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(PdfSharpWriter));
pdf.Save(Path.Combine(dir, page.Name + "." + pdf.Extension), page, Editor.Project);

var png = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(PngSkiaSharpWriter));
png.Save(Path.Combine(dir, page.Name + "." + png.Extension), page, Editor.Project);

var svg = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(SvgSkiaSharpWriter));
svg.Save(Path.Combine(dir, page.Name + "." + svg.Extension), page, Editor.Project);
