
void Lines(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var l = XLine.Create(x1, y1, x2, y2, style, ps);
        builder.Add(l);
    }
    layer.Shapes = builder.ToImmutable();
}

void Rectangles(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var r = XRectangle.Create(x1, y1, x2, y2, style, ps);
        builder.Add(r);
    }
    layer.Shapes = builder.ToImmutable();
}

void Ellipses(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var e = XEllipse.Create(x1, y1, x2, y2, style, ps);
        builder.Add(e);
    }
    layer.Shapes = builder.ToImmutable();
}

void Arcs(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        double x4 = rand.NextDouble() * width;
        double y4 = rand.NextDouble() * height;
        var a = XArc.Create(x1, y1, x2, y2, x3, y3, x4, y4, style, ps);
        builder.Add(a);
    }
    layer.Shapes = builder.ToImmutable();
}

void Beziers(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        double x4 = rand.NextDouble() * width;
        double y4 = rand.NextDouble() * height;
        var b = XBezier.Create(x1, y1, x2, y2, x3, y3, x4, y4, style, ps);
        builder.Add(b);
    }
    layer.Shapes = builder.ToImmutable();
}

void QBeziers(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        var b = XQBezier.Create(x1, y1, x2, y2, x3, y3, style, ps);
        builder.Add(b);
    }
    layer.Shapes = builder.ToImmutable();
}

void Texts(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);

    var builder = layer.Shapes.ToBuilder();
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var t = XText.Create(x1, y1, x2, y2, style, ps, "Demo");
        builder.Add(t);
    }
    layer.Shapes = builder.ToImmutable();
}

void Demo(Project p, int n = 100)
{
    var ps = p.Options.PointShape;
    var c = p.Documents.FirstOrDefault().Containers.FirstOrDefault();
    var width = c.Width;
    var height = c.Height;
    var rand = new Random(Guid.NewGuid().GetHashCode());

    var ll = Layer.Create("Demo-Lines", c);
    var rl = Layer.Create("Demo-Rectangles", c);
    var el = Layer.Create("Demo-Ellipses", c);
    var al = Layer.Create("Demo-Arcs", c);
    var bl = Layer.Create("Demo-Beziers", c);
    var ql = Layer.Create("Demo-QBeziers", c);
    var tl = Layer.Create("Demo-Texts", c);

    var layersBuilder = c.Layers.ToBuilder();
    layersBuilder.Add(ll);
    layersBuilder.Add(rl);
    layersBuilder.Add(el);
    layersBuilder.Add(al);
    layersBuilder.Add(bl);
    layersBuilder.Add(ql);
    layersBuilder.Add(tl);
    c.Layers = layersBuilder.ToImmutable();

    var sl = StyleLibrary.Create("Demo");
    p.StyleLibraries = p.StyleLibraries.Add(sl);

    sl.Styles = sl.Styles.Clear();
    var stylesBuilder = sl.Styles.ToBuilder();
    for (int i = 0; i <= 255; i++)
    {
        var b = new byte[8];
        rand.NextBytes(b);
        var style = ShapeStyle.Create(
            i.ToString(), 
            b[0], b[1], b[2], b[3], 
            b[4], b[5], b[6], b[7], 
            2.0);
        stylesBuilder.Add(style);
    }
    sl.Styles = stylesBuilder.ToImmutable();

    Lines(ps, n, width, height, sl.Styles, ll, rand);
    Rectangles(ps, n, width, height, sl.Styles, rl, rand);
    Ellipses(ps, n, width, height, sl.Styles, el, rand);
    Arcs(ps, n, width, height, sl.Styles, al, rand);
    Beziers(ps, n, width, height, sl.Styles, bl, rand);
    QBeziers(ps, n, width, height, sl.Styles, ql, rand);
    Texts(ps, n, width, height, sl.Styles, tl, rand);

    c.Invalidate();
}

Demo(Context.Editor.Project, 100);
