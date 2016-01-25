// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace Core2D.UnitTests
{
    public class BaseShapeTests
    {
        [Fact]
        [Trait("Core2D", "Shape")]
        public void Inherits_From_ObservableResource()
        {
            var target = new Class1();
            Assert.True(target is ObservableResource);
        }

        [Fact]
        [Trait("Core2D", "Shape")]
        public void State_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.State);
        }

        [Fact]
        [Trait("Core2D", "Shape")]
        public void Default_ShapeStateFlags_Value()
        {
            var target = new Class1();
            Assert.Equal(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone, target.State.Flags);
        }

        [Fact]
        [Trait("Core2D", "Shape")]
        public void Data_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Data);
        }

        private class Class1 : BaseShape
        {
            public override void Draw(object dc, Renderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<XPoint> GetPoints()
            {
                throw new NotImplementedException();
            }

            public override void Move(double dx, double dy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
