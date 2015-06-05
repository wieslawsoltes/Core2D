
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleLibrary.CurrentStyle;
var line = XLine.Create(30, 30, 60, 30, s, p.Options.PointShape);

e.AddWithHistory(line);
c.CurrentLayer.Invalidate();
