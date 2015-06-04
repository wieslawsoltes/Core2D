
IEnumerable<XPoint> GetPoints(IEnumerable<BaseShape> shapes)
{
    if (shapes == null)
        yield break;
    foreach (var shape in shapes)
    {
        if (shape is XPoint)
        {
            yield return shape as XPoint;
        }
        else if (shape is XLine)
        {
            var line = shape as XLine;
            yield return line.Start;
            yield return line.End;
        }
        else if (shape is XRectangle)
        {
            var rectangle = shape as XRectangle;
            yield return rectangle.TopLeft;
            yield return rectangle.BottomRight;
        }
        else if (shape is XEllipse)
        {
            var ellipse = shape as XEllipse;
            yield return ellipse.TopLeft;
            yield return ellipse.BottomRight;
        }
        else if (shape is XArc)
        {
            var arc = shape as XArc;
            yield return arc.Point1;
            yield return arc.Point2;
        }
        else if (shape is XBezier)
        {
            var bezier = shape as XBezier;
            yield return bezier.Point1;
            yield return bezier.Point2;
            yield return bezier.Point3;
            yield return bezier.Point4;
        }
        else if (shape is XQBezier)
        {
            var qbezier = shape as XQBezier;
            yield return qbezier.Point1;
            yield return qbezier.Point2;
            yield return qbezier.Point3;
        }
        else if (shape is XText)
        {
            var text = shape as XText;
            yield return text.TopLeft;
            yield return text.BottomRight;
        }
        else if (shape is XGroup)
        {
            var group = shape as XGroup;
            foreach (var point in GetPoints(group.Shapes))
            {
                yield return point;
            }
        }
    }
}

void Move(IEnumerable<XPoint> points, double dx, double dy)
{
    foreach (var point in points)
    {
        point.Move(dx, dy);
    }
}

var shapes = Context?.Editor?.Renderer?.SelectedShapes;
var layer = Context?.Editor?.Project?.CurrentContainer?.CurrentLayer;

if (shapes != null && layer != null)
{
    Move(GetPoints(shapes).Distinct(), dx: 30.0, dy: 30.0);
    layer.Invalidate();
}
