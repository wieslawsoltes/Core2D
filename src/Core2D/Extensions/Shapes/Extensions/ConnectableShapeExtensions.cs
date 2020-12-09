using Core2D.Renderer;

namespace Core2D.Shapes
{
    public static class ConnectableShapeExtensions
    {
        public static void AddConnectorAsNone(this ConnectableShapeViewModel shapeViewModel, PointShapeViewModel point)
        {
            point.Owner = shapeViewModel;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.None;
            point.State &= ~ShapeStateFlags.Standalone;
            shapeViewModel.Connectors = shapeViewModel.Connectors.Add(point);
        }

        public static void AddConnectorAsInput(this ConnectableShapeViewModel shapeViewModel, PointShapeViewModel point)
        {
            point.Owner = shapeViewModel;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
            point.State &= ~ShapeStateFlags.Standalone;
            shapeViewModel.Connectors = shapeViewModel.Connectors.Add(point);
        }

        public static void AddConnectorAsOutput(this ConnectableShapeViewModel shapeViewModel, PointShapeViewModel point)
        {
            point.Owner = shapeViewModel;
            point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
            point.State &= ~ShapeStateFlags.Standalone;
            shapeViewModel.Connectors = shapeViewModel.Connectors.Add(point);
        }
    }
}
