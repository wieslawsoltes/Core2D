// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Invalidate layer event arguments.
    /// </summary>
    public class InvalidateLayerEventArgs : EventArgs { }

    /// <summary>
    /// Invalidate layer event handler delegate.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    /// <summary>
    /// Container layer.
    /// </summary>
    [ContentProperty(nameof(Shapes))]
    [RuntimeNameProperty(nameof(Name))]
    public class Layer : ObservableResource
    {
        /// <summary>
        /// Invalidate layer event.
        /// </summary>
        public event InvalidateLayerEventHandler InvalidateLayer;

        private string _name;
        private Container _owner;
        private bool _isVisible = true;
        private ImmutableArray<BaseShape> _shapes;

        /// <summary>
        /// Gets or sets layer name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets layer owner.
        /// </summary>
        public Container Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether layer is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Update(ref _isVisible, value); Invalidate(); }
        }

        /// <summary>
        /// Gets or sets layer shapes.
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        public Layer()
            : base()
        {
            _shapes = ImmutableArray.Create<BaseShape>();
        }

        /// <summary>
        /// Invalidate layer shapes.
        /// </summary>
        public void Invalidate()
        {
            var handler = InvalidateLayer;
            if (handler != null)
            {
                handler(this, new InvalidateLayerEventArgs());
            }
        }

        /// <summary>
        /// Creates a new <see cref="Layer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="Layer"/>.</returns>
        public static Layer Create(string name = "Layer", Container owner = null, bool isVisible = true)
        {
            return new Layer()
            {
                Name = name,
                Owner = owner,
                IsVisible = isVisible
            };
        }
    }
}
