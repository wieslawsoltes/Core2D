// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shape;

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
        /// <param name="selected">The selected shapes set.</param>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        void Move(ISet<BaseShape> selected, double dx, double dy);

        /// <summary>
        /// Selects the shape.
        /// </summary>
        /// <param name="selected">The selected shapes set.</param>
        void Select(ISet<BaseShape> selected);

        /// <summary>
        /// Deselects the shape.
        /// </summary>
        /// <param name="selected">The selected shapes set.</param>
        void Deselect(ISet<BaseShape> selected);
    }
}
