public static Container Create(double width = 810, double height = 600, bool grid = true)
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

    var pss = ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 1.0);
    //EllipsePointShape(c, pss);
    //FilledEllipsePointShape(c, pss);
    //RectanglePointShape(c, pss);
    //FilledRectanglePointShape(c, pss);
    CrossPointShape(c, pss);

    if (grid)
    {
        var g = LineGrid.Create(
            ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0),
            LineGrid.Settings.Create(0, 0, width, height, 30, 30));
        c.TemplateLayer.Shapes.Add(g);
    }

    return c;
}

public static void EllipsePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XEllipse.Create(-4, -4, 4, 4, pss, null, false);
}

public static void FilledEllipsePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XEllipse.Create(-3, -3, 3, 3, pss, null, true);
}

public static void RectanglePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XRectangle.Create(-4, -4, 4, 4, pss, null, false);
}

public static void FilledRectanglePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XRectangle.Create(-3, -3, 3, 3, pss, null, true);
}

public static void CrossPointShape(Container c, ShapeStyle pss)
{
    var g = XGroup.Create("PointShape");
    g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
    g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
    c.PointShape = g;
}

Context.Editor.Load(Create());