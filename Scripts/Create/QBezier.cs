var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleGroup.CurrentStyle;
var qbezier = XQBezier.Create(30, 30, 30, 60, 120, 30, s, p.Options.PointShape, isFilled: false);
c.CurrentLayer.Shapes = c.CurrentLayer.Shapes.Add(qbezier);
c.CurrentLayer.Invalidate();