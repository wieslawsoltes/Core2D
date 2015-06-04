
void SetStyle(IEnumerable<BaseShape> shapes, ShapeStyle style)
{
    if (shapes == null || style == null)
        return;
    foreach (var shape in shapes)
    {
        shape.Style = style;
        if (shape is XGroup)
            SetStyle((shape as XGroup).Shapes, style);
    }
}

var layer = Context?.Editor?.Project?.CurrentContainer?.CurrentLayer;
var style = Context?.Editor?.Project?.CurrentStyleGroup?.CurrentStyle;

SetStyle(layer?.Shapes, style);
layer?.Invalidate();
