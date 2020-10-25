using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Shapes;

namespace Core2D.Path
{
    [DataContract(IsReference = true)]
    public abstract class PathSegment : ObservableObject
    {
        private bool _isStroked;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsStroked
        {
            get => _isStroked;
            set => RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public abstract void GetPoints(IList<PointShape> points);

        public abstract string ToXamlString();

        public abstract string ToSvgString();

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
