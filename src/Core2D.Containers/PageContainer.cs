// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;
using Core2D.Containers.Interfaces;
using Core2D.Data;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Containers
{
    /// <summary>
    /// Page container.
    /// </summary>
    public class PageContainer : SelectableObject, IPageContainer
    {
        private double _width;
        private double _height;
        private ArgbColor _background;
        private ImmutableArray<ILayerContainer> _layers;
        private ILayerContainer _currentLayer;
        private ILayerContainer _workingLayer;
        private ILayerContainer _helperLayer;
        private IBaseShape _currentShape;
        private IPageContainer _template;
        private Context _data;
        private bool _isExpanded = false;

        /// <summary>
        /// Gets or sets container width.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Width property is used.
        /// </remarks>
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

        /// <summary>
        /// Gets or sets container height.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Height property is used.
        /// </remarks>
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

        /// <summary>
        /// Gets or sets container background color.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Background property is used.
        /// </remarks>
        public ArgbColor Background
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

        /// <summary>
        /// Gets or sets container layers.
        /// </summary>
        [Content]
        public ImmutableArray<ILayerContainer> Layers
        {
            get => _layers;
            set => Update(ref _layers, value);
        }

        /// <summary>
        /// Gets or sets current container layer.
        /// </summary>
        public ILayerContainer CurrentLayer
        {
            get => _currentLayer;
            set => Update(ref _currentLayer, value);
        }

        /// <summary>
        /// Gets or sets working container layer.
        /// </summary>
        public ILayerContainer WorkingLayer
        {
            get => _workingLayer;
            set => Update(ref _workingLayer, value);
        }

        /// <summary>
        /// Gets or sets helper container layer.
        /// </summary>
        public ILayerContainer HelperLayer
        {
            get => _helperLayer;
            set => Update(ref _helperLayer, value);
        }

        /// <summary>
        /// Gets or sets current container shape.
        /// </summary>
        public IBaseShape CurrentShape
        {
            get => _currentShape;
            set => Update(ref _currentShape, value);
        }

        /// <summary>
        /// Gets or sets container template.
        /// </summary>
        public IPageContainer Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        /// <summary>
        /// Gets or sets container data.
        /// </summary>
        public Context Data
        {
            get => _data;
            set => Update(ref _data, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether container is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Update(ref _isExpanded, value);
        }

        /// <summary>
        /// Gets or sets property Value using Name as key for data Properties array values. 
        /// </summary>
        /// <remarks>If property with the specified key does not exist it is created.</remarks>
        /// <param name="name">The property name value.</param>
        /// <returns>The property value.</returns>
        public string this[string name]
        {
            get
            {
                var result = _data.Properties.FirstOrDefault(p => p.Name == name);
                if (result != null)
                {
                    return result.Value;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    var result = _data.Properties.FirstOrDefault(p => p.Name == name);
                    if (result != null)
                    {
                        result.Value = value;
                    }
                    else
                    {
                        var property = Property.Create(_data, name, value);
                        _data.Properties = _data.Properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContainer"/> class.
        /// </summary>
        public PageContainer()
            : base()
        {
            _layers = ImmutableArray.Create<ILayerContainer>();
            _data = new Context();
        }

        /// <summary>
        /// Set current layer.
        /// </summary>
        /// <param name="layer">The layer instance.</param>
        public void SetCurrentLayer(ILayerContainer layer) => CurrentLayer = layer;

        /// <summary>
        /// Invalidate container layers.
        /// </summary>
        public virtual void Invalidate()
        {
            if (Template != null)
            {
                Template.Invalidate();
            }

            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.Invalidate();
                }
            }

            if (WorkingLayer != null)
            {
                WorkingLayer.Invalidate();
            }

            if (HelperLayer != null)
            {
                HelperLayer.Invalidate();
            }
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> page instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        public static PageContainer CreatePage(string name = "Page")
        {
            var page = new PageContainer()
            {
                Name = name
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(LayerContainer.Create("Layer1", page));
            builder.Add(LayerContainer.Create("Layer2", page));
            builder.Add(LayerContainer.Create("Layer3", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = LayerContainer.Create("Working", page);
            page.HelperLayer = LayerContainer.Create("Helper", page);

            return page;
        }

        /// <summary>
        /// Creates a new <see cref="PageContainer"/> template instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        public static PageContainer CreateTemplate(string name = "Template", double width = 840, double height = 600)
        {
            var template = new PageContainer()
            {
                Name = name
            };

            template.Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            var builder = template.Layers.ToBuilder();
            builder.Add(LayerContainer.Create("TemplateLayer1", template));
            builder.Add(LayerContainer.Create("TemplateLayer2", template));
            builder.Add(LayerContainer.Create("TemplateLayer3", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = LayerContainer.Create("TemplateWorking", template);
            template.HelperLayer = LayerContainer.Create("TemplateHelper", template);

            return template;
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
        public virtual bool ShouldSerializeLayers() => _layers.IsEmpty == false;

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
