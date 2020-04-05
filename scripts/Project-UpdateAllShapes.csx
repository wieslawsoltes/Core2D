#r "System.Linq"
#r "Core2D"
using System;
using System.Linq;
using Core2D.Shapes;

void UpdateShape(IBaseShape shape, Action<IBaseShape> updater)
{
    updater(shape);

    if (shape is IGroupShape group)
    {
        foreach (var child in group.Shapes)
        {
            UpdateShape(child, updater);
        }
    }
}

void UpdateAllShapes(Action<IBaseShape> updater)
{
    var projectShapes = Project.Documents.SelectMany(x => x.Pages).SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
    foreach (var shape in projectShapes)
    {
        UpdateShape(shape, updater);
    }

    var templateShapes = Project.Templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
    foreach (var shape in templateShapes)
    {
        UpdateShape(shape, updater);
    }
}

UpdateAllShapes((shape) =>
{
    if (shape is ITextShape text && !(shape is IEllipseShape) && !(shape is IImageShape) && !(shape is IRectangleShape))
    {
        text.IsFilled = true;
        text.IsStroked = false;
    }
});
