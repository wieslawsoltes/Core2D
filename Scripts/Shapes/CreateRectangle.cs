var p = Context?.Editor?.Project;
var c = p?.CurrentContainer;
var s = p?.CurrentStyleGroup?.CurrentStyle;
var rectangle = XRectangle.Create(30, 30, 60, 60, s, p?.Options?.PointShape, isFilled: false);
c?.CurrentLayer?.Shapes?.Add(rectangle);
c?.CurrentLayer?.Invalidate();