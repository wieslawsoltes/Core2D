
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
var layer = p.CurrentContainer.CurrentLayer;
var style = p.CurrentStyleLibrary.CurrentStyle;
var shapes = Context.Editor.Renderers[0].State.SelectedShapes;

SetStyle(shapes, style);
layer.Invalidate();
