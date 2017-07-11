// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    }
}
