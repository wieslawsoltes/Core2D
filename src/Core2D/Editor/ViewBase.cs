// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Views.Interfaces;

namespace Core2D.Editor
{
    /// <summary>
    /// View base class.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ViewBase{T}.Context"/> property.</typeparam>
    public abstract class ViewBase<T> : ObservableObject, IView
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public abstract object Context { get; }
    }
}
