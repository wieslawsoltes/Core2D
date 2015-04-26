void SetStyle(IList<BaseShape> shapes, ShapeStyle style)
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
var layer = Context?.Editor?.Container?.CurrentLayer;
var style = Context?.Editor?.Container?.CurrentStyle;
SetStyle(layer?.Shapes, style);
layer?.Invalidate();