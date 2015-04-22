var container = Context.Editor.Container;
var line = XLine.Create(30, 30, 120, 30, container.CurrentStyle, container.PointShape);
container.CurrentLayer.Shapes.Add(line);
container.CurrentLayer.Invalidate();