using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    public partial class Record : ViewModelBase
    {
        [AutoNotify] private string _id = Guid.NewGuid().ToString();
        [AutoNotify] private ImmutableArray<Value> _values = ImmutableArray.Create<Value>();

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
    }
}
