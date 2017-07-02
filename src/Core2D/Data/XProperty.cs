// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Data property.
    /// </summary>
    public class XProperty : ObservableObject, ICopyable
    {
        private string _value;
        private XContext _owner;

        /// <summary>
        /// Gets or sets property value.
        /// </summary>
        [Content]
        public string Value
        {
            get => _value;
            set => Update(ref _value, value);
        }

        /// <summary>
        /// Gets or sets property owner.
        /// </summary>
        public XContext Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="XProperty"/> instance.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The new instance of the <see cref="XProperty"/> class.</returns>
        public static XProperty Create(XContext owner, string name, string value)
        {
            return new XProperty()
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        /// <inheritdoc/>
        public override string ToString() => _value.ToString();

        /// <summary>
        /// Check whether the <see cref="Value"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValue() => !String.IsNullOrWhiteSpace(_value);

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;
    }
}
