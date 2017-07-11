// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shape;

namespace Core2D.Shapes
{
    /// <summary>
    /// Group shape extension methods.
    /// </summary>
    public static class GroupShapeExtensions
    {
        /// <summary>
        /// Adds <see cref="BaseShape"/> to <see cref="GroupShape.Shapes"/> collection.
        /// </summary>
        /// <param name="group">The group shape.</param>
        /// <param name="shape">The shape object.</param>
        public static void AddShape(this GroupShape group, BaseShape shape)
        {
            shape.Owner = group;
            shape.State.Flags &= ~ShapeStateFlags.Standalone;
            group.Shapes = group.Shapes.Add(shape);
        }

        /// <summary>
        /// Creates a new <see cref="GroupShape"/> instance.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="name">The group name.</param>
        /// <param name="source">The source shapes collection.</param>
        /// <returns>The new instance of the <see cref="GroupShape"/> class.</returns>
        public static GroupShape Group(this IEnumerable<BaseShape> shapes, string name = "g", IList<BaseShape> source = null)
        {
            var group = GroupShape.Create(name);

            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is PointShape)
                    {
                        group.AddConnectorAsNone(shape as PointShape);
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
        private static void Ungroup(IEnumerable<BaseShape> shapes, IList<BaseShape> source)
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
        public static void Ungroup(this GroupShape group, IList<BaseShape> source)
        {
            Ungroup(group.Shapes, source);
            Ungroup(group.Connectors, source);

            // Remove group from source collection.
            source.Remove(group);
        }
    }
}
