// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// The page layers container.
    /// </summary>
    public class Page : Container
    {
        private Data _data;

        /// <summary>
        /// Gets or sets page data.
        /// </summary>
        public Data Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        /// <summary>
        /// Gets or sets property Value using Name as key for data Properties array values. 
        /// </summary>
        /// <remarks>If property with the specified key does not exist it is created.</remarks>
        /// <param name="name">The property name value.</param>
        /// <returns>The property value.</returns>
        public object this[string name]
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
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        public Page()
            : base()
        {
            Data = new Data();
        }

        /// <summary>
        /// Creates a new <see cref="Page"/> instance.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <returns>The new instance of the <see cref="Page"/>.</returns>
        public static Page Create(string name = "Page")
        {
            var page = new Page()
            {
                Name = name
            };

            var builder = page.Layers.ToBuilder();
            builder.Add(Layer.Create("Layer1", page));
            builder.Add(Layer.Create("Layer2", page));
            builder.Add(Layer.Create("Layer3", page));
            page.Layers = builder.ToImmutable();

            page.CurrentLayer = page.Layers.FirstOrDefault();
            page.WorkingLayer = Layer.Create("Working", page);
            page.HelperLayer = Layer.Create("Helper", page);

            return page;
        }
    }
}
