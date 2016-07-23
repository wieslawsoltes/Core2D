// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Path;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathPolySegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var segment = new Class1();

            var target = ImmutableArray.Create<XPoint>();
            var actual = segment.ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Points()
        {
            var segment = new Class1();

            var target = ImmutableArray.Create<XPoint>();
            target = target.Add(new XPoint());
            target = target.Add(new XPoint());
            target = target.Add(new XPoint());

            var actual = segment.ToString(target);

            Assert.Equal("0,0 0,0 0,0", actual);
        }

        public class Class1 : XPathPolySegment
        {
        }
    }
}
