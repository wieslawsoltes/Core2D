using System;
using Xunit;

namespace Core2D.Spatial.UnitTests
{
    public class Rect2Tests
    {
        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.Equal(15.0, target.X);
            Assert.Equal(20.0, target.Y);
            Assert.Equal(40.0, target.Width);
            Assert.Equal(35.0, target.Height);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void FromPoints_Returns_Valid_Rectangle()
        {
            var target1 = Rect2.FromPoints(0, 0, 10, 10);
            Assert.Equal(0.0, target1.X);
            Assert.Equal(0.0, target1.Y);
            Assert.Equal(10.0, target1.Width);
            Assert.Equal(10.0, target1.Height);

            var target2 = Rect2.FromPoints(20, 20, 5, 5);
            Assert.Equal(5.0, target2.X);
            Assert.Equal(5.0, target2.Y);
            Assert.Equal(15.0, target2.Width);
            Assert.Equal(15.0, target2.Height);

            var target3 = Rect2.FromPoints(20, 20, 5, 5, 3, 2);
            Assert.Equal(8.0, target3.X);
            Assert.Equal(7.0, target3.Y);
            Assert.Equal(15.0, target3.Width);
            Assert.Equal(15.0, target3.Height);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void FromPoints_Point_Returns_Valid_Rectangle()
        {
            var target1 = Rect2.FromPoints(new Point2(0, 0), new Point2(10, 10));
            Assert.Equal(0.0, target1.X);
            Assert.Equal(0.0, target1.Y);
            Assert.Equal(10.0, target1.Width);
            Assert.Equal(10.0, target1.Height);

            var target2 = Rect2.FromPoints(new Point2(20, 20), new Point2(5, 5));
            Assert.Equal(5.0, target2.X);
            Assert.Equal(5.0, target2.Y);
            Assert.Equal(15.0, target2.Width);
            Assert.Equal(15.0, target2.Height);

            var target3 = Rect2.FromPoints(new Point2(20, 20), new Point2(5, 5), 3, 2);
            Assert.Equal(8.0, target3.X);
            Assert.Equal(7.0, target3.Y);
            Assert.Equal(15.0, target3.Width);
            Assert.Equal(15.0, target3.Height);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Top_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.Equal(20.0, target.Top);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Left_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.Equal(15.0, target.Left);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Bottom_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.Equal(20.0 + 35.0, target.Bottom);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Right_Property_Returns_Valid_Value()
        {
            var target = new Rect2(15, 20, 40, 35);
            Assert.Equal(15.0 + 40.0, target.Right);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Center_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.Center;
            Assert.Equal(15.0 + 40.0 / 2, target.X);
            Assert.Equal(20.0 + 35.0 / 2, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void TopLeft_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.TopLeft;
            Assert.Equal(15.0, target.X);
            Assert.Equal(20.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void BottomRight_Property_Returns_Valid_Value()
        {
            var rect = new Rect2(15, 20, 40, 35);
            var target = rect.BottomRight;
            Assert.Equal(15.0 + 40.0, target.X);
            Assert.Equal(20.0 + 35.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Contains_Point_Returns_True_If_Point_Is_Inside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.Contains(new Point2(15, 15)));
            Assert.True(target.Contains(new Point2(11, 10)));
            Assert.True(target.Contains(new Point2(29, 30)));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Contains_Point_Returns_False_If_Point_Is_Outside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.Contains(new Point2(9, 15)));
            Assert.False(target.Contains(new Point2(15, 31)));
            Assert.False(target.Contains(new Point2(15, 9)));
            Assert.False(target.Contains(new Point2(15, 31)));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Contains_Returns_True_If_Point_Is_Inside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.Contains(15, 15));
            Assert.True(target.Contains(11, 10));
            Assert.True(target.Contains(29, 30));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void Contains_Returns_False_If_Point_Is_Outside_Rectangle()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.Contains(9, 15));
            Assert.False(target.Contains(15, 31));
            Assert.False(target.Contains(15, 9));
            Assert.False(target.Contains(15, 31));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void IntersectsWith_Returns_True_If_Rectangles_Intersect()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.True(target.IntersectsWith(new Rect2(12, 12, 16, 16)));
            Assert.True(target.IntersectsWith(new Rect2(5, 10, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(20, 10, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(10, 5, 20, 20)));
            Assert.True(target.IntersectsWith(new Rect2(10, 20, 20, 20)));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Rect2")]
        public void IntersectsWith_Returns_False_If_Rectangles_Do_Not_Intersect()
        {
            var target = new Rect2(10, 10, 20, 20);
            Assert.False(target.IntersectsWith(new Rect2(0, 9, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(31, 9, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(10, 0, 9, 9)));
            Assert.False(target.IntersectsWith(new Rect2(10, 31, 9, 9)));
        }
    }
}
