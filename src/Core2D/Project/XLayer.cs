// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;
using Core2D.Shape;

namespace Core2D.Project
{
    /// <summary>
    /// Invalidate layer event arguments.
    /// </summary>
    public class InvalidateLayerEventArgs : EventArgs { }

    /// <summary>
    /// Invalidate layer event handler.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    /// <summary>
    /// Container layer.
    /// </summary>
    public class XLayer : XSelectable, ICopyable
    {
        /// <summary>
        /// Invalidate layer event.
        /// </summary>
        public event InvalidateLayerEventHandler InvalidateLayer;

        private XContainer _owner;
        private bool _isVisible = true;
        private ImmutableArray<BaseShape> _shapes;

        /// <summary>
        /// Gets or sets layer owner.
        /// </summary>
        public XContainer Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether layer is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                Update(ref _isVisible, value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets layer shapes.
        /// </summary>
        [Content]
        public ImmutableArray<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLayer"/> class.
        /// </summary>
        public XLayer() : base() => _shapes = ImmutableArray.Create<BaseShape>();

        /// <summary>
        /// Invalidate layer shapes.
        /// </summary>
        public void Invalidate() => InvalidateLayer?.Invoke(this, new InvalidateLayerEventArgs());

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="XLayer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="XLayer"/>.</returns>
        public static XLayer Create(string name = "Layer", XContainer owner = null, bool isVisible = true)
        {
            return new XLayer()
            {
                Name = name,
                Owner = owner,
                IsVisible = isVisible
            };
        }

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;

        /// <summary>
        /// Check whether the <see cref="IsVisible"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsVisible() => _isVisible != default(bool);

        /// <summary>
        /// Check whether the <see cref="Shapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShapes() => _shapes.IsEmpty == false;
    }
}
