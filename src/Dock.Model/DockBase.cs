// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock base class.
    /// </summary>
    public abstract class DockBase : ObservableObject, IDock
    {
        private string _dock;
        private IList<IDockWindow> _windows;

        /// <inheritdoc/>
        public string Dock
        {
            get => _dock;
            set => Update(ref _dock, value);
        }

        /// <inheritdoc/>
        public abstract string Title { get; }

        /// <inheritdoc/>
        public abstract object Context { get; }

        /// <inheritdoc/>
        public IList<IDockWindow> Windows
        {
            get => _windows;
            set => Update(ref _windows, value);
        }

        /// <inheritdoc/>
        public virtual void ShowWindows()
        {
            if (_windows != null)
            {
                foreach (var window in _windows)
                {
                    window.Present();
                }
            }
        }

        /// <inheritdoc/>
        public virtual void CloseWindows()
        {
            if (_windows != null)
            {
                foreach (var window in _windows)
                {
                    window.Destroy();
                }
            }
        }

        /// <inheritdoc/>
        public virtual void AddWindow(IDockWindow window)
        {
            _windows?.Add(window);
        }

        /// <inheritdoc/>
        public virtual void RemoveWindow(IDockWindow window)
        {
            _windows?.Remove(window);
        }

        /// <summary>
        /// Check whether the <see cref="Dock"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDock() => true;

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
        public virtual bool ShouldSerializeWindows() => _windows != null;
    }
}
