enum PointShapeType
{
    None,
    Ellipse,
    FilledEllipse,
    Rectangle,
    FilledRectangle,
    Cross
}

void EllipsePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XEllipse.Create(-4, -4, 4, 4, pss, null, false);
}

void FilledEllipsePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XEllipse.Create(-3, -3, 3, 3, pss, null, true);
}

void RectanglePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XRectangle.Create(-4, -4, 4, 4, pss, null, false);
}

void FilledRectanglePointShape(Container c, ShapeStyle pss)
{
    c.PointShape = XRectangle.Create(-3, -3, 3, 3, pss, null, true);
}

void CrossPointShape(Container c, ShapeStyle pss)
{
    var g = XGroup.Create("PointShape");
    g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
    g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
    c.PointShape = g;
}

Container Create(double width, double height, bool grid, PointShapeType pst)
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

    if (pst != PointShapeType.None)
    {
        var pss = ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 2.0);
        switch(pst)
        {
            case PointShapeType.None:
                break;
            case PointShapeType.Ellipse:
                EllipsePointShape(c, pss);
                break;
            case PointShapeType.FilledEllipse:
                FilledEllipsePointShape(c, pss);
                break;
            case PointShapeType.Rectangle:
                RectanglePointShape(c, pss);
                break;
            case PointShapeType.FilledRectangle:
                FilledRectanglePointShape(c, pss);
                break;
            case PointShapeType.Cross:
                CrossPointShape(c, pss);
                break;
        }
    }
    
    if (grid)
    {
        var style = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
        var settings = LineGrid.Settings.Create(0, 0, width, height, 30, 30);
        var g = LineGrid.Create(style, settings);
        c.TemplateLayer.Shapes.Add(g);
    }

    return c;
}

var c = Create(
    width: 810, 
    height: 600, 
    grid: true, 
    pst: PointShapeType.Cross);

Context.Editor.Load(c);