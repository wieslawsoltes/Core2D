if (PageState.SelectedShape != null)
{
    PageState.SelectedShape.IsFilled = true;
}

if (PageState.SelectedShapes != null)
{
    foreach (var shape in PageState.SelectedShapes)
    {
        shape.IsFilled = true;
    }
}
