// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;

namespace Core2D.Data.Database
{
    /// <summary>
    /// Database record.
    /// </summary>
    public class XRecord : ObservableObject
    {
        private string _id;
        private ImmutableArray<XColumn> _columns;
        private ImmutableArray<XValue> _values;
        private XDatabase _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="XRecord"/> class.
        /// </summary>
        public XRecord()
            : base()
        {
            _columns = ImmutableArray.Create<XColumn>();
            _values = ImmutableArray.Create<XValue>();
        }

        /// <summary>
        /// Gets or sets record Id.
        /// </summary>
        [Name]
        public string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <summary>
        /// Gets or sets record columns.
        /// </summary>
        public ImmutableArray<XColumn> Columns
        {
            get => _columns;
            set => Update(ref _columns, value);
        }

        /// <summary>
        /// Gets or sets record values.
        /// </summary>
        [Content]
        public ImmutableArray<XValue> Values
        {
            get => _values;
            set => Update(ref _values, value);
        }

        /// <summary>
        /// Gets or sets record owner.
        /// </summary>
        public XDatabase Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <summary>
        /// Creates a new <see cref="XRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="columns">The record columns.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="XRecord"/> class.</returns>
        public static XRecord Create(XDatabase owner, ImmutableArray<XColumn> columns, ImmutableArray<XValue> values)
        {
            return new XRecord()
            {
                Id = Guid.NewGuid().ToString(),
                Columns = columns,
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="XRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="columns">The record columns.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="XRecord"/> class.</returns>
        public static XRecord Create(XDatabase owner, string id, ImmutableArray<XColumn> columns, ImmutableArray<XValue> values)
        {
            return new XRecord()
            {
                Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id,
                Columns = columns,
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="XRecord"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="XRecord"/> class.</returns>
        public static XRecord Create(XDatabase owner, string value)
        {
            return new XRecord()
            {
                Id = Guid.NewGuid().ToString(),
                Columns = owner.Columns,
                Values = ImmutableArray.CreateRange(
                    Enumerable.Repeat(
                        value, 
                        owner.Columns.Length).Select(c => XValue.Create(c))),
                Owner = owner
            };
        }

        /// <summary>
        /// Check whether the <see cref="Id"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeId() => !String.IsNullOrWhiteSpace(_id);

        /// <summary>
        /// Check whether the <see cref="Columns"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeColumns() => _columns.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Values"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeValues() => _values.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeOwner() => _owner != null;
    }
}
