#nullable enable
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

        public override object Copy(IDictionary<object, object>? shared)
        {
            return new ArrowStyleViewModel(ServiceProvider)
            {
                Name = Name,
                ArrowType = _arrowType,
                RadiusX = _radiusX,
                RadiusY = _radiusY
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }
    }
}
