if (Editor.PageState.SelectedShape != null)
{
    Editor.PageState.SelectedShape.IsFilled = false;
}

if (Editor.PageState.SelectedShapes != null)
{
    foreach (var shape in Editor.PageState.SelectedShapes)
    {
        shape.IsFilled = false;
    }
}
