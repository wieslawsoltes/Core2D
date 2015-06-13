
var p = Context.Editor.Project;
var sl = p.StyleLibraries.FirstOrDefault(x => x.Name == "Logic-Wires");
if (sl == null)
{
    sl = StyleLibrary.Create("Logic-Wires");
    p.StyleLibraries = p.StyleLibraries.Add(sl);
}

p.CurrentStyleLibrary = sl;

var styles = sl.Styles.ToBuilder();
var radiusX = 5.0;
var radiusY = 5.0;
var thickness = 2.0;

// wire
{
    var name = "Logic-Wire";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var lineStyle = LineStyle.Create();
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, textStyle, lineStyle, null, null);
    style.StartArrowStyle = ArrowStyle.Create("Start", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    style.EndArrowStyle = ArrowStyle.Create("End", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    styles.Add(style);
}

// wire start inverted
{
    var name = "Logic-Wire-Start-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var lineStyle = LineStyle.Create();
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, textStyle, lineStyle, null, null);
    style.StartArrowStyle = ArrowStyle.Create("Start", style, ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    style.EndArrowStyle = ArrowStyle.Create("End", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    styles.Add(style);
}

// wire end inverted
{
    var name = "Logic-Wire-End-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var lineStyle = LineStyle.Create();
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, textStyle, lineStyle, null, null);
    style.StartArrowStyle = ArrowStyle.Create("Start", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    style.EndArrowStyle = ArrowStyle.Create("End", style, ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    styles.Add(style);
}

// wire start & end inverted
{
    var name = "Logic-Wire-Start/End-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var lineStyle = LineStyle.Create();
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, textStyle, lineStyle, null, null);
    style.StartArrowStyle = ArrowStyle.Create("Start", style, ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    style.EndArrowStyle = ArrowStyle.Create("End", style, ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    styles.Add(style);
}

sl.Styles = styles.ToImmutable();
sl.CurrentStyle = sl.Styles.FirstOrDefault();
