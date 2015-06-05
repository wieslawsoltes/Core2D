
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleLibrary.CurrentStyle;
var bezier = XBezier.Create(30, 30, 30, 60, 120, 0, 120, 30, s, p.Options.PointShape, isFilled: false);

e.AddWithHistory(bezier);
c.CurrentLayer.Invalidate();
