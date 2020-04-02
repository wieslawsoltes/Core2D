#r "System.Linq"
#r "Core2D"

using System;
using System.IO;
using System.Linq;
using Core2D.FileWriter.Dxf;
using Core2D.FileWriter.PdfSharp;
using Core2D.FileWriter.SkiaSharpPng;
using Core2D.FileWriter.SkiaSharpSvg;
using Core2D.FileWriter.Emf;

var dir = "D:\\";
var page = Project.CurrentContainer;

var dxf = FileWriters.FirstOrDefault(x => x.GetType() == typeof(DxfWriter));
using (var dxfStream = FileIO.Create(Path.Combine(dir, page.Name + "." + dxf.Extension)))
{
    dxf.Save(dxfStream, page, Project);
}

var pdf = FileWriters.FirstOrDefault(x => x.GetType() == typeof(PdfSharpWriter));
using (var pdfStream = FileIO.Create(Path.Combine(dir, page.Name + "." + pdf.Extension)))
{
    pdf.Save(pdfStream, page, Project);
}

var png = FileWriters.FirstOrDefault(x => x.GetType() == typeof(PngSkiaSharpWriter));
using (var pngStream = FileIO.Create(Path.Combine(dir, page.Name + "." + png.Extension)))
{
    png.Save(pngStream, page, Project);
}

var svg = FileWriters.FirstOrDefault(x => x.GetType() == typeof(SvgSkiaSharpWriter));
using (var svgStream = FileIO.Create(Path.Combine(dir, page.Name + "." + svg.Extension)))
{
    svg.Save(svgStream, page, Project);
}

var emf = FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));
using (var emfStream = FileIO.Create(Path.Combine(dir, page.Name + "." + emf.Extension)))
{
    emf.Save(emfStream, page, Project);
}
