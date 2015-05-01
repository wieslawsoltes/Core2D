var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var rectangle = XRectangle.Create(30, 30, 60, 60, s, c?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(rectangle);
c?.CurrentLayer?.Invalidate();