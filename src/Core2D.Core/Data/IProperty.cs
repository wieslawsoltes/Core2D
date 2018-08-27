// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Data
{
    /// <summary>
    /// Defines property contract.
    /// </summary>
    public interface IProperty : IObservableObject
    {
        /// <summary>
        /// Gets or sets property value.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets property owner.
        /// </summary>
        IContext Owner { get; set; }
    }
}
