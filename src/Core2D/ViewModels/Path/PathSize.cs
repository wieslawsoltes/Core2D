using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core2D.Path
{
    public class PathSize : ObservableObject
    {
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set => RaiseAndSetIfChanged(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => RaiseAndSetIfChanged(ref _height, value);
        }

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

        public virtual bool ShouldSerializeWidth() => _width != default;

        public virtual bool ShouldSerializeHeight() => _height != default;
    }
}
