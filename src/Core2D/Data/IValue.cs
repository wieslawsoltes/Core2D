// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Data
{
    /// <summary>
    /// Defines value contract.
    /// </summary>
    public interface IValue : IObservableObject
    {
        /// <summary>
        /// Gets or sets value content.
        /// </summary>
        string Content { get; set; }
    }
}
