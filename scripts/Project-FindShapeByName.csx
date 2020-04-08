#r "System.Linq"
#r "Core2D"
using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Shapes;
using Core2D.Style;

if (FindShapeByName("Rect1") is IRectangleShape rect1 && rect1.Style.Stroke is IArgbColor argb) WriteLine($"{rect1.Name}: {argb.A} {argb.R} {argb.G} {argb.B}");

foreach (var shape in GetAllShapes()) WriteLine($"{shape.GetType().Name}: {shape.Name}");

IEnumerable<IBaseShape> GetAllShapes() => Project.Documents.SelectMany(x => x.Pages).SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
IBaseShape FindShapeByName(string name) => GetAllShapes().FirstOrDefault(x => x.Name == name);
