// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Defines context contract.
    /// </summary>
    public interface IContext : IObservableObject
    {
        /// <summary>
        /// Gets or sets a collection <see cref="IProperty"/> that will be used during drawing.
        /// </summary>
        ImmutableArray<IProperty> Properties { get; set; }

        /// <summary>
        /// Gets or sets shape data record.
        /// </summary>
        IRecord Record { get; set; }
    }
}
