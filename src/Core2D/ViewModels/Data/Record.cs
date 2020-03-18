// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Database record.
    /// </summary>
    public class Record : ObservableObject, IRecord
    {
        private ImmutableArray<IValue> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        public Record()
            : base()
        {
            _values = ImmutableArray.Create<IValue>();
        }

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IValue> Values
        {
            get => _values;
            set => Update(ref _values, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            var values = this._values.Copy(shared).ToImmutable();

            return new Record()
            {
                Name = this.Name,
                Values = values
            };
        }

        /// <summary>
        /// Check whether the <see cref="Values"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValues() => true;
    }
}
