var p = Context?.Editor?.Project;
var c = p?.CurrentContainer;
var s = p?.CurrentStyleGroup?.CurrentStyle;
var text = XText.Create(30, 30, 90, 60, s, p?.Options?.PointShape, "Text", isFilled: false);
c?.CurrentLayer?.Shapes?.Add(text);
c?.CurrentLayer?.Invalidate();