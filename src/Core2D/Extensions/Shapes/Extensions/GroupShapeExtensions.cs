using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Group shape extension methods.
    /// </summary>
    public static class GroupShapeExtensions
    {
        /// <summary>
        /// Adds <see cref="IBaseShape"/> to <see cref="IGroupShape.Shapes"/> collection.
        /// </summary>
        /// <param name="group">The group shape.</param>
        /// <param name="shape">The shape object.</param>
        public static void AddShape(this IGroupShape group, IBaseShape shape)
        {
            shape.Owner = group;
            shape.State.Flags &= ~ShapeStateFlags.Standalone;
            group.Shapes = group.Shapes.Add(shape);
        }

        /// <summary>
        /// Creates a new <see cref="IGroupShape"/> instance.
        /// </summary>
        /// <param name="group">The group shape.</param>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        public static void Group(this IGroupShape group, IEnumerable<IBaseShape> shapes, IList<IBaseShape> source = null)
        {
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is IPointShape)
                    {
                        group.AddConnectorAsNone(shape as IPointShape);
                    }
                    else
                    {
                        group.AddShape(shape);
                    }

                    if (source != null)
                    {
                        source.Remove(shape);
                    }
                }
            }

            if (source != null)
            {
                source.Add(group);
            }
        }

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        public static void Ungroup(IEnumerable<IBaseShape> shapes, IList<IBaseShape> source)
        {
            if (shapes != null && source != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is IPointShape point)
                    {
                        point.State.Flags &=
                            ~(ShapeStateFlags.Connector
                            | ShapeStateFlags.None
                            | ShapeStateFlags.Input
                            | ShapeStateFlags.Output);
                    }

                    shape.State.Flags |= ShapeStateFlags.Standalone;

                    if (source != null)
                    {
                        source.Add(shape);
                    }
                }
            }
        }

        /// <summary>
        /// Ungroup group shape.
        /// </summary>
        /// <param name="group">The group instance.</param>
        /// <param name="source">The source shapes collection.</param>
        public static void Ungroup(this IGroupShape group, IList<IBaseShape> source)
        {
            Ungroup(group.Shapes, source);
            Ungroup(group.Connectors, source);

            if (source != null)
            {
                source.Remove(group);
            }
        }
    }
}
