var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var ellipse = XEllipse.Create(30, 30, 60, 60, s, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(ellipse);
c?.CurrentLayer?.Invalidate();