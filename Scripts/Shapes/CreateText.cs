var c = Context?.Editor?.Container;
var text = XText.Create(30, 30, 90, 60, c?.CurrentStyle, c?.PointShape, "Text", isFilled: false);
c?.CurrentLayer?.Shapes?.Add(text);
c?.CurrentLayer?.Invalidate();