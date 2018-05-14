// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock container contract.
    /// </summary>
    public interface IDockContainer
    {
        /// <summary>
        /// Gets or sets row.
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// Gets or sets column.
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets or sets views.
        /// </summary>
        IList<IDockView> Views { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IDockView CurrentView { get; set; }
    }
}
