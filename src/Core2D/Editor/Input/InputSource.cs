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
        public virtual IObservable<Vector2> LeftDown { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<Vector2> LeftUp { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<Vector2> RightDown { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<Vector2> RightUp { get; set; }

        /// <inheritdoc/>
        public virtual IObservable<Vector2> Move { get; set; }
    }
}
