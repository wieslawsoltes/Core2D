// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dock.Model
{
    /// <summary>
    /// Dock window contract.
    /// </summary>
    public interface IDockWindow
    {
        /// <summary>
        /// Presents window.
        /// </summary>
        void Present();

        /// <summary>
        /// Destroys window.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Sets window position.
        /// </summary>
        /// <param name="x">The X coordinate of window.</param>
        /// <param name="y">The Y coordinate of window.</param>
        void SetPosition(double x, double y);

        /// <summary>
        /// Gets window position.
        /// </summary>
        /// <param name="x">The X coordinate of window.</param>
        /// <param name="y">The Y coordinate of window.</param>
        void GetPosition(ref double x, ref double y);

        /// <summary>
        /// Sets window size.
        /// </summary>
        /// <param name="x">The window width.</param>
        /// <param name="y">The window height.</param>
        void SetSize(double width, double height);

        /// <summary>
        /// Gets window size.
        /// </summary>
        /// <param name="x">The window width.</param>
        /// <param name="y">The window height.</param>
        void GetSize(ref double width, ref double height);

        /// <summary>
        /// Sets window title.
        /// </summary>
        /// <param name="context">The window title.</param>
        void SetTitle(string title);

        /// <summary>
        /// Sets window context.
        /// </summary>
        /// <param name="context">The window context.</param>
        void SetContext(object context);
    }
}
