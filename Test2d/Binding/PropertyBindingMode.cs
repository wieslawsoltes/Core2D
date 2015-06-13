// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Describes the direction of the data flow in a binding.
    /// </summary>
    public enum PropertyBindingMode
    {
        /// <summary>
        /// Uses the default two-way binding mode.
        /// </summary>
        Default = TwoWay,
        /// <summary>
        /// Updates the binding target only once during initialization.
        /// </summary>
        OneTime = 0,
        /// <summary>
        /// Updates the binding target (target) property when the binding source (source) changes.
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Updates the source property when the target property changes.
        /// </summary>
        OneWayToSource = 2,
        /// <summary>
        /// Causes changes to either the source property or the target property to automatically update the other. 
        /// </summary>
        TwoWay = 3
    }
}
