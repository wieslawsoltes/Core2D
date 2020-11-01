using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class ArrowStyle : ObservableObject
    {
        private ArrowType _arrowType;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ArrowType ArrowType
        {
            get => _arrowType;
            set => RaiseAndSetIfChanged(ref _arrowType, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsStroked
        {
            get => _isStroked;
            set => RaiseAndSetIfChanged(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsFilled
        {
            get => _isFilled;
            set => RaiseAndSetIfChanged(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double RadiusX
        {
            get => _radiusX;
            set => RaiseAndSetIfChanged(ref _radiusX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double RadiusY
        {
            get => _radiusY;
            set => RaiseAndSetIfChanged(ref _radiusY, value);
        }

        public ArrowStyle() : base()
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArrowStyle()
            {
                Name = this.Name,
                ArrowType = this.ArrowType,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
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
