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
    public class Property : ObservableObject, IProperty
    {
        private string _value;

        /// <inheritdoc/>
        [Content]
        public string Value
        {
            get => _value;
            set => Update(ref _value, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new Property()
            {
                Name = this.Name,
                Value = this.Value
            };
        }

        /// <inheritdoc/>
        public override string ToString() => _value.ToString();

        /// <summary>
        /// Check whether the <see cref="Value"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(_value);
    }
}
