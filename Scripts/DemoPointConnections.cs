
var p = Context.Editor.Project;
var c = p.CurrentContainer;
var l = c.CurrentLayer.Shapes
var s = p.CurrentStyleLibrary.CurrentStyle;
var ps = p.Options.PointShape;

var l1 = XLine.Create(30, 30, 60, 30, s, ps);
var l2 = XLine.Create(60, 30, 60, 60, s, ps);
var l3 = XLine.Create(60, 30, 90, 30, s, ps);

l2.Start = l1.End;
l3.Start = l1.End;

var builder = l.Shapes.ToBuilder();
builder.Add(l1);
builder.Add(l2);
builder.Add(l3);
l.Shapes = builder.ToImmutable();

l.Invalidate();
