using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class ArrowStyle : ViewModelBase
    {
        [AutoNotify] private ArrowType _arrowType;
        [AutoNotify] private double _radiusX;
        [AutoNotify] private double _radiusY;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArrowStyle()
            {
                Name = this.Name,
                ArrowType = this.ArrowType,
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY
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
