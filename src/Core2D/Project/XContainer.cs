// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Core2D.Style;
using Portable.Xaml.Markup;
using System.Collections.Immutable;

namespace Core2D.Project
{
    /// <summary>
    /// Container base class.
    /// </summary>
    [ContentProperty(nameof(Layers))]
    [RuntimeNameProperty(nameof(Name))]
    public abstract class XContainer : XSelectable
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

        /// <summary>
        /// Gets or sets container name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets container width.
        /// </summary>
        public double Width
        {
            get { return _template != null ? _template.Width : _width; }
            set { Update(ref _width, value); }
        }

        /// <summary>
        /// Gets or sets container height.
        /// </summary>
        public double Height
        {
            get { return _template != null ? _template.Height : _height; }
            set { Update(ref _height, value); }
        }

        /// <summary>
        /// Gets or sets container background color.
        /// </summary>
        public ArgbColor Background
        {
            get { return _template != null ? _template.Background : _background; }
            set { Update(ref _background, value); }
        }

        /// <summary>
        /// Gets or sets container layers.
        /// </summary>
        public ImmutableArray<XLayer> Layers
        {
            get { return _layers; }
            set { Update(ref _layers, value); }
        }

        /// <summary>
        /// Gets or sets current container layer.
        /// </summary>
        public XLayer CurrentLayer
        {
            get { return _currentLayer; }
            set { Update(ref _currentLayer, value); }
        }

        /// <summary>
        /// Gets or sets working container layer.
        /// </summary>
        public XLayer WorkingLayer
        {
            get { return _workingLayer; }
            set { Update(ref _workingLayer, value); }
        }

        /// <summary>
        /// Gets or sets helper container layer.
        /// </summary>
        public XLayer HelperLayer
        {
            get { return _helperLayer; }
            set { Update(ref _helperLayer, value); }
        }

        /// <summary>
        /// Gets or sets current container shape.
        /// </summary>
        public BaseShape CurrentShape
        {
            get { return _currentShape; }
            set { Update(ref _currentShape, value); }
        }

        /// <summary>
        /// Gets or sets container template.
        /// </summary>
        public XContainer Template
        {
            get { return _template; }
            set { Update(ref _template, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XContainer"/> class.
        /// </summary>
        public XContainer()
            : base()
        {
            _layers = ImmutableArray.Create<XLayer>();
        }

        /// <summary>
        /// Set current layer.
        /// </summary>
        /// <param name="layer">The layer instance.</param>
        public void SetCurrentLayer(XLayer layer)
        {
            CurrentLayer = layer;
        }

        /// <summary>
        /// Invalidate container layers.
        /// </summary>
        public virtual void Invalidate()
        {
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
    }
}
