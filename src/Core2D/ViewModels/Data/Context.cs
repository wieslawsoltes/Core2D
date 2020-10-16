using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Data context.
    /// </summary>
    public class Context : ObservableObject
    {
        private ImmutableArray<Property> _properties;
        private Record _record;

        /// <inheritdoc/>
        public ImmutableArray<Property> Properties
        {
            get => _properties;
            set => RaiseAndSetIfChanged(ref _properties, value);
        }

        /// <inheritdoc/>
        public Record Record
        {
            get => _record;
            set => RaiseAndSetIfChanged(ref _record, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            var properties = this._properties.Copy(shared).ToImmutable();

            return new Context()
            {
                Name = this.Name,
                Properties = properties,
                Record = (Record)this.Record.Copy(shared)
            };
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var property in Properties)
            {
                isDirty |= property.IsDirty();
            }

            if (Record != null)
            {
                isDirty |= Record.IsDirty();
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var property in Properties)
            {
                property.Invalidate();
            }

            if (Record != null)
            {
                Record.Invalidate();
            }
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
