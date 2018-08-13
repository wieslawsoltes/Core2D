// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path geometry contract.
    /// </summary>
    public interface IPathGeometry : IObservableObject, ICopyable
    {
        /// <summary>
        /// Gets or sets figures collection.
        /// </summary>
        ImmutableArray<IPathFigure> Figures { get; set; }

        /// <summary>
        /// Gets or sets fill rule.
        /// </summary>
        FillRule FillRule { get; set; }
    }
}
