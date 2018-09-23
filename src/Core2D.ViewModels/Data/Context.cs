// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Data context.
    /// </summary>
    public class Context : ObservableObject, IContext
    {
        private ImmutableArray<IProperty> _properties;
        private IRecord _record;

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IProperty> Properties
        {
            get => _properties;
            set => Update(ref _properties, value);
        }

        /// <inheritdoc/>
        public IRecord Record
        {
            get => _record;
            set => Update(ref _record, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Properties"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeProperties() => true;

        /// <summary>
        /// Check whether the <see cref="Record"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRecord() => _record != null;
    }
}
