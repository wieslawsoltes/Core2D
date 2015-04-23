var c = Context?.Editor?.Container;
var line = XLine.Create(30, 30, 60, 30, c?.CurrentStyle, c?.PointShape);
c?.CurrentLayer?.Shapes?.Add(line);
c?.CurrentLayer?.Invalidate();