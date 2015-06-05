
var name = "New";
var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
var thickness = 2.0;
var startArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
var endArrowStyle = ArrowStyle.Create(ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
var lineStyle = LineStyle.Create(startArrowStyle, endArrowStyle);
var textStyle = TextStyle.Create(fontName: "Calibri", fontFile: "calibri.ttf", fontSize: 12.0, fontStyle: FontStyle.Regular, textHAlignment: TextHAlignment.Center, textVAlignment: TextVAlignment.Center);
var style = ShapeStyle.Create(name, stroke, fill, thickness, lineStyle, textStyle);

var sl = Context.Editor.Project.CurrentStyleLibrary;

sl.Styles = sl.Styles.Add(style);
