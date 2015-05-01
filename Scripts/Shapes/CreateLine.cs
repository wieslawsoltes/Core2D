var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var line = XLine.Create(30, 30, 60, 30, s, c?.PointShape);
c?.CurrentLayer?.Shapes?.Add(line);
c?.CurrentLayer?.Invalidate();