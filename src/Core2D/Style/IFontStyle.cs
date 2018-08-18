// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Define font style contract.
    /// </summary>
    public interface IFontStyle : IObservableObject
    {
        /// <summary>
        /// Get or sets font style flags.
        /// </summary>
        FontStyleFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Regular"/> flag.
        /// </summary>
        bool Regular { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Bold"/> flag.
        /// </summary>
        bool Bold { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Italic"/> flag.
        /// </summary>
        bool Italic { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Underline"/> flag.
        /// </summary>
        bool Underline { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Strikeout"/> flag.
        /// </summary>
        bool Strikeout { get; set; }
    }
}
