if (PageState.SelectedShape != null)
{
    PageState.SelectedShape.IsStroked = false;
}

if (PageState.SelectedShapes != null)
{
    foreach (var shape in PageState.SelectedShapes)
    {
        shape.IsStroked = false;
    }
}
