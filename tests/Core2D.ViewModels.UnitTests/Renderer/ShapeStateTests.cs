// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Common;
using Core2D.Renderer;
using Xunit;

namespace Core2D.Renderer.UnitTests
{
    public class ShapeStateTests
    {
        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Inherits_From_ObservableObject()
        {
            var target = new ShapeState();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Flags_On_Set_Notify_Events_Are_Raised()
        {
            var state = new ShapeState();
            var target = new PropertyChangedObserver(state);

            state.Flags = 
                ShapeStateFlags.Visible 
                | ShapeStateFlags.Printable 
                | ShapeStateFlags.Standalone;

            Assert.Equal(
                ShapeStateFlags.Visible 
                | ShapeStateFlags.Printable 
                | ShapeStateFlags.Standalone, state.Flags);
            Assert.Equal(10, target.PropertyNames.Count);

            var propertyNames = new string[]
            {
                nameof(IShapeState.Flags),
                nameof(IShapeState.Default),
                nameof(IShapeState.Visible),
                nameof(IShapeState.Printable),
                nameof(IShapeState.Locked),
                nameof(IShapeState.Connector),
                nameof(IShapeState.None),
                nameof(IShapeState.Standalone),
                nameof(IShapeState.Input),
                nameof(IShapeState.Output)
            };

            Assert.Equal(propertyNames, target.PropertyNames);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Default_Property()
        {
            var target = new ShapeState();

            target.Default = true;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);

            target.Default = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Visible_Property()
        {
            var target = new ShapeState();

            target.Visible = true;
            Assert.Equal(ShapeStateFlags.Visible, target.Flags);

            target.Visible = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Printable_Property()
        {
            var target = new ShapeState();

            target.Printable = true;
            Assert.Equal(ShapeStateFlags.Printable, target.Flags);

            target.Printable = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Locked_Property()
        {
            var target = new ShapeState();

            target.Locked = true;
            Assert.Equal(ShapeStateFlags.Locked, target.Flags);

            target.Locked = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Connector_Property()
        {
            var target = new ShapeState();

            target.Connector = true;
            Assert.Equal(ShapeStateFlags.Connector, target.Flags);

            target.Connector = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void None_Property()
        {
            var target = new ShapeState();

            target.None = true;
            Assert.Equal(ShapeStateFlags.None, target.Flags);

            target.None = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Standalone_Property()
        {
            var target = new ShapeState();

            target.Standalone = true;
            Assert.Equal(ShapeStateFlags.Standalone, target.Flags);

            target.Standalone = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Input_Property()
        {
            var target = new ShapeState();

            target.Input = true;
            Assert.Equal(ShapeStateFlags.Input, target.Flags);

            target.Input = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Renderer", "Renderer")]
        public void Output_Property()
        {
            var target = new ShapeState();

            target.Output = true;
            Assert.Equal(ShapeStateFlags.Output, target.Flags);

            target.Output = false;
            Assert.Equal(ShapeStateFlags.Default, target.Flags);
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
            var target = Factory.CreateShapeState(
                ShapeStateFlags.Visible
                | ShapeStateFlags.Printable
                | ShapeStateFlags.Standalone);

            Assert.Equal("Visible, Printable, Standalone", target.ToString());
        }
    }
}
