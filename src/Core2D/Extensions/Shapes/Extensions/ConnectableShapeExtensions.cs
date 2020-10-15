using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Connectable shape extension methods.
    /// </summary>
    public static class ConnectableShapeExtensions
    {
        /// <summary>
        /// Adds <see cref="PointShape"/> to <see cref="ConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.None"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsNone(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.None;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="PointShape"/> to <see cref="ConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.Input"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsInput(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="PointShape"/> to <see cref="ConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.Output"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsOutput(this ConnectableShape shape, PointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }
    }
}
