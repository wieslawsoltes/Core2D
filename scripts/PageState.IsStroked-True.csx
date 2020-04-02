if (PageState.SelectedShape != null)
{
    PageState.SelectedShape.IsStroked = true;
}

if (PageState.SelectedShapes != null)
{
    foreach (var shape in PageState.SelectedShapes)
    {
        shape.IsStroked = true;
    }
}
