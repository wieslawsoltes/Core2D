using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;

namespace Core2D.ViewModels.Data
{
    public partial class RecordViewModel : ViewModelBase
    {
        [AutoNotify] private string _id = Guid.NewGuid().ToString();
        [AutoNotify] private ImmutableArray<ValueViewModel> _values = ImmutableArray.Create<ValueViewModel>();

        public RecordViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            var values = _values.Copy(shared).ToImmutable();

            return new RecordViewModel(_serviceProvider)
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
