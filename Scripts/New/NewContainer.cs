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
        GroupLibraries = new ObservableCollection<GroupLibrary>(),
        Layers = new ObservableCollection<Layer>(),
        StyleGroups = new ObservableCollection<ShapeStyleGroup>()
    };

    c.Layers.Add(Layer.Create("Layer1"));
    c.Layers.Add(Layer.Create("Layer2"));
    c.Layers.Add(Layer.Create("Layer3"));
    c.Layers.Add(Layer.Create("Layer4"));

    c.CurrentLayer = c.Layers.FirstOrDefault();

    c.TemplateLayer = Layer.Create("Template");
    c.WorkingLayer = Layer.Create("Working");

    // default group library
    var gld = GroupLibrary.Create("Default");
    c.GroupLibraries.Add(gld);
    c.CurrentGroupLibrary = c.GroupLibraries.FirstOrDefault();

    // default styles group
    var sgd = ShapeStyleGroup.Create("Default");
    sgd.Styles.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
    sgd.Styles.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
    sgd.Styles.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
    sgd.Styles.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
    sgd.Styles.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
    sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

    c.StyleGroups.Add(sgd);
    c.CurrentStyleGroup = c.StyleGroups.FirstOrDefault();

    // template styles group
    var sgt = ShapeStyleGroup.Create("Template");
    var pss = ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 2.0);
    var gs = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
    sgt.Styles.Add(pss);
    sgt.Styles.Add(gs);
    c.StyleGroups.Add(sgt);
    sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

    if (pst != PointShapeType.None)
    {
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
        var settings = LineGrid.Settings.Create(0, 0, width, height, 30, 30);
        var g = LineGrid.Create(gs, settings);
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