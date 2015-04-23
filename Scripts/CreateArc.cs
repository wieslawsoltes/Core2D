var c = Context?.Editor?.Container;
var arc = XArc.Create(30, 60, 60, 60, c?.CurrentStyle, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(arc);
c?.CurrentLayer?.Invalidate();