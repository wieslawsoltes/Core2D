using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Core2D.Path
{
    [DataContract(IsReference = true)]
    public class PathSize : ObservableObject
    {
        private double _width;
        private double _height;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double Width
        {
            get => _width;
            set => RaiseAndSetIfChanged(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
