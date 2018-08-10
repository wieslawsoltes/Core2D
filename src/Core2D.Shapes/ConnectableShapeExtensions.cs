// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Core2D.Shapes.Interfaces;

namespace Core2D.Shapes
{
    /// <summary>
    /// Connectable shape extension methods.
    /// </summary>
    public static class ConnectableShapeExtensions
    {
        /// <summary>
        /// Adds <see cref="IPointShape"/> to <see cref="IConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.None"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsNone(this IConnectableShape shape, IPointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.None;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="IPointShape"/> to <see cref="IConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.Input"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsInput(this IConnectableShape shape, IPointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }

        /// <summary>
        /// Adds <see cref="IPointShape"/> to <see cref="IConnectableShape.Connectors"/> collection with <see cref="ShapeStateFlags.Output"/> flag set.
        /// </summary>
        /// <param name="shape">The connectable shape.</param>
        /// <param name="point">The point object.</param>
        public static void AddConnectorAsOutput(this IConnectableShape shape, IPointShape point)
        {
            point.Owner = shape;
            point.State.Flags |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
            point.State.Flags &= ~ShapeStateFlags.Standalone;
            shape.Connectors = shape.Connectors.Add(point);
        }
    }
}
