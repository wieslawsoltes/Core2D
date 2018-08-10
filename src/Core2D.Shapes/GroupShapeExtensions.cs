// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shapes.Interfaces;

namespace Core2D.Shapes
{
    /// <summary>
    /// Group shape extension methods.
    /// </summary>
    public static class GroupShapeExtensions
    {
        /// <summary>
        /// Adds <see cref="IShape"/> to <see cref="IGroupShape.Shapes"/> collection.
        /// </summary>
        /// <param name="group">The group shape.</param>
        /// <param name="shape">The shape object.</param>
        public static void AddShape(this IGroupShape group, IShape shape)
        {
            shape.Owner = group;
            shape.State.Flags &= ~ShapeStateFlags.Standalone;
            group.Shapes = group.Shapes.Add(shape);
        }

        /// <summary>
        /// Creates a new <see cref="IGroupShape"/> instance.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="name">The group name.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <returns>The new instance of the <see cref="IGroupShape"/> class.</returns>
        public static IGroupShape Group(this IEnumerable<IShape> shapes, string name = "g", IList<IShape> source = null)
        {
            var group = GroupShape.Create(name);

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

            return group;
        }

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="source">The source shapes collection.</param>
        private static void Ungroup(IEnumerable<IShape> shapes, IList<IShape> source)
        {
            if (shapes != null && source != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is PointShape point)
                    {
                        // Remove connector related state flags.
                        point.State.Flags &=
                            ~(ShapeStateFlags.Connector
                            | ShapeStateFlags.None
                            | ShapeStateFlags.Input
                            | ShapeStateFlags.Output);
                    }

                    // Add shape standalone flag.
                    shape.State.Flags |= ShapeStateFlags.Standalone;

                    // Add shape to source collection.
                    source.Add(shape);
                }
            }
        }

        /// <summary>
        /// Ungroup group shape.
        /// </summary>
        /// <param name="group">The group instance.</param>
        /// <param name="source">The source shapes collection.</param>
        public static void Ungroup(this IGroupShape group, IList<IShape> source)
        {
            Ungroup(group.Shapes, source);
            Ungroup(group.Connectors, source);

            // Remove group from source collection.
            source.Remove(group);
        }
    }
}
