// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Data context.
    /// </summary>
    public class Context : ObservableObject
    {
        private ImmutableArray<Property> _properties;
        private Record _record;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context() : base() => _properties = ImmutableArray.Create<Property>();

        /// <summary>
        /// Gets or sets a collection <see cref="Property"/> that will be used during drawing.
        /// </summary>
        [Content]
        public ImmutableArray<Property> Properties
        {
            get => _properties;
            set => Update(ref _properties, value);
        }

        /// <summary>
        /// Gets or sets shape data record.
        /// </summary>
        public Record Record
        {
            get => _record;
            set => Update(ref _record, value);
        }

        /// <summary>
        /// Gets or sets <see cref="Property.Value"/> using name as key for <see cref="Properties"/> array values.
        /// </summary>
        /// <remarks>
        /// If property with the specified key does not exist it is created.
        /// </remarks>
        /// <param name="name">The property value.</param>
        /// <returns>The property value.</returns>
        public string this[string name]
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

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        public static Context Create() => new Context();

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        /// <param name="record">The record instance.</param>
        /// <returns>The new instance of the <see cref="Context"/> class.</returns>
        public static Context Create(Record record) => new Context() { Record = record };

        /// <summary>
        /// Check whether the <see cref="Properties"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeProperties() => _properties.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Record"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRecord() => _record != null;
    }
}
