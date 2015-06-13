
var name = "New";
var stroke = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
var fill = ArgbColor.Create(a: 0xFF, r: 0x00, g: 0x00, b: 0x00);
var thickness = 2.0;
var lineStyle = LineStyle.Create("Line", maxLengthFlags: MaxLengthFlags.Disabled, maxLength: 15.0, maxLengthStartState: ShapeState.Connector | ShapeState.Output, maxLengthEndState: ShapeState.Connector | ShapeState.Input);
var textStyle = TextStyle.Create("Text", fontName: "Calibri", fontFile: "calibri.ttf", fontSize: 12.0, fontStyle: FontStyle.Regular, textHAlignment: TextHAlignment.Center, textVAlignment: TextVAlignment.Center);

var style = ShapeStyle.Create(name, stroke, fill, thickness, textStyle, lineStyle, null, null);
style.StartArrowStyle = ArrowStyle.Create("Start", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);
style.EndArrowStyle = ArrowStyle.Create("End", style, ArrowType.None, isFilled: false, radiusX: 0.0, radiusY: 0.0);

var sl = Context.Editor.Project.CurrentStyleLibrary;
sl.Styles = sl.Styles.Add(style);
