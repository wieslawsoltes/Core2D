
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleLibrary.CurrentStyle;
var rectangle = XRectangle.Create(30, 30, 60, 60, s, p.Options.PointShape, isFilled: false);

e.AddWithHistory(rectangle);
c.CurrentLayer.Invalidate();
