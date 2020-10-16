using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Containers
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
    /// Layer container.
    /// </summary>
    public class LayerContainer : BaseContainer
    {
        /// <summary>
        /// Invalidate layer event.
        /// </summary>
        public event InvalidateLayerEventHandler InvalidateLayerHandler;

        private bool _isVisible = true;
        private ImmutableArray<BaseShape> _shapes;

        /// <summary>
        /// Gets or sets flag indicating whether layer is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                RaiseAndSetIfChanged(ref _isVisible, value);
                InvalidateLayer();
            }
        }

        /// <summary>
        /// Gets or sets layer shapes.
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get => _shapes;
            set => RaiseAndSetIfChanged(ref _shapes, value);
        }

        /// <summary>
        /// Invalidate layer shapes.
        /// </summary>
        public void InvalidateLayer() => InvalidateLayerHandler?.Invoke(this, new InvalidateLayerEventArgs());

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var shape in Shapes)
            {
                isDirty |= shape.IsDirty();
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var shape in Shapes)
            {
                shape.Invalidate();
            }
        }

        /// <summary>
        /// Check whether the <see cref="IsVisible"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsVisible() => _isVisible != default;

        /// <summary>
        /// Check whether the <see cref="Shapes"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShapes() => true;
    }
}
