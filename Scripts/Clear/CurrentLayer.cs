var layer = Context?.Editor?.Project?.CurrentContainer?.CurrentLayer;
layer?.Shapes.Clear();
layer?.Invalidate();