// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Data
{
    /// <summary>
    /// Defines column contract.
    /// </summary>
    public interface IColumn : IObservableObject
    {
        /// <summary>
        /// Gets or sets column display width.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether column is visible.
        /// </summary>
        bool IsVisible { get; set; }
    }
}
