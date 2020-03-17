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
        /// <param name="args">The input arguments.</param>
        void LeftDown(InputArgs args);

        /// <summary>
        /// Handle left up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void LeftUp(InputArgs args);

        /// <summary>
        /// Handle right down events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void RightDown(InputArgs args);

        /// <summary>
        /// Handle right up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void RightUp(InputArgs args);

        /// <summary>
        /// Handle move events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void Move(InputArgs args);

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
