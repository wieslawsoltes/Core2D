var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var arc = XArc.Create(30, 60, 60, 60, s, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(arc);
c?.CurrentLayer?.Invalidate();