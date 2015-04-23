var c = Context?.Editor?.Container;
var qbezier = XQBezier.Create(30, 30, 30, 60, 120, 30, c?.CurrentStyle, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(qbezier);
c?.CurrentLayer?.Invalidate();