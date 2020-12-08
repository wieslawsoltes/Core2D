using Core2D.Renderer;

namespace Core2D.Shapes
{
    public static class ConnectableShapeExtensions
    {
        public static void AddConnectorAsNone(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.None;
            point.State &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        public static void AddConnectorAsInput(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
            point.State &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        public static void AddConnectorAsOutput(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
            point.State &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }
    }
}
