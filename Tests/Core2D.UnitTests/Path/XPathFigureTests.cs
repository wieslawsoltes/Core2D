// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathFigureTests
    {
        [Fact]
        [Trait("Core2D", "Path")]
        public void StartPoint_Not_Null()
        {
            var target = new XPathFigure();
            Assert.NotNull(target.StartPoint);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void Segments_Not_Null()
        {
            var target = new XPathFigure();
            Assert.NotNull(target.Segments);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void IsFilled_By_Default_Is_False()
        {
            var target = new XPathFigure();
            Assert.False(target.IsFilled);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void IsClosed_By_Default_Is_False()
        {
            var target = new XPathFigure();
            Assert.False(target.IsClosed);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var figure = new XPathFigure();

            var segment1 = new Class1() { Point = new XPoint() };
            figure.Segments.Add(segment1);

            var segment2 = new Class1() { Point = new XPoint() };
            figure.Segments.Add(segment2);

            var target = figure.GetPoints();

            Assert.Equal(3, target.Count());

            Assert.Contains(figure.StartPoint, target);
            Assert.Contains(segment1.Point, target);
            Assert.Contains(segment2.Point, target);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void ToString_Should_Path_Markup_Empty_Not_Closed()
        {
            var target = new XPathFigure();

            var actual = target.ToString();

            Assert.Equal("M0,0", actual);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void ToString_Should_Path_Markup_Empty_Closed()
        {
            var target = new XPathFigure() { IsClosed = true };

            var actual = target.ToString();

            Assert.Equal("M0,0z", actual);
        }

        public class Class1 : XPathSegment
        {
            public XPoint Point { get; set; }

            public override IEnumerable<XPoint> GetPoints()
            {
                yield return Point;
            }
        }
    }
}
