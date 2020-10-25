using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Core2D.Data
{
    [DataContract(IsReference = true)]
    public class Context : ObservableObject
    {
        private ImmutableArray<Property> _properties;
        private Record _record;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ImmutableArray<Property> Properties
        {
            get => _properties;
            set => RaiseAndSetIfChanged(ref _properties, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public Record Record
        {
            get => _record;
            set => RaiseAndSetIfChanged(ref _record, value);
        }

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
    }
}
