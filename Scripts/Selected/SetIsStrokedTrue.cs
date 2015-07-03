var layer = Context.Editor.Project.CurrentContainer.CurrentLayer;
var shapes = Context.Editor.Renderers[0].State.SelectedShapes;
if (shapes != null && layer != null)
{
    foreach (var shape in shapes)
    {
        shape.IsStroked = true;
    }
    layer.Invalidate();
}