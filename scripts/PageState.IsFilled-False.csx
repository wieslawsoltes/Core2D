if (PageState.SelectedShape != null)
{
    PageState.SelectedShape.IsFilled = false;
}

if (PageState.SelectedShapes != null)
{
    foreach (var shape in PageState.SelectedShapes)
    {
        shape.IsFilled = false;
    }
}
