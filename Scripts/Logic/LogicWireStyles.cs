var styles = Context.Editor.Container.Styles;
var radiusX = 5.0;
var radiusY = 5.0;
var thickness = 2.0;
// wire
{
    var name = "Wire";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var startArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    var endArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    var lineStyle = LineStyle.Create(startArrowStyle, endArrowStyle);
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, lineStyle, textStyle);
    styles.Add(style);
}
// wire start inverted
{
    var name = "Wire-Start-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var startArrowStyle = ArrowStyle.Create(ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    var endArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    var lineStyle = LineStyle.Create(startArrowStyle, endArrowStyle);
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, lineStyle, textStyle);
    styles.Add(style);
}
// wire end inverted
{
    var name = "Wire-End-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var startArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
    var endArrowStyle = ArrowStyle.Create(ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    var lineStyle = LineStyle.Create(startArrowStyle, endArrowStyle);
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, lineStyle, textStyle);
    styles.Add(style);
}
// wire start & end inverted
{
    var name = "Wire-Start-End-Inverted";
    var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
    var startArrowStyle = ArrowStyle.Create(ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    var endArrowStyle = ArrowStyle.Create(ArrowType.Ellipse, isFilled: false, radiusX: radiusX, radiusY: radiusY);
    var lineStyle = LineStyle.Create(startArrowStyle, endArrowStyle);
    var textStyle = TextStyle.Create();
    var style = ShapeStyle.Create(name, stroke, fill, thickness, lineStyle, textStyle);
    styles.Add(style);
}