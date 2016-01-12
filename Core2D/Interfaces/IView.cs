// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Defines data context view contract.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets or sets data context object.
        /// </summary>
        object DataContext { get; set; }

        /// <summary>
        /// Closes the view.
        /// </summary>
        void Close();
    }
}
