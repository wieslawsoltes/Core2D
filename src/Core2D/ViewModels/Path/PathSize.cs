using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core2D.Path
{
    public partial class PathSize : ViewModelBase
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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

        public string ToXamlString()
            => $"{Width.ToString(CultureInfo.InvariantCulture)},{Height.ToString(CultureInfo.InvariantCulture)}";

        public string ToSvgString()
            => $"{Width.ToString(CultureInfo.InvariantCulture)},{Height.ToString(CultureInfo.InvariantCulture)}";
    }
}
