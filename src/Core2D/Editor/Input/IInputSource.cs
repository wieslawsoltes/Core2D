// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Spatial;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Defines input source contract.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Left down events.
        /// </summary>
        IObservable<Vector2> LeftDown { get; set; }

        /// <summary>
        /// Left up events.
        /// </summary>
        IObservable<Vector2> LeftUp { get; set; }

        /// <summary>
        /// Right down events.
        /// </summary>
        IObservable<Vector2> RightDown { get; set; }

        /// <summary>
        /// Right up events.
        /// </summary>
        IObservable<Vector2> RightUp { get; set; }

        /// <summary>
        /// Move events.
        /// </summary>
        IObservable<Vector2> Move { get; set; }
    }
}
