// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Defines input target contract.
    /// </summary>
    public interface IInputTarget
    {
        /// <summary>
        /// Handle left down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="modifier">The modifier flags.</param>
        void LeftDown(double x, double y, ModifierFlags modifier);

        /// <summary>
        /// Handle left up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="modifier">The modifier flags.</param>
        void LeftUp(double x, double y, ModifierFlags modifier);

        /// <summary>
        /// Handle right down events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="modifier">The modifier flags.</param>
        void RightDown(double x, double y, ModifierFlags modifier);

        /// <summary>
        /// Handle right up events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="modifier">The modifier flags.</param>
        void RightUp(double x, double y, ModifierFlags modifier);

        /// <summary>
        /// Handle move events.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="modifier">The modifier flags.</param>
        void Move(double x, double y, ModifierFlags modifier);

        /// <summary>
        /// Check if left down action is available.
        /// </summary>
        /// <returns>True if left down action is available.</returns>
        bool IsLeftDownAvailable();

        /// <summary>
        /// Check if left up action is available.
        /// </summary>
        /// <returns>True if left up action is available.</returns>
        bool IsLeftUpAvailable();

        /// <summary>
        /// Check if right down action is available.
        /// </summary>
        /// <returns>True if right down action is available.</returns>
        bool IsRightDownAvailable();

        /// <summary>
        /// Check if right up action is available.
        /// </summary>
        /// <returns>True if right up action is available.</returns>
        bool IsRightUpAvailable();

        /// <summary>
        /// Check if move action is available.
        /// </summary>
        /// <returns>True if move action is available.</returns>
        bool IsMoveAvailable();
    }
}
