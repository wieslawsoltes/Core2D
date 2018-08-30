// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathGeometryContextTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Inherits_From_XGeometryContext()
        {
            var target = new PathGeometryContext(_factory, new PathGeometry());
            Assert.True(target is IGeometryContext);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PathGeometryContext(_factory, null));
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void BeginFigure_Adds_New_Figure()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            Assert.Empty(geometry.Figures);

            target.BeginFigure(_factory.CreatePointShape());
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
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.LineTo(_factory.CreatePointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<LineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ArcTo_Adds_New_ArcSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.ArcTo(_factory.CreatePointShape(), _factory.CreatePathSize());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<ArcSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void CubicBezierTo_Adds_New_CubicBezierSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.CubicBezierTo(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<CubicBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void QuadraticBezierTo_Adds_New_QuadraticBezierSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.QuadraticBezierTo(_factory.CreatePointShape(), _factory.CreatePointShape());

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<QuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyLineTo_Adds_New_XPolyLineSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);
            
            target.PolyLineTo(ImmutableArray.Create<IPointShape>(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyLineSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyCubicBezierTo_Adds_New_XPolyCubicBezierSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.PolyCubicBezierTo(ImmutableArray.Create<IPointShape>(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyCubicBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void PolyQuadraticBezierTo_Adds_New_XPolyQuadraticBezierSegment()
        {
            var geometry = _factory.CreatePathGeometry();
            var target = new PathGeometryContext(_factory, geometry);
            target.BeginFigure(_factory.CreatePointShape());
            Assert.Empty(geometry.Figures[0].Segments);

            target.PolyQuadraticBezierTo(ImmutableArray.Create<IPointShape>(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape()));

            var segment = geometry.Figures[0].Segments[0];
            Assert.IsType<PolyQuadraticBezierSegment>(segment);
            Assert.True(segment.IsStroked);
            Assert.True(segment.IsSmoothJoin);
        }
    }
}
