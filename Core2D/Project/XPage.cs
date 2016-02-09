// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using System.Linq;

namespace Core2D.Project
{
    /// <summary>
    /// Page container.
    /// </summary>
    public sealed class XPage : XContainer
    {
        private XContext _data;
        private bool _isExpanded = false;

        /// <summary>
        /// Gets or sets page data.
        /// </summary>
        public XContext Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether page is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Update(ref _isExpanded, value); }
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
        /// Invalidate page layers.
        /// </summary>
        public override void Invalidate()
        {
            if (Template != null)
            {
                Template.Invalidate();
            }

            base.Invalidate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPage"/> class.
        /// </summary>
        public XPage()
            : base()
        {
            Data = new XContext();
        }

        /// <summary>
        /// Creates a new <see cref="XPage"/> instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="XPage"/>.</returns>
        public static XPage Create(string name = "Page")
        {
            var page = new XPage()
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
    }
}
