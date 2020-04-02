if (Editor.PageState.SelectedShape != null)
{
    Editor.PageState.SelectedShape.IsStroked = true;
}

if (Editor.PageState.SelectedShapes != null)
{
    foreach (var shape in Editor.PageState.SelectedShapes)
    {
        shape.IsStroked = true;
    }
}
