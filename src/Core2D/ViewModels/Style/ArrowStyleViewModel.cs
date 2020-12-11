using System;
using System.Collections.Generic;
using Core2D.Model.Style;

namespace Core2D.ViewModels.Style
{
    public partial class ArrowStyleViewModel : ViewModelBase
    {
        [AutoNotify] private ArrowType _arrowType;
        [AutoNotify] private double _radiusX;
        [AutoNotify] private double _radiusY;

        public ArrowStyleViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArrowStyleViewModel(_serviceProvider)
            {
                Name = this.Name,
                ArrowType = this._arrowType,
                RadiusX = this._radiusX,
                RadiusY = this._radiusY
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }
    }
}
