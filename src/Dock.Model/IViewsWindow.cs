// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dock.Model
{
    /// <summary>
    /// Window contract.
    /// </summary>
    public interface IViewsWindow
    {
        /// <summary>
        /// Gets or sets window X coordinate.
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Gets or sets window X coordinate.
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// Gets or sets window width.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets window height.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Gets or sets window title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets window context.
        /// </summary>
        object Context { get; set; }

        /// <summary>
        /// Gets or sets views layout.
        /// </summary>
        IViewsLayout Layout { get; set; }

        /// <summary>
        /// Gets or sets dock window.
        /// </summary>
        IDockWindow Window { get; set; }

        /// <summary>
        /// Presents window.
        /// </summary>
        void Present();

        /// <summary>
        /// Destroys window.
        /// </summary>
        void Destroy();
    }
}
