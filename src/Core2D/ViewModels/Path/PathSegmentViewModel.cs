using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path
{
    public partial class PathSegmentViewModel : ViewModelBase
    {
        [AutoNotify] private bool _isStroked;

        public PathSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual void GetPoints(IList<PointShapeViewModel> points)
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
    }
}
