// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;

namespace Core2D.Style
{
    /// <summary>
    /// Line fixed length extensions.
    /// </summary>
    public static class LineFixedLengthExtensions
    {
        /// <summary>
        /// Clones line fixed length.
        /// </summary>
        /// <param name="fixedLength">The line fixed length to clone.</param>
        /// <returns>The new instance of the <see cref="LineFixedLength"/> class.</returns>
        public static ILineFixedLength Clone(this ILineFixedLength fixedLength)
        {
            return new LineFixedLength()
            {
                Flags = fixedLength.Flags,
                StartTrigger = fixedLength.StartTrigger.Clone(),
                EndTrigger = fixedLength.EndTrigger.Clone()
            };
        }
    }
}
