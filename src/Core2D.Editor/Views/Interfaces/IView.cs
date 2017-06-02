// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Views.Interfaces
{
    /// <summary>
    /// View contract.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets view name.
        /// </summary>
       string Name { get; set; }

        /// <summary>
        /// Gets view context.
        /// </summary>
        object Context { get; set; }
    }
}
