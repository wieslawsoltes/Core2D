// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;
using Core2D.Shapes;

namespace Core2D.Containers
{
    /// <summary>
    /// Layer container.
    /// </summary>
    public class LayerContainer : ObservableObject, ILayerContainer
    {
        /// <inheritdoc/>
        public event InvalidateLayerEventHandler InvalidateLayer;

        private IPageContainer _owner;
        private bool _isVisible = true;
        private ImmutableArray<IBaseShape> _shapes;

        /// <inheritdoc/>
        public IPageContainer Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                Update(ref _isVisible, value);
                Invalidate();
            }
        }

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerContainer"/> class.
        /// </summary>
        public LayerContainer() : base() => _shapes = ImmutableArray.Create<IBaseShape>();

        /// <inheritdoc/>
        public void Invalidate() => InvalidateLayer?.Invoke(this, new InvalidateLayerEventArgs());

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="LayerContainer"/> instance.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="owner">The layer owner.</param>
        /// <param name="isVisible">The flag indicating whether layer is visible.</param>
        /// <returns>The new instance of the <see cref="LayerContainer"/>.</returns>
        public static LayerContainer Create(string name = "Layer", IPageContainer owner = null, bool isVisible = true)
        {
            return new LayerContainer()
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
        public virtual bool ShouldSerializeIsVisible() => _isVisible != default;

        /// <summary>
        /// Check whether the <see cref="Shapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShapes() => _shapes.IsEmpty == false;
    }
}
