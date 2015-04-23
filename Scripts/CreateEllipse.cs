var c = Context?.Editor?.Container;
var ellipse = XEllipse.Create(30, 30, 60, 60, c?.CurrentStyle, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(ellipse);
c?.CurrentLayer?.Invalidate();