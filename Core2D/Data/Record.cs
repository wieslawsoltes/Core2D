// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Database record.
    /// </summary>
    [ContentProperty(nameof(Values))]
    public class Record : ObservableObject
    {
        private Guid _id;
        private ImmutableArray<Column> _columns;
        private ImmutableArray<Value> _values;
        private Database _owner;

        /// <summary>
        /// Gets or sets record Id.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { Update(ref _id, value); }
        }

        /// <summary>
        /// Gets or sets record columns.
        /// </summary>
        public ImmutableArray<Column> Columns
        {
            get { return _columns; }
            set { Update(ref _columns, value); }
        }

        /// <summary>
        /// Gets or sets record values.
        /// </summary>
        public ImmutableArray<Value> Values
        {
            get { return _values; }
            set { Update(ref _values, value); }
        }

        /// <summary>
        /// Gets or sets record owner.
        /// </summary>
        public Database Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="columns">The record columns.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static Record Create(Database owner, ImmutableArray<Column> columns, ImmutableArray<Value> values)
        {
            return new Record()
            {
                Id = Guid.NewGuid(),
                Columns = columns,
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="id">The record Id.</param>
        /// <param name="columns">The record columns.</param>
        /// <param name="values">The record values.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static Record Create(Database owner, string id, ImmutableArray<Column> columns, ImmutableArray<Value> values)
        {
            return new Record()
            {
                Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid() : Guid.Parse(id),
                Columns = columns,
                Values = values,
                Owner = owner
            };
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> instance.
        /// </summary>
        /// <param name="owner">The record owner.</param>
        /// <param name="value">The record value.</param>
        /// <returns>The new instance of the <see cref="Record"/> class.</returns>
        public static Record Create(Database owner, string value)
        {
            return new Record()
            {
                Id = Guid.NewGuid(),
                Columns = owner.Columns,
                Values = ImmutableArray.CreateRange(
                    Enumerable.Repeat(
                        Constants.DefaulValue, 
                        owner.Columns.Length).Select(c => Value.Create(c))),
                Owner = owner
            };
        }
    }
}
