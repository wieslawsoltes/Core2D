using System;
using Xunit;

namespace Core2D.Spatial.UnitTests
{
    public class Point2Tests
    {
        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Point2(10, 20);
            Assert.Equal(10.0, target.X);
            Assert.Equal(20.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void FromXY_Returns_Valid_Point()
        {
            var target = Point2.FromXY(30, 50);
            Assert.NotNull(target);
            Assert.Equal(30.0, target.X);
            Assert.Equal(50.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void Operator_Plus_Adds_Field_Values()
        {
            var point1 = new Point2(5, 10);
            var point2 = new Point2(20, 7);
            var target = point1 + point2;
            Assert.Equal(25.0, target.X);
            Assert.Equal(17.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void Operator_Minus_Subtracts_Field_Values()
        {
            var point1 = new Point2(21, 9);
            var point2 = new Point2(8, 15);
            var target = point1 - point2;
            Assert.Equal(13.0, target.X);
            Assert.Equal(-6.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void DistanceTo_Calculates_Horizontal_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(30, 10);
            var target = point1.DistanceTo(point2);
            Assert.Equal(20.0, target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void DistanceTo_Calculates_Vertical_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(10, 40);
            var target = point1.DistanceTo(point2);
            Assert.Equal(30.0, target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void DistanceTo_Calculates_Diagonal_Distance_Between_Points()
        {
            var point1 = new Point2(10, 10);
            var point2 = new Point2(35, 20);
            var target = point1.DistanceTo(point2);
            Assert.Equal(26.92582403567252, target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void AngleBetween_Calculates_Angle_In_Degrees()
        {
            var origin = new Point2(30, 30);
            Assert.Equal(0.0, origin.AngleBetween(new Point2(30, 30)));
            Assert.Equal(0.0, origin.AngleBetween(new Point2(60, 30)));
            Assert.Equal(45.0, origin.AngleBetween(new Point2(60, 60)));
            Assert.Equal(90.0, origin.AngleBetween(new Point2(30, 60)));
            Assert.Equal(135.0, origin.AngleBetween(new Point2(0, 60)));
            Assert.Equal(180.0, origin.AngleBetween(new Point2(0, 30)));
            Assert.Equal(225.0, origin.AngleBetween(new Point2(0, 0)));
            Assert.Equal(270.0, origin.AngleBetween(new Point2(30, 0)));
            Assert.Equal(315.0, origin.AngleBetween(new Point2(60, 0)));
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void RotateAt_Rotates_Point_Around_Center_Point_By_Specified_Angle_In_Degrees()
        {
            var center = new Point2(0, 0);
            var point = new Point2(0, 30);

            var delta = 12;

            var target0 = point.RotateAt(center, 0);
            Assert.Equal(0.0, target0.X, delta);
            Assert.Equal(30.0, target0.Y, delta);

            var target90 = point.RotateAt(center, 90);
            Assert.Equal(-30.0, target90.X, delta);
            Assert.Equal(0.0, target90.Y, delta);

            var target180 = point.RotateAt(center, 180);
            Assert.Equal(0.0, target180.X, delta);
            Assert.Equal(-30.0, target180.Y);

            var target270 = point.RotateAt(center, 270);
            Assert.Equal(30.0, target270.X, delta);
            Assert.Equal(0.0, target270.Y, delta);

            var target360 = point.RotateAt(center, 360);
            Assert.Equal(0.0, target360.X, delta);
            Assert.Equal(30.0, target360.Y, delta);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void ProjectOnLine_Projects_Point_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(25, 25).ProjectOnLine(a, b);
            Assert.Equal(0.0, target.X);
            Assert.Equal(25.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void ProjectOnLine_Projects_Point_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).ProjectOnLine(a, b);
            Assert.Equal(25.0, target.X);
            Assert.Equal(0.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void ProjectOnLine_Projects_Point_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(0, 0).ProjectOnLine(a, b);
            Assert.Equal(25.0, target.X);
            Assert.Equal(25.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void NearestOnLine_Finds_Nearest_Point_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(25, 25).NearestOnLine(a, b);
            Assert.Equal(0.0, target.X);
            Assert.Equal(25.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void NearestOnLine_Finds_Nearest_Point_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).NearestOnLine(a, b);
            Assert.Equal(25.0, target.X);
            Assert.Equal(0.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void NearestOnLine_Finds_Nearest_Point_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(0, 0).NearestOnLine(a, b);
            Assert.Equal(25.0, target.X);
            Assert.Equal(25.0, target.Y);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_True_If_Point_Is_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(0, 25).IsOnLine(a, b);
            Assert.True(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_True_If_Point_Is_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 0).IsOnLine(a, b);
            Assert.True(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_True_If_Point_Is_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(25, 25).IsOnLine(a, b);
            Assert.True(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_False_If_Point_Is_Not_On_Vertical_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(0, 50);
            var target = new Point2(0.1, 25).IsOnLine(a, b);
            Assert.False(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_False_If_Point_Is_Not_On_Horizontal_Line()
        {
            var a = new Point2(0, 0);
            var b = new Point2(50, 0);
            var target = new Point2(25, 0.1).IsOnLine(a, b);
            Assert.False(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_False_If_Point_X_Is_Not_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(50.1, 50.0).IsOnLine(a, b);
            Assert.False(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void IsOnLine_Returns_False_If_Point_Y_Is_Not_On_Diagonal_Line()
        {
            var a = new Point2(0, 50);
            var b = new Point2(50, 0);
            var target = new Point2(50.0, 50.1).IsOnLine(a, b);
            Assert.False(target);
        }

        [Fact]
        [Trait("Core2D.Spatial", "Point2")]
        public void ExpandToRect_Expands_Point_To_Rectangle_By_Specified_Radius()
        {
            var point = new Point2(0, 0);
            var target = point.ExpandToRect(10);
            Assert.Equal(-10.0, target.Left);
            Assert.Equal(10.0, target.Right);
            Assert.Equal(-10.0, target.Top);
            Assert.Equal(10.0, target.Bottom);
        }
    }
}
