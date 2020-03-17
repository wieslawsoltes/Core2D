// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Containers
{
    /// <summary>
    /// Defines generic library contract.
    /// </summary>
    /// <typeparam name="T">The library item type.</typeparam>
    public interface ILibrary<T> : ILibrary
    {
        /// <summary>
        /// Gets or sets a items collection.
        /// </summary>
        ImmutableArray<T> Items { get; set; }

        /// <summary>
        /// Gets or sets currently selected item from <see cref="Items"/> collection.
        /// </summary>
        T Selected { get; set; }

        /// <summary>
        /// Set selected.
        /// </summary>
        /// <param name="item">The item instance.</param>
        void SetSelected(T item);
    }
}
