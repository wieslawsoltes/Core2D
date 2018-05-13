// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dock.Model
{
    /// <summary>
    /// View base class.
    /// </summary>
    public abstract class ViewBase : ObservableObject, IView
    {
        private ImmutableArray<IViewsWindow> _windows;

        /// <inheritdoc/>
        public ImmutableArray<IViewsWindow> Windows
        {
            get => _panels;
            set => Update(ref _windows, value);
        }

        /// <inheritdoc/>
        public abstract string Title { get; }

        /// <inheritdoc/>
        public abstract object Context { get; }

        /// <inheritdoc/>
        public virtual void ShowWindows()
        {
            foreach (var window in _windows)
            {
                window.Create();
                window.Present();
            }
        }

        /// <inheritdoc/>
        public virtual void CloseWindows()
        {
            foreach (var window in _windows)
            {
                window.Destroy();
            }
        }

        /// <summary>
        /// Check whether the <see cref="Title"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTitle() => true;

        /// <summary>
        /// Check whether the <see cref="Context"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContext() => false;

        /// <summary>
        /// Check whether the <see cref="Windows"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWindows() => _windows.IsEmpty == false;
    }
}
