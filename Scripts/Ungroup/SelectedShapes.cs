
ShapeState cs = ShapeState.Connector | ShapeState.None | ShapeState.Input | ShapeState.Output;

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
            layer.Shapes = layer.Shapes.Remove(g);
        }
        else if (isGroup)
        {
            if (shape is XPoint)
            {
                shape.State &= ~cs;
                shape.State |= ShapeState.Standalone;
                layer.Shapes = layer.Shapes.Add(shape);
            }
            else
            {
                shape.State |= ShapeState.Standalone;
                layer.Shapes = layer.Shapes.Add(shape);
            }
        }
    }
}

var editor = Context.Editor;
var renderer = editor.Renderer;
var shapes = renderer.SelectedShapes;
var shape = renderer.SelectedShape;
var layer = editor.Project.CurrentContainer.CurrentLayer;

if (shape != null && shape is XGroup && layer != null)
{
    var g = shape as XGroup;
    Ungroup(g.Shapes, layer, isGroup: true);
    Ungroup(g.Connectors, layer, isGroup: true);
    layer.Shapes = layer.Shapes.Remove(g);
    layer.Invalidate();
    renderer.SelectedShape = null;
}

if (shapes != null && layer != null)
{
    Ungroup(shapes, layer, isGroup: false);
    layer.Invalidate();
    renderer.SelectedShapes = null;
}
