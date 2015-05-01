var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var qbezier = XQBezier.Create(30, 30, 30, 60, 120, 30, s, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(qbezier);
c?.CurrentLayer?.Invalidate();