// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;
using Core2D.Data;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Project
{
    /// <summary>
    /// Container base class.
    /// </summary>
    public class XContainer : XSelectable
    {
        private string _name;
        private double _width;
        private double _height;
        private ArgbColor _background;
        private ImmutableArray<XLayer> _layers;
        private XLayer _currentLayer;
        private XLayer _workingLayer;
        private XLayer _helperLayer;
        private BaseShape _currentShape;
        private XContainer _template;
        private XContext _data;
        private bool _isExpanded = false;

        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        [Name]
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets container width.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Width property is used.
        /// </remarks>
        public double Width
        {
            get { return _template != null ? _template.Width : _width; }
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
            get { return _template != null ? _template.Height : _height; }
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
            get { return _template != null ? _template.Background : _background; }
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
        public ImmutableArray<XLayer> Layers
        {
            get => _layers;
            set => Update(ref _layers, value);
        }

        /// <summary>
        /// Gets or sets current container layer.
        /// </summary>
        public XLayer CurrentLayer
        {
            get => _currentLayer;
            set => Update(ref _currentLayer, value);
        }

        /// <summary>
        /// Gets or sets working container layer.
        /// </summary>
        public XLayer WorkingLayer
        {
            get => _workingLayer;
            set => Update(ref _workingLayer, value);
        }

        /// <summary>
        /// Gets or sets helper container layer.
        /// </summary>
        public XLayer HelperLayer
        {
            get => _helperLayer;
            set => Update(ref _helperLayer, value);
        }

        /// <summary>
        /// Gets or sets current container shape.
        /// </summary>
        public BaseShape CurrentShape
        {
            get => _currentShape;
            set => Update(ref _currentShape, value);
        }

        /// <summary>
        /// Gets or sets container template.
        /// </summary>
        public XContainer Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        /// <summary>
        /// Gets or sets container data.
        /// </summary>
        public XContext Data
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
                        var property = XProperty.Create(_data, name, value);
                        _data.Properties = _data.Properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XContainer"/> class.
        /// </summary>
        public XContainer()
            : base()
        {
            _layers = ImmutableArray.Create<XLayer>();
            _data = new XContext();
        }

        /// <summary>
        /// Set current layer.
        /// </summary>
        /// <param name="layer">The layer instance.</param>
        public void SetCurrentLayer(XLayer layer) => CurrentLayer = layer;

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

        /// <summary>
        /// Creates a new <see cref="XContainer"/> page instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="XContainer"/>.</returns>
        public static XContainer CreatePage(string name = "Page")
        {
            var page = new XContainer()
            {
                Name = name
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(XLayer.Create("Layer1", page));
            builder.Add(XLayer.Create("Layer2", page));
            builder.Add(XLayer.Create("Layer3", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = XLayer.Create("Working", page);
            page.HelperLayer = XLayer.Create("Helper", page);

            return page;
        }

        /// <summary>
        /// Creates a new <see cref="XContainer"/> template instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="XContainer"/>.</returns>
        public static XContainer CreateTemplate(string name = "Template", double width = 840, double height = 600)
        {
            var template = new XContainer()
            {
                Name = name
            };

            template.Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            var builder = template.Layers.ToBuilder();
            builder.Add(XLayer.Create("TemplateLayer1", template));
            builder.Add(XLayer.Create("TemplateLayer2", template));
            builder.Add(XLayer.Create("TemplateLayer3", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = XLayer.Create("TemplateWorking", template);
            template.HelperLayer = XLayer.Create("TemplateHelper", template);

            return template;
        }
    }
}
