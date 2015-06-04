
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleGroup.CurrentStyle;
var arc = XArc.Create(30, 30, 60, 60, 30, 45, 60, 45, s, p.Options.PointShape, isFilled: false);

e.AddWithHistory(arc);
c.CurrentLayer.Invalidate();
