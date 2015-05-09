void Lines(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var l = XLine.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(l);
    }
}

void Rectangles(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var r = XRectangle.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(r);
    }
}

void Ellipses(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var e = XEllipse.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(e);
    }
}

void Arcs(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var a = XArc.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(a);
    }
}

void Beziers(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        double x4 = rand.NextDouble() * width;
        double y4 = rand.NextDouble() * height;
        var b = XBezier.Create(x1, y1, x2, y2, x3, y3, x4, y4, style, ps);
        layer.Shapes.Add(b);
    }
}

void QBeziers(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        var b = XQBezier.Create(x1, y1, x2, y2, x3, y3, style, ps);
        layer.Shapes.Add(b);
    }
}

void Texts(BaseShape ps, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var t = XText.Create(x1, y1, x2, y2, style, ps, "Demo");
        layer.Shapes.Add(t);
    }
}

var p = Context.DefaultProject();
var ps = p.PointShape;
var c = p.Documents.FirstOrDefault().Containers.FirstOrDefault();
var styles = p.StyleGroups.FirstOrDefault().Styles;

var n = 100;
var width = c.Width;
var height = c.Height;
var rand = new Random(Guid.NewGuid().GetHashCode());

Lines(ps, n, width, height, styles[0], c.Layers[0], rand);
Rectangles(ps, n, width, height, styles[1], c.Layers[1], rand);
Ellipses(ps, n, width, height, styles[2], c.Layers[1], rand);
Arcs(ps, n, width, height, styles[2], c.Layers[1], rand);
Beziers(ps, n, width, height, styles[3], c.Layers[2], rand);
QBeziers(ps, n, width, height, styles[4], c.Layers[2], rand);
Texts(ps, n, width, height, styles[4], c.Layers[3], rand);

Context.Editor.Load(p);