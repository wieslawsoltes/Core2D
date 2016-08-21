// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Core2D.Shape;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines tool contract.
    /// </summary>
    public abstract class Tool
    {
        /// <summary>
        /// Gets the tool name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public virtual void LeftDown(double x, double y)
        {
        }

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public virtual void LeftUp(double x, double y)
        {
        }

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public virtual void RightDown(double x, double y)
        {
        }

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public virtual void RightUp(double x, double y)
        {
        }

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public virtual void Move(double x, double y)
        {
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.One"/>.
        /// </summary>
        public virtual void ToStateOne()
        {
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.Two"/>.
        /// </summary>
        public virtual void ToStateTwo()
        {
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.Three"/>.
        /// </summary>
        public virtual void ToStateThree()
        {
        }

        /// <summary>
        /// Transfer tool state to <see cref="ToolState.Four"/>.
        /// </summary>
        public virtual void ToStateFour()
        {
        }

        /// <summary>
        /// Move edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public virtual void Move(BaseShape shape)
        {
        }

        /// <summary>
        /// Finalize edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public virtual void Finalize(BaseShape shape)
        {
        }

        /// <summary>
        /// Remove edited shape.
        /// </summary>
        public virtual void Remove()
        {
        }
    }
}
