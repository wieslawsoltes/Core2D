// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Specifies shape state flags.
    /// </summary>
    [Flags]
    public enum ShapeStateFlags
    {
        /// <summary>
        /// Default shape state.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Shape is visible.
        /// </summary>
        Visible = 1,

        /// <summary>
        /// Shape is printable.
        /// </summary>
        Printable = 2,

        /// <summary>
        /// Shape position is locked.
        /// </summary>
        Locked = 4,

        /// <summary>
        /// Shape is connector.
        /// </summary>
        Connector = 8,

        /// <summary>
        /// Shape in none.
        /// </summary>
        None = 16,

        /// <summary>
        /// Shape is standalone.
        /// </summary>
        Standalone = 32,

        /// <summary>
        /// Shape is an input.
        /// </summary>
        Input = 64,

        /// <summary>
        /// Shape is and output.
        /// </summary>
        Output = 128
    }
}
