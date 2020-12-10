using System;
using Core2D.ViewModels.Shapes;

namespace Core2D.Common.UnitTests
{
    public class TestPointShapeViewModel : PointShapeViewModel
    {
        public new Type TargetType => typeof(TestPointShapeViewModel);
    }
}
