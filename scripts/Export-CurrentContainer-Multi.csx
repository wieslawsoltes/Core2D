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
var page = Editor.Project.CurrentContainer;

var dxf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(DxfWriter));
using (var dxfStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + dxf.Extension)))
{
    dxf.Save(dxfStream, page, Editor.Project);
}

var pdf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(PdfSharpWriter));
using (var pdfStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + pdf.Extension)))
{
    pdf.Save(pdfStream, page, Editor.Project);
}

var png = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(PngSkiaSharpWriter));
using (var pngStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + png.Extension)))
{
    png.Save(pngStream, page, Editor.Project);
}

var svg = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(SvgSkiaSharpWriter));
using (var svgStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + svg.Extension)))
{
    svg.Save(svgStream, page, Editor.Project);
}

var emf = Editor.FileWriters.FirstOrDefault(x => x.GetType() == typeof(EmfWriter));
using (var emfStream = Editor.FileIO.Create(Path.Combine(dir, page.Name + "." + emf.Extension)))
{
    emf.Save(emfStream, page, Editor.Project);
}
