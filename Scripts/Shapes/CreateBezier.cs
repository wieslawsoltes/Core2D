var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var bezier = XBezier.Create(30, 30, 30, 60, 120, 0, 120, 30, s, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(bezier);
c?.CurrentLayer?.Invalidate();