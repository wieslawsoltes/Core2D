// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Core2D data context.
    /// </summary>
    public class Data : ObservableObject
    {
        private ImmutableArray<Property> _properties;
        private Record _record;

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
                        var property = Property.Create(name, value, this);
                        Properties = Properties.Add(property);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Data"/> instance.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static Data Create(ImmutableArray<Property> properties, Record record)
        {
            return new Data()
            {
                Properties = properties,
                Record = record
            };
        }
    }
}
