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
    /// Database record.
    /// </summary>
    public class Record : ObservableObject, IRecord
    {
        private ImmutableArray<IValue> _values;
        private IDatabase _owner;

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
        public IDatabase Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord Create(IDatabase owner, ImmutableArray<IValue> values)
        {
            return new Record()
            {
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord Create(IDatabase owner, string id, ImmutableArray<IValue> values)
        {
            var record = new Record()
            {
                Values = values,
                Owner = owner
            };

            if (!string.IsNullOrWhiteSpace(id))
            {
                record.Id = id;
            }

            return record;
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static IRecord Create(IDatabase owner, string value)
        {
            return new Record()
            {
                Values = ImmutableArray.CreateRange(
                    Enumerable.Repeat(
                        value, 
                        owner.Columns.Length).Select(c => Value.Create(c))),
                Owner = owner
            };
        }

        /// <summary>
        /// Check whether the <see cref="Values"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValues() => _values.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;
    }
}
