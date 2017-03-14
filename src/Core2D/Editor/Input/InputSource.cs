// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Spatial;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input source base class.
    /// </summary>
    public abstract class InputSource : IInputSource
    {
        /// <inheritdoc/>
        public virtual IObservable<InputArgs> LeftDown { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<InputArgs> LeftUp { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<InputArgs> RightDown { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<InputArgs> RightUp { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<InputArgs> Move { get; set; }
    }
}
