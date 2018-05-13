// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dock.Model
{
    /// <summary>
    /// Views window.
    /// </summary>
    public class ViewsWindow : ObservableObject, IViewsWindow
    {
        private readonly IServiceProvider _serviceProvider;
        private IDockWindow _window;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _title;
        private object _context;
        private IViewsLayout _layout;

        /// <inheritdoc/>
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        /// <inheritdoc/>
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        /// <inheritdoc/>
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        /// <inheritdoc/>
        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        /// <inheritdoc/>
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        /// <inheritdoc/>
        public object Context
        {
            get => _context;
            set => Update(ref _context, value);
        }

        /// <inheritdoc/>
        public IViewsLayout Layout
        {
            get => _layout;
            set => Update(ref _layout, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ViewsWindow"/> class.
        /// </summary>
        /// <param name="window">The dock window.</param>
        public ViewsWindow(IDockWindow window) : base()
        {
            _window = window;
        }

        /// <inheritdoc/>
        public void Present()
        {
            _window?.SetPosition(_x, _y);
            _window?.SetSize(_width, _height);
            _window?.SetTitle(_title);
            _window?.SetContext(_context);
            _window?.Present();
        }

        /// <inheritdoc/>
        public void Destroy()
        {
            _window?.GetPosition(ref _x, ref _y);
            _window?.GetSize(ref _width, ref _height);
            _window?.Destroy();
        }

        /// <summary>
        /// Check whether the <see cref="X"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeX() => true;

        /// <summary>
        /// Check whether the <see cref="Y"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeY() => true;

        /// <summary>
        /// Check whether the <see cref="Width"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWidth() => true;

        /// <summary>
        /// Check whether the <see cref="Height"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHeight() => true;

        /// <summary>
        /// Check whether the <see cref="Title"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTitle() => _title != null;

        /// <summary>
        /// Check whether the <see cref="Context"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContext() => false;

        /// <summary>
        /// Check whether the <see cref="Layout"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLayout() => _layout != null;
    }
}
