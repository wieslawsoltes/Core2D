var p = Context?.Editor?.Project;
var c = p?.CurrentContainer;
var s = p?.CurrentStyleGroup?.CurrentStyle;
var arc = XArc.Create(30, 60, 60, 60, s, p?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(arc);
c?.CurrentLayer?.Invalidate();