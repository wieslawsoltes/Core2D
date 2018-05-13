// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dock.Model
{
    /// <summary>
    /// Grid panel contract.
    /// </summary>
    public interface IGridPanel
    {
        /// <summary>
        /// Gets or sets row.
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// Gets or sets column.
        /// </summary>
        int Column { get; set; }
    }
}
