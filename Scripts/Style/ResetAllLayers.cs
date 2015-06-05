
void SetStyle(IEnumerable<BaseShape> shapes, ShapeStyle style)
{
    if (shapes == null || style == null)
        return;

    foreach (var shape in shapes)
    {
        shape.Style = style;
        if (shape is XGroup)
        {
            SetStyle((shape as XGroup).Shapes, style);
        }
    }
}

var p = Context.Editor.Project;
var layers = p.CurrentContainer.Layers;
var style = p.CurrentStyleLibrary.CurrentStyle;

foreach (var layer in layers)
{
    SetStyle(layer.Shapes, style);
    layer.Invalidate();
}
