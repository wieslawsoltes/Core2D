
struct Point
{
    public double X;
    public double Y;
    public static Point Create(double x, double y) => new Point(x, y);
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

struct Size
{
    public double Width;
    public double Height;
    public static Size Create(double width, double height) => new Size(width, height);
    public Size(double width, double height)
    {
        Width = width;
        Height = height;
    }
}

struct Settings
{
    public Point Origin;
    public Size GridSize;
    public Size CellSize;
    public static Settings Create(Point origin, Size grid, Size cell) => new Settings(origin, grid, cell);
    public Settings(Point origin, Size grid, Size cell)
    {
        Origin = origin;
        GridSize = grid;;
        CellSize = cell;
    }
}

IList<BaseShape> Create(ShapeStyle style, Settings settings, BaseShape point)
{
    double sx = settings.Origin.X + settings.CellSize.Width;
    double sy = settings.Origin.Y + settings.CellSize.Height;
    double ex = settings.Origin.X + settings.GridSize.Width;
    double ey = settings.Origin.Y + settings.GridSize.Height;

    var shapes = new List<BaseShape>();

    var g = XGroup.Create("grid");
    g.State &= ~ShapeState.Printable;

    for (double x = sx; x < ex; x += settings.CellSize.Width)
    {
        var line = XLine.Create(x, settings.Origin.Y, x, ey, style, point);
        line.State &= ~ShapeState.Printable;
        shapes.Add(line);
    }

    for (double y = sy; y < ey; y += settings.CellSize.Height)
    {
        var line = XLine.Create(settings.Origin.X, y, ex, y, style, point);
        line.State &= ~ShapeState.Printable;
        shapes.Add(line);
    }

    return shapes;
}

var p = Context.Editor.Project;
var c = p.CurrentContainer;
var layer = c.Layers.FirstOrDefault();
var style = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
var settings = Settings.Create(Point.Create(0, 0), Size.Create(c.Width, c.Height), Size.Create(30, 30));
var shapes = Create(style, settings, p.Options.PointShape);

foreach (var shape in shapes) 
{
    layer.Shapes.Add(shape);
}

layer.Invalidate();
