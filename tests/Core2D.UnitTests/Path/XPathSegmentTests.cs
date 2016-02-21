// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shapes;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var segment = new Class1();

            var target = new List<XPoint>();
            var actual = segment.ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Points()
        {
            var segment = new Class1();

            var target = new List<XPoint>();
            target.Add(new XPoint());
            target.Add(new XPoint());
            target.Add(new XPoint());

            var actual = segment.ToString(target);

            Assert.Equal("0,0 0,0 0,0", actual);
        }

        public class Class1 : XPathSegment
        {
            public override IEnumerable<XPoint> GetPoints()
            {
                throw new NotImplementedException();
            }
        }
    }
}
