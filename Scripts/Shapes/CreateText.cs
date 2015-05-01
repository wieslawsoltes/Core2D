var c = Context?.Editor?.Container;
var s = c?.CurrentStyleGroup?.CurrentStyle;
var text = XText.Create(30, 30, 90, 60, s, c?.PointShape, "Text", isFilled: false);
c?.CurrentLayer?.Shapes?.Add(text);
c?.CurrentLayer?.Invalidate();