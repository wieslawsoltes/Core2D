// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;

namespace Core2D.Style
{
    /// <summary>
    /// Define line fixed length contract.
    /// </summary>
    public interface ILineFixedLength : IObservableObject
    {
        /// <summary>
        /// Get or sets line fixed length flags.
        /// </summary>
        LineFixedLengthFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Disabled"/> flag.
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Start"/> flag.
        /// </summary>
        bool Start { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.End"/> flag.
        /// </summary>
        bool End { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Vertical"/> flag.
        /// </summary>
        bool Vertical { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.Horizontal"/> flag.
        /// </summary>
        bool Horizontal { get; set; }

        /// <summary>
        /// Gets or sets <see cref="LineFixedLengthFlags.All"/> flag.
        /// </summary>
        bool All { get; set; }

        /// <summary>
        /// Gets or sets line start point state trigger.
        /// </summary>
        IShapeState StartTrigger { get; set; }

        /// <summary>
        /// Gets or sets line end point state trigger.
        /// </summary>
        IShapeState EndTrigger { get; set; }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        double Length { get; set; }
    }
}
