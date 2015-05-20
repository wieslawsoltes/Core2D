var p = Context?.Editor?.Project;
var c = p?.CurrentContainer;
var s = p?.CurrentStyleGroup?.CurrentStyle;
var image = XImage.Create(30, 30, 90, 60, s, p?.Options?.PointShape, new Uri("C:\\image.jpg"), isFilled: false);
c?.CurrentLayer?.Shapes?.Add(image);
c?.CurrentLayer?.Invalidate();