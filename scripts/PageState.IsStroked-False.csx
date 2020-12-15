if (Project.SelectedShapes != null)
{
    foreach (var shape in Project.SelectedShapes)
    {
        shape.IsStroked = false;
    }
}
