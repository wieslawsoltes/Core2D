// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines group shape contract.
    /// </summary>
    public interface IGroupShape : IConnectableShape
    {
        /// <summary>
        /// Gets all properties from <see cref="Shapes"/> collection.
        /// </summary>
        ImmutableArray<IProperty> ShapesProperties { get; }

        /// <summary>
        ///  Gets or sets shapes collection.
        /// </summary>
        ImmutableArray<IBaseShape> Shapes { get; set; }

    }
}
