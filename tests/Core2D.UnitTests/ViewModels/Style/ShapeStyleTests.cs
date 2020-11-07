using Xunit;

namespace Core2D.Style.UnitTests
{
    public class ShapeStyleTests
    {
        private readonly IFactory _factory = new Factory();
        
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDoubleArrayToDashes_Should_Return_Dashes_String()
        {
            var dashes = new double[] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 };
            var expected = "2 2 0 2 0 2";

            var target = StyleHelper.ConvertDoubleArrayToDashes(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertFloatArrayToDashes_Should_Return_Dashes_String()
        {
            var dashes = new float[] { 2.0f, 2.0f, 0.0f, 2.0f, 0.0f, 2.0f };
            var expected = "2 2 0 2 0 2";

            var target = StyleHelper.ConvertFloatArrayToDashes(dashes);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToDoubleArray_Should_Return_Dashes_Double_Array()
        {
            var dashes = "2 2 0 2 0 2";
            var expected = new double[] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 };

            var target = StyleHelper.ConvertDashesToDoubleArray(dashes, 1.0);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToFloatArray_Should_Return_Dashes_Float_Array()
        {
            var dashes = "2 2 0 2 0 2";
            var expected = new float[] { 2.0f, 2.0f, 0.0f, 2.0f, 0.0f, 2.0f };

            var target = StyleHelper.ConvertDashesToFloatArray(dashes, 1.0);

            Assert.Equal(expected, target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDoubleArrayToDashes_Should_Not_Throw()
        {
            var target = StyleHelper.ConvertDoubleArrayToDashes(null);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertFloatArrayToDashes_Should_Not_Throw()
        {
            var target = StyleHelper.ConvertFloatArrayToDashes(null);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToDoubleArray_Should_Not_Throw()
        {
            var dashes = "0 A";
            var target = StyleHelper.ConvertDashesToDoubleArray(dashes, 1.0);
            Assert.Null(target);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ConvertDashesToFloatArray_Should_Not_Throw()
        {
            var dashes = "0 A";
            var target = StyleHelper.ConvertDashesToFloatArray(dashes, 1.0);
            Assert.Null(target);
        }
        
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateShapeStyle();
            Assert.True(target is ViewModelBase);
        }
    }
}
