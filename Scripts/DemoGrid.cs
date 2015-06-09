
struct Settings
{
    public Point2 Origin;
    public Size2 GridSize;
    public Size2 CellSize;
    public static Settings Create(Point2 origin, Size2 grid, Size2 cell) => new Settings(origin, grid, cell);
    public Settings(Point2 origin, Size2 grid, Size2 cell)
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

var style = default(ShapeStyle);
var sl = p.StyleLibraries.FirstOrDefault(g => g.Name == "Template");
if (sl != null)
{
    style = sl.Styles.FirstOrDefault(s => s.Name == "Grid");
}
else
{
    sl = p.CurrentStyleLibrary;
}

if (style == null)
{
    style = ShapeStyle.Create("Grid", 255, 222, 222, 222, 255, 222, 222, 222, 1.0);
    sl.Styles = sl.Styles.Add(style);
}

var settings = Settings.Create(Point2.Create(0, 0), Size2.Create(c.Width, c.Height), Size2.Create(30, 30));
var shapes = Create(style, settings, p.Options.PointShape);

var builder = layer.Shapes.ToBuilder();
foreach (var shape in shapes) 
{
    builder.Add(shape);
}
layer.Shapes = builder.ToImmutable();

layer.Invalidate();
