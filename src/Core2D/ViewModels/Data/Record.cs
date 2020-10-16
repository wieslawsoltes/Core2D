using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    public class Record : ObservableObject
    {
        private string _id = "";
        private ImmutableArray<Value> _values;

        public Record()
            : base()
        {
            _id = Guid.NewGuid().ToString();
            _values = ImmutableArray.Create<Value>();
        }

        public string Id
        {
            get => _id;
            set => RaiseAndSetIfChanged(ref _id, value);
        }

        public ImmutableArray<Value> Values
        {
            get => _values;
            set => RaiseAndSetIfChanged(ref _values, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            var values = _values.Copy(shared).ToImmutable();

            return new Record()
            {
                Name = Name,
                Values = values
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var value in Values)
            {
                isDirty |= value.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var value in Values)
            {
                value.Invalidate();
            }
        }

        public virtual bool ShouldSerializeId() => !string.IsNullOrWhiteSpace(_id);

        public virtual bool ShouldSerializeValues() => true;
    }
}
