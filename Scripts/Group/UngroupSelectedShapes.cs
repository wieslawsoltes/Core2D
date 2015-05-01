void Ungroup(IEnumerable<BaseShape> shapes, Layer layer, bool isGroup)
{
    if (shapes == null || layer == null)
        return;
    foreach (var shape in shapes)
    {
        if (shape is XGroup)
        {
            var g = shape as XGroup;
            Ungroup(g.Shapes, layer, isGroup: true);
            Ungroup(g.Connectors, layer, isGroup: true);
            layer.Shapes.Remove(g);
        }
        else if (isGroup)
        {
            if (shape is XPoint)
            {
                shape.State &= ~ShapeState.Connector;
            }
            else
            {
                layer.Shapes.Add(shape);
            }
        }
    }
}

var shapes = Context?.Editor?.Renderer?.SelectedShapes;
var shape = Context?.Editor?.Renderer?.SelectedShape;
var layer = Context?.Editor?.Container?.CurrentLayer;

if (shape != null && shape is XGroup && layer != null)
{
    var g = shape as XGroup;
    Ungroup(g.Shapes, layer, isGroup: true);
    Ungroup(g.Connectors, layer, isGroup: true);
    layer.Shapes.Remove(g);
    layer.Invalidate();
    Context.Editor.Renderer.SelectedShape = null;
}

if (shapes != null && layer != null)
{
    Ungroup(shapes, layer, isGroup: false);
    layer.Invalidate();
    Context.Editor.Renderer.SelectedShapes = null;
}