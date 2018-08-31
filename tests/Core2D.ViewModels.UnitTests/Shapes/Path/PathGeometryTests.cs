// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Interfaces;
using Core2D.Path;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathGeometryTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Figures_Not_Null()
        {
            var target = _factory.CreatePathGeometry();
            Assert.False(target.Figures.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void FillRule_Set_To_Nonzero_By_Default()
        {
            var target = _factory.CreatePathGeometry();
            Assert.Equal(FillRule.Nonzero, target.FillRule);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var geometry = _factory.CreatePathGeometry();

            var target = ImmutableArray.Create<IPathFigure>();
            var actual = (geometry as PathGeometry).ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Nonzero()
        {
            var target = _factory.CreatePathGeometry();

            var actual = target.ToString();

            Assert.Equal("F1", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_EvenOdd()
        {
            var target = _factory.CreatePathGeometry();

            target.FillRule = FillRule.EvenOdd;

            var actual = target.ToString();

            Assert.Equal("", actual);
        }
    }
}
