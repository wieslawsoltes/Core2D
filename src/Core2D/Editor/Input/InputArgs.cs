// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Spatial;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input arguments.
    /// </summary>
    public struct InputArgs
    {
        /// <summary>
        /// Gets input position.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Gets input modifier flags.
        /// </summary>
        public ModifierFlags Modifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputArgs"/> struct.
        /// </summary>
        /// <param name="position">The input position.</param>
        /// <param name="modifier">The modifier flags.</param>
        public InputArgs(Vector2 position, ModifierFlags modifier)
        {
            this.Position = position;
            this.Modifier = modifier;
        }
    }
}
