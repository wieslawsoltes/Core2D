// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Defines selectable shape contract.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Moves shape to new position using X and Y axis offset.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        void Move(ISelection selection, double dx, double dy);

        /// <summary>
        /// Selects the shape.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        void Select(ISelection selection);

        /// <summary>
        /// Deselects the shape.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        void Deselect(ISelection selection);
    }
}
