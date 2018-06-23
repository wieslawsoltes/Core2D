// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines editor layout platform contract.
    /// </summary>
    public interface IEditorLayoutPlatform
    {
        /// <summary>
        /// Gets or sets load layout action.
        /// </summary>
        /// <remarks>Loads layout configuration.</remarks>
        Action LoadLayout { get; set; }

        /// <summary>
        /// Gets or sets save layout action.
        /// </summary>
        /// <remarks>Saves layout configuration.</remarks>
        Action SaveLayout { get; set; }

        /// <summary>
        /// Gets or sets reset layout action.
        /// </summary>
        /// <remarks>Resets layout configuration.</remarks>
        Action ResetLayout { get; set; }
    }
}
