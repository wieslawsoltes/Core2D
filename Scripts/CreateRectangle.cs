var c = Context?.Editor?.Container;
var rectangle = XRectangle.Create(30, 30, 60, 60, c?.CurrentStyle, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(rectangle);
c?.CurrentLayer?.Invalidate();