using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Database record.
    /// </summary>
    public class Record : ObservableObject
    {
        private string _id = "";
        private ImmutableArray<Value> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        public Record()
            : base()
        {
            _id = Guid.NewGuid().ToString();
            _values = ImmutableArray.Create<Value>();
        }

        /// <inheritdoc/>
        public string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<Value> Values
        {
            get => _values;
            set => Update(ref _values, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            var values = _values.Copy(shared).ToImmutable();

            return new Record()
            {
                Name = Name,
                Values = values
            };
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var value in Values)
            {
                isDirty |= value.IsDirty();
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var value in Values)
            {
                value.Invalidate();
            }
        }

        /// <summary>
        /// Check whether the <see cref="Id"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeId() => !string.IsNullOrWhiteSpace(_id);

        /// <summary>
        /// Check whether the <see cref="Values"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValues() => true;
    }
}
