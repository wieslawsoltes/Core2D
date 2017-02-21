using System;
using Xunit;

namespace Core2D.Spatial.UnitTests
{
    public class Size2Tests
    {
        [Fact]
        [Trait("Core2D.Spatial", "Size2")]
        public void Construtor_Sets_All_Fields()
        {
            var target = new Size2(600, 400);
            Assert.Equal(600.0, target.Width);
            Assert.Equal(400.0, target.Height);
        }
    }
}
