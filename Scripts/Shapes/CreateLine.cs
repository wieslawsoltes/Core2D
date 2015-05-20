var p = Context?.Editor?.Project;
var c = p?.CurrentContainer;
var s = p?.CurrentStyleGroup?.CurrentStyle;
var line = XLine.Create(30, 30, 60, 30, s, p?.Options?.PointShape);
c?.CurrentLayer?.Shapes?.Add(line);
c?.CurrentLayer?.Invalidate();