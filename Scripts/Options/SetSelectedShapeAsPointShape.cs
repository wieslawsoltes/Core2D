
var p = Context.Editor.Project;
var s = Context.Editor.Renderers[0].State.SelectedShape;

if (s != null)
{
    p.Options.PointShape = s;
}
