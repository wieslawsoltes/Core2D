
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleGroup.CurrentStyle;
var qbezier = XQBezier.Create(30, 30, 30, 60, 120, 30, s, p.Options.PointShape, isFilled: false);

e.AddWithHistory(qbezier);
c.CurrentLayer.Invalidate();
