// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathGeometryContextTests
    {
        [Fact]
        [Trait("Core2D", "Path")]
        public void Inherits_From_XGeometryContext()
        {
            var target = new XPathGeometryContext(new XPathGeometry());
            Assert.True(target is XGeometryContext);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new XPathGeometryContext(null));
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void BeginFigure_Adds_New_Figure()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            Assert.Equal(0, geometry.Figures.Count);

            target.BeginFigure(new XPoint());
            Assert.Equal(1, geometry.Figures.Count);
            Assert.True(geometry.Figures[0].IsFilled);
            Assert.True(geometry.Figures[0].IsClosed);

            target.SetClosedState(false);
            Assert.False(geometry.Figures[0].IsClosed);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void LineTo_Adds_New_XLineSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.LineTo(new XPoint());
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XLineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void ArcTo_Adds_New_XArcSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.ArcTo(new XPoint(), new XPathSize());
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XArcSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void BezierTo_Adds_New_XBezierSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.BezierTo(new XPoint(), new XPoint(), new XPoint());
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void QuadraticBezierTo_Adds_New_XQuadraticBezierSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.QuadraticBezierTo(new XPoint(), new XPoint());
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XQuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void PolyLineTo_Adds_New_XPolyLineSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.PolyLineTo(new List<XPoint>() { new XPoint(), new XPoint(), new XPoint() });
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XPolyLineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void PolyCubicBezierTo_Adds_New_XPolyCubicBezierSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.PolyCubicBezierTo(new List<XPoint>() { new XPoint(), new XPoint(), new XPoint() });
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XPolyCubicBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void PolyQuadraticBezierTo_Adds_New_XPolyQuadraticBezierSegment()
        {
            var geometry = new XPathGeometry();
            var target = new XPathGeometryContext(geometry);
            target.BeginFigure(new XPoint());
            Assert.Equal(0, geometry.Figures[0].Segments.Count);

            target.PolyQuadraticBezierTo(new List<XPoint>() { new XPoint(), new XPoint(), new XPoint() });
            Assert.Equal(1, geometry.Figures[0].Segments.Count);

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<XPolyQuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }
    }
}
