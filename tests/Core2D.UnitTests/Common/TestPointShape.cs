using System;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D.Common.UnitTests
{
    public class TestPointShape : TestBaseShape, PointShape
    {
        public override Type TargetType => typeof(TestPointShape);
        public double X { get; set; }
        public double Y { get; set; }
        public PointAlignment Alignment { get; set; }
        public BaseShape Shape { get; set; }
        public string ToXamlString() => throw new NotImplementedException();
        public string ToSvgString() => throw new NotImplementedException();
    }
}
