#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core2D.ViewModels.Path
{
    public partial class PathSizeViewModel : ViewModelBase
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;

        public PathSizeViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            return new PathSizeViewModel(ServiceProvider)
            {
                Width = _width,
                Height = _height
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public string ToXamlString()
            => $"{_width.ToString(CultureInfo.InvariantCulture)},{Height.ToString(CultureInfo.InvariantCulture)}";

        public string ToSvgString()
            => $"{_height.ToString(CultureInfo.InvariantCulture)},{Height.ToString(CultureInfo.InvariantCulture)}";
    }
}
