using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathPolySegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var segment = new Class1();

            var target = ImmutableArray.Create<IPointShape>();
            var actual = segment.ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Points()
        {
            var segment = new Class1();

            var target = ImmutableArray.Create<IPointShape>();
            target = target.Add(_factory.CreatePointShape());
            target = target.Add(_factory.CreatePointShape());
            target = target.Add(_factory.CreatePointShape());

            var actual = segment.ToString(target);

            Assert.Equal("0,0 0,0 0,0", actual);
        }

        public class Class1 : PathPolySegment
        {
            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

        }
    }
}
