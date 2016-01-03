// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// The data context.
    /// </summary>
    public class Data : ObservableObject
    {
        private ImmutableArray<Property> _properties;
        private Record _record;

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        public Data()
            : base()
        {
            _properties = ImmutableArray.Create<Property>();
            _record = null;
        }

        /// <summary>
        /// Gets or sets a collection <see cref="Property"/> that will be used during drawing.
        /// </summary>
        public ImmutableArray<Property> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        /// <summary>
        /// Gets or sets shape data record.
        /// </summary>
        public Record Record
        {
            get { return _record; }
            set { Update(ref _record, value); }
        }

        /// <summary>
        /// Gets or sets property Value using Name as key for Properties array values. If property with the specified key does not exist it is created.
        /// </summary>
        /// <param name="name">The property name value.</param>
        /// <returns>The property Value.</returns>
        public object this[string name]
        {
            get
            {
                var result = _properties.FirstOrDefault(p => p.Name == name);
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
                    var result = _properties.FirstOrDefault(p => p.Name == name);
                    if (result != null)
                    {
                        result.Value = value;
                    }
                    else
                    {
                        var property = Property.Create(this, name, value);
                        Properties = Properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Data"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Data"/> class.</returns>
        public static Data Create()
        {
            return new Data();
        }

        /// <summary>
        /// Creates a new <see cref="Data"/> instance.
        /// </summary>
        /// <param name="record"></param>
        /// <returns>The new instance of the <see cref="Data"/> class.</returns>
        public static Data Create(Record record)
        {
            return new Data()
            {
                Record = record
            };
        }
    }
}
