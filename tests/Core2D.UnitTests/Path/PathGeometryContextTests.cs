// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathGeometryContextTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Inherits_From_XGeometryContext()
        {
            var target = new PathGeometryContext(new PathGeometry());
            Assert.True(target is GeometryContext);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PathGeometryContext(null));
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void BeginFigure_Adds_New_Figure()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            Assert.Empty(geometry.Figures);

            target.BeginFigure(new PointShape());
            Assert.NotNull(geometry.Figures[0]);
            Assert.True(geometry.Figures[0].IsFilled);
            Assert.True(geometry.Figures[0].IsClosed);

            target.SetClosedState(false);
            Assert.False(geometry.Figures[0].IsClosed);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void LineTo_Adds_New_LineSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.LineTo(new PointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<LineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ArcTo_Adds_New_ArcSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.ArcTo(new PointShape(), new PathSize());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<ArcSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void CubicBezierTo_Adds_New_CubicBezierSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.CubicBezierTo(new PointShape(), new PointShape(), new PointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<CubicBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void QuadraticBezierTo_Adds_New_QuadraticBezierSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.QuadraticBezierTo(new PointShape(), new PointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<QuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyLineTo_Adds_New_XPolyLineSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);
            
            target.PolyLineTo(ImmutableArray.Create<PointShape>(new PointShape(), new PointShape(), new PointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyLineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyCubicBezierTo_Adds_New_XPolyCubicBezierSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.PolyCubicBezierTo(ImmutableArray.Create<PointShape>(new PointShape(), new PointShape(), new PointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyCubicBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyQuadraticBezierTo_Adds_New_XPolyQuadraticBezierSegment()
        {
            var geometry = new PathGeometry();
            var target = new PathGeometryContext(geometry);
            target.BeginFigure(new PointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.PolyQuadraticBezierTo(ImmutableArray.Create<PointShape>(new PointShape(), new PointShape(), new PointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyQuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }
    }
}
