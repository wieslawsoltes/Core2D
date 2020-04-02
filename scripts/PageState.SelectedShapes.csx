foreach (var shape in Editor.PageState.SelectedShapes)
{
    shape.IsFilled = true;
    shape.IsStroked= false;
}
