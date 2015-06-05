
var e = Context.Editor;
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleLibrary.CurrentStyle;
var image = XImage.Create(30, 30, 90, 60, s, p.Options.PointShape, new Uri("C:\\image.jpg"), isFilled: false);

e.AddWithHistory(image);
c.CurrentLayer.Invalidate();
