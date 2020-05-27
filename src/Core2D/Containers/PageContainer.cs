using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    /// <summary>
    /// Page container.
    /// </summary>
    public class PageContainer : ObservableObject, IPageContainer
    {
        private double _width;
        private double _height;
        private IColor _background;
        private ImmutableArray<ILayerContainer> _layers;
        private ILayerContainer _currentLayer;
        private ILayerContainer _workingLayer;
        private ILayerContainer _helperLayer;
        private IBaseShape _currentShape;
        private IPageContainer _template;
        private IContext _data;
        private bool _isExpanded = false;

        /// <inheritdoc/>
        public double Width
        {
            get => _template != null ? _template.Width : _width;
            set
            {
                if (_template != null)
                {
                    _template.Width = value;
                    Notify();
                }
                else
                {
                    Update(ref _width, value);
                }
            }
        }

        /// <inheritdoc/>
        public double Height
        {
            get => _template != null ? _template.Height : _height;
            set
            {
                if (_template != null)
                {
                    _template.Height = value;
                    Notify();
                }
                else
                {
                    Update(ref _height, value);
                }
            }
        }

        /// <inheritdoc/>
        public IColor Background
        {
            get => _template != null ? _template.Background : _background;
            set
            {
                if (_template != null)
                {
                    _template.Background = value;
                    Notify();
                }
                else
                {
                    Update(ref _background, value);
                }
            }
        }

        /// <inheritdoc/>
        public ImmutableArray<ILayerContainer> Layers
        {
            get => _layers;
            set => Update(ref _layers, value);
        }

        /// <inheritdoc/>
        public ILayerContainer CurrentLayer
        {
            get => _currentLayer;
            set => Update(ref _currentLayer, value);
        }

        /// <inheritdoc/>
        public ILayerContainer WorkingLayer
        {
            get => _workingLayer;
            set => Update(ref _workingLayer, value);
        }

        /// <inheritdoc/>
        public ILayerContainer HelperLayer
        {
            get => _helperLayer;
            set => Update(ref _helperLayer, value);
        }

        /// <inheritdoc/>
        public IBaseShape CurrentShape
        {
            get => _currentShape;
            set => Update(ref _currentShape, value);
        }

        /// <inheritdoc/>
        public IPageContainer Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        /// <inheritdoc/>
        public IContext Data
        {
            get => _data;
            set => Update(ref _data, value);
        }

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Update(ref _isExpanded, value);
        }

        /// <inheritdoc/>
        public void SetCurrentLayer(ILayerContainer layer) => CurrentLayer = layer;

        /// <inheritdoc/>
        public virtual void InvalidateLayer()
        {
            if (Template != null)
            {
                Template.InvalidateLayer();
            }

            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.InvalidateLayer();
                }
            }

            if (WorkingLayer != null)
            {
                WorkingLayer.InvalidateLayer();
            }

            if (HelperLayer != null)
            {
                HelperLayer.InvalidateLayer();
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Background.IsDirty();

            foreach (var layer in Layers)
            {
                isDirty |= layer.IsDirty();
            }

            isDirty |= WorkingLayer.IsDirty();
            isDirty |= HelperLayer.IsDirty();
            isDirty |= Template.IsDirty();
            isDirty |= Data.IsDirty();

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            Background.Invalidate();

            foreach (var layer in Layers)
            {
                layer.Invalidate();
            }

            WorkingLayer.Invalidate();
            HelperLayer.Invalidate();
            Template.Invalidate();
            Data.Invalidate();
        }

        /// <summary>
        /// Check whether the <see cref="Width"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWidth() => _width != default;

        /// <summary>
        /// Check whether the <see cref="Height"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHeight() => _height != default;

        /// <summary>
        /// Check whether the <see cref="Background"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeBackground() => _background != null;

        /// <summary>
        /// Check whether the <see cref="Layers"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLayers() => true;

        /// <summary>
        /// Check whether the <see cref="CurrentLayer"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentLayer() => _currentLayer != null;

        /// <summary>
        /// Check whether the <see cref="WorkingLayer"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWorkingLayer() => _workingLayer != null;

        /// <summary>
        /// Check whether the <see cref="HelperLayer"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHelperLayer() => _helperLayer != null;

        /// <summary>
        /// Check whether the <see cref="CurrentShape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentShape() => _currentShape != null;

        /// <summary>
        /// Check whether the <see cref="Template"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTemplate() => _template != null;

        /// <summary>
        /// Check whether the <see cref="Data"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeData() => _data != null;

        /// <summary>
        /// Check whether the <see cref="IsExpanded"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsExpanded() => _isExpanded != default;
    }
}
