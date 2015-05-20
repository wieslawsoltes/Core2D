var p = Context.Editor.Project;
var r = Context.Editor.Renderer;
if (r.SelectedShape != null)
{
    p.Options.PointShape = r.SelectedShape;
}