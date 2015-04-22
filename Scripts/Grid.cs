public struct Point
{
    public double X;
    public double Y;
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

public struct Size
{
    public double Width;
    public double Height;
    public Size(double width, double height)
    {
        Width = width;
        Height = height;
    }
}

public struct Settings
{
    public Point Origin;
    public Size GridSize;
    public Size CellSize;
    public static Settings Create(
        double originX,
        double originY,
        double gridWidth,
        double gridHeight,
        double cellWidth,
        double cellHeight)
    {
        return new Settings()
        {
            Origin = new Point(originX, originY),
            GridSize = new Size(gridWidth, gridHeight),
            CellSize = new Size(cellWidth, cellHeight)
        };
    }
}

public static XGroup Create(ShapeStyle style, Settings settings)
{
    double sx = settings.Origin.X + settings.CellSize.Width;
    double sy = settings.Origin.Y + settings.CellSize.Height;
    double ex = settings.Origin.X + settings.GridSize.Width;
    double ey = settings.Origin.Y + settings.GridSize.Height;

    var g = XGroup.Create("grid");
    g.State &= ~ShapeState.Printable;

    for (double x = sx; x < ex; x += settings.CellSize.Width)
    {
        var line = XLine.Create(
            x,
            settings.Origin.Y,
            x,
            ey,
            style, null);
        line.State &= ~ShapeState.Printable;
        g.Shapes.Add(line);
    }

    for (double y = sy; y < ey; y += settings.CellSize.Height)
    {
        var line = XLine.Create(
            settings.Origin.X,
            y,
            ex,
            y,
            style, null);
        line.State &= ~ShapeState.Printable;
        g.Shapes.Add(line);
    }

    return g;
}

var container = Context.Editor.Container;
var style = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
var settings = Settings.Create(0, 0, container.Width, container.Height, 30, 30);
var grid = Create(style, settings);

container.TemplateLayer.Shapes.Clear();
container.TemplateLayer.Shapes.Add(grid);
container.TemplateLayer.Invalidate();