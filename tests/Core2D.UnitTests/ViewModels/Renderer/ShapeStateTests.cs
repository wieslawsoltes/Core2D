using Core2D;
using Core2D.Common.UnitTests;
using Xunit;

namespace Core2D.Renderer.UnitTests
{
    public class ShapeStateTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateShapeState();
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Parse_ShapeStateFlags_String()
        {
            var target = ShapeState.Parse("Visible, Printable, Standalone");

            Assert.Equal(
                ShapeStateFlags.Visible
                | ShapeStateFlags.Printable
                | ShapeStateFlags.Standalone, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void ToString_Should_Return_Flags_String()
        {
            var target = _factory.CreateShapeState(
                ShapeStateFlags.Visible
                | ShapeStateFlags.Printable
                | ShapeStateFlags.Standalone);

            Assert.Equal("Visible, Printable, Standalone", target.ToString());
        }
    }
}
