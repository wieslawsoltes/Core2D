void Lines(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var l = XLine.Create(x1, y1, x2, y2, style, c.PointShape);
        layer.Shapes.Add(l);
    }
}

void Rectangles(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var r = XRectangle.Create(x1, y1, x2, y2, style, c.PointShape);
        layer.Shapes.Add(r);
    }
}

void Ellipses(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var e = XEllipse.Create(x1, y1, x2, y2, style, c.PointShape);
        layer.Shapes.Add(e);
    }
}

void Arcs(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var a = XArc.Create(x1, y1, x2, y2, style, c.PointShape);
        layer.Shapes.Add(a);
    }
}

void Beziers(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
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
        var b = XBezier.Create(x1, y1, x2, y2, x3, y3, x4, y4, style, c.PointShape);
        layer.Shapes.Add(b);
    }
}

void QBeziers(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        double x3 = rand.NextDouble() * width;
        double y3 = rand.NextDouble() * height;
        var b = XQBezier.Create(x1, y1, x2, y2, x3, y3, style, c.PointShape);
        layer.Shapes.Add(b);
    }
}

void Texts(Container c, int n, double width, double height, ShapeStyle style, Layer layer, Random rand)
{
    for (int i = 0; i < n; i++)
    {
        double x1 = rand.NextDouble() * width;
        double y1 = rand.NextDouble() * height;
        double x2 = rand.NextDouble() * width;
        double y2 = rand.NextDouble() * height;
        var t = XText.Create(x1, y1, x2, y2, style, c.PointShape, "Demo");
        layer.Shapes.Add(t);
    }
}

Container Create(double width, double height)
{
    var c = new Container()
    {
        Width = width,
        Height = height,
        Layers = new ObservableCollection<Layer>(),
        Styles = new ObservableCollection<ShapeStyle>()
    };
    c.Layers.Add(Layer.Create("Layer1"));
    c.Layers.Add(Layer.Create("Layer2"));
    c.Layers.Add(Layer.Create("Layer3"));
    c.Layers.Add(Layer.Create("Layer4"));
    c.CurrentLayer = c.Layers.FirstOrDefault();
    c.TemplateLayer = Layer.Create("Template");
    c.WorkingLayer = Layer.Create("Working");
    c.Styles.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
    c.Styles.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
    c.Styles.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
    c.Styles.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
    c.Styles.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
    c.CurrentStyle = c.Styles.FirstOrDefault();
    return c;
}

var c = Create(810, 600);
var n = 100;
var width = c.Width;
var height = c.Height;
var rand = new Random(Guid.NewGuid().GetHashCode());

Lines(c, n, width, height, c.Styles[0], c.Layers[0], rand);
Rectangles(c, n, width, height, c.Styles[1], c.Layers[1], rand);
Ellipses(c, n, width, height, c.Styles[2], c.Layers[1], rand);
Arcs(c, n, width, height, c.Styles[2], c.Layers[1], rand);
Beziers(c, n, width, height, c.Styles[3], c.Layers[2], rand);
QBeziers(c, n, width, height, c.Styles[4], c.Layers[2], rand);
Texts(c, n, width, height, c.Styles[4], c.Layers[3], rand);

Context.Editor.Load(c);