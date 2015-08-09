
if (SelectedShape != null)
    SelectedShape.Move(0,30);
    
if (SelectedShapes != null)
    foreach (var shape in SelectedShapes)
        shape.Move(0,30);
