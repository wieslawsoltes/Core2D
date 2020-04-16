using Core2D;
using Core2D.Common.UnitTests;
using Xunit;

namespace Core2D.Style
{
    public class LineFixedLengthTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateLineFixedLength();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Flags_On_Set_Notify_Events_Are_Raised()
        {
            var state = _factory.CreateLineFixedLength();
            var target = new PropertyChangedObserver(state);

            state.Flags =
                LineFixedLengthFlags.Start
                | LineFixedLengthFlags.Vertical
                | LineFixedLengthFlags.Horizontal;

            Assert.Equal(
                LineFixedLengthFlags.Start
                | LineFixedLengthFlags.Vertical
                | LineFixedLengthFlags.Horizontal, state.Flags);
            Assert.Equal(7, target.PropertyNames.Count);

            var propertyNames = new string[]
            {
                nameof(ILineFixedLength.Flags),
                nameof(ILineFixedLength.Disabled),
                nameof(ILineFixedLength.Start),
                nameof(ILineFixedLength.End),
                nameof(ILineFixedLength.Vertical),
                nameof(ILineFixedLength.Horizontal),
                nameof(ILineFixedLength.All)
            };

            Assert.Equal(propertyNames, target.PropertyNames);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Disabled_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.Disabled = true;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);

            target.Disabled = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Start_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.Start = true;
            Assert.Equal(LineFixedLengthFlags.Start, target.Flags);

            target.Start = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void End_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.End = true;
            Assert.Equal(LineFixedLengthFlags.End, target.Flags);

            target.End = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Vertical_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.Vertical = true;
            Assert.Equal(LineFixedLengthFlags.Vertical, target.Flags);

            target.Vertical = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Horizontal_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.Horizontal = true;
            Assert.Equal(LineFixedLengthFlags.Horizontal, target.Flags);

            target.Horizontal = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void All_Property()
        {
            var target = _factory.CreateLineFixedLength();

            target.All = true;
            Assert.Equal(LineFixedLengthFlags.All, target.Flags);

            target.All = false;
            Assert.Equal(LineFixedLengthFlags.Disabled, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Parse_LineFixedLengthFlags_String()
        {
            var target = LineFixedLength.Parse("Start, Vertical, Horizontal");

            Assert.Equal(
                LineFixedLengthFlags.Start
                | LineFixedLengthFlags.Vertical
                | LineFixedLengthFlags.Horizontal, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ToString_Should_Return_Flags_String()
        {
            var target = _factory.CreateLineFixedLength(
                LineFixedLengthFlags.Start
                | LineFixedLengthFlags.Vertical
                | LineFixedLengthFlags.Horizontal);

            Assert.Equal("Start, Vertical, Horizontal", target.ToString());
        }
    }
}
