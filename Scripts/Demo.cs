void Lines(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var l = XLine.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(l);
    }
}

void Rectangles(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var r = XRectangle.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(r);
    }
}

void Ellipses(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var e = XEllipse.Create(x1, y1, x2, y2, style, ps);
        layer.Shapes.Add(e);
    }
}

void Arcs(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
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
        layer.Shapes.Add(a);
    }
}

void Beziers(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
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
        layer.Shapes.Add(b);
    }
}

void QBeziers(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
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
        layer.Shapes.Add(b);
    }
}

void Texts(BaseShape ps, int n, double width, double height, IList<ShapeStyle> styles, Layer layer, Random rand)
{
    var sb = new byte[n];
    rand.NextBytes(sb);
    for (int i = 0; i < n; i++)
    {
        var style = styles[sb[i]];
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var t = XText.Create(x1, y1, x2, y2, style, ps, "Demo");
        layer.Shapes.Add(t);
    }
}

void Demo(Project p, int n = 100)
{
    var ps = p.PointShape;
    var c = p.Documents.FirstOrDefault().Containers.FirstOrDefault();
    var width = c.Width;
    var height = c.Height;
    var rand = new Random(Guid.NewGuid().GetHashCode());

    var ll = Layer.Create("Demo-Lines");
    var rl = Layer.Create("Demo-Rectangles");
    var el = Layer.Create("Demo-Ellipses");
    var al = Layer.Create("Demo-Arcs");
    var bl = Layer.Create("Demo-Beziers");
    var ql = Layer.Create("Demo-QBeziers");
    var tl = Layer.Create("Demo-Texts");
    c.Layers.Add(ll);
    c.Layers.Add(rl);
    c.Layers.Add(el);
    c.Layers.Add(al);
    c.Layers.Add(bl);
    c.Layers.Add(ql);
    c.Layers.Add(tl);

    var sg = ShapeStyleGroup.Create("Demo");
    p.StyleGroups.Add(sg);
    sg.Styles.Clear();
    for (int i = 0; i <= 255; i++)
    {
        var b = new byte[8];
        rand.NextBytes(b);
        var style = ShapeStyle.Create(
            i.ToString(), 
            b[0], b[1], b[2], b[3], 
            b[4], b[5], b[6], b[7], 
            2.0);
        sg.Styles.Add(style);
    }

    Lines(ps, n, width, height, sg.Styles, ll, rand);
    Rectangles(ps, n, width, height, sg.Styles, rl, rand);
    Ellipses(ps, n, width, height, sg.Styles, el, rand);
    Arcs(ps, n, width, height, sg.Styles, al, rand);
    Beziers(ps, n, width, height, sg.Styles, bl, rand);
    QBeziers(ps, n, width, height, sg.Styles, ql, rand);
    Texts(ps, n, width, height, sg.Styles, tl, rand);

    c.Invalidate();
}

Demo(Context.Editor.Project, 100);