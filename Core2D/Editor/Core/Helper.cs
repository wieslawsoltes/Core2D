// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Base class for editor helpers.
    /// </summary>
    public abstract class Helper
    {
        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public abstract void LeftDown(double x, double y);

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public abstract void LeftUp(double x, double y);

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public abstract void RightDown(double x, double y);

        /// <summary>
        ///  Handle mouse right button up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public abstract void RightUp(double x, double y);

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        public abstract void Move(double x, double y);

        /// <summary>
        /// Transfer helper state to <see cref="State.One"/>.
        /// </summary>
        public abstract void ToStateOne();

        /// <summary>
        /// Transfer helper state to <see cref="State.Two"/>.
        /// </summary>
        public abstract void ToStateTwo();

        /// <summary>
        /// Transfer helper state to <see cref="State.Three"/>.
        /// </summary>
        public abstract void ToStateThree();

        /// <summary>
        /// Transfer helper state to <see cref="State.Four"/>.
        /// </summary>
        public abstract void ToStateFour();

        /// <summary>
        /// Move edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public abstract void Move(BaseShape shape);

        /// <summary>
        /// Finalize edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        public abstract void Finalize(BaseShape shape);

        /// <summary>
        /// Remove edited shape.
        /// </summary>
        public abstract void Remove();
    }
}
