// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Shape state flags.
    /// </summary>
    [Flags]
    public enum ShapeStateFlags
    {
        /// <summary>
        /// Default state flag.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Visible state flag.
        /// </summary>
        Visible = 1,
        /// <summary>
        /// Printable state flag.
        /// </summary>
        Printable = 2,
        /// <summary>
        /// Locked state flag.
        /// </summary>
        Locked = 4,
        /// <summary>
        /// Connector state flag.
        /// </summary>
        Connector = 8,
        /// <summary>
        /// None state flag.
        /// </summary>
        None = 16,
        /// <summary>
        /// Standalone state flag.
        /// </summary>
        Standalone = 32,
        /// <summary>
        /// Input state flag.
        /// </summary>
        Input = 64,
        /// <summary>
        /// Output state flag.
        /// </summary>
        Output = 128
    }
}
