using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core2D.ViewModels.Path
{
    public partial class PathSizeViewModel : ViewModelBase
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;

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
