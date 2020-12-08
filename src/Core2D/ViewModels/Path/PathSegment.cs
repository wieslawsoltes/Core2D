using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    public partial class PathSegment : ViewModelBase
    {
        [AutoNotify] private bool _isStroked;

        protected PathSegment()
        {
        }

        public virtual void GetPoints(IList<PointShape> points)
        {
            throw new NotImplementedException();
        }

        public virtual string ToXamlString()
        {
            throw new NotImplementedException();
        }

        public virtual string ToSvgString()
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
    }
}
