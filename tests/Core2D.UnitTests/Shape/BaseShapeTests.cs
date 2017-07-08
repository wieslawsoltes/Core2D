// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core2D.UnitTests
{
    public class BaseShapeTests
    {
        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Class1();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void State_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.State);
        }

        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void Default_ShapeStateFlags_Value()
        {
            var target = new Class1();
            Assert.Equal(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone, target.State.Flags);
        }

        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void Data_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Data);
        }

        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void Transform_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Transform);
        }

        private class Class1 : BaseShape
        {
            public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<PointShape> GetPoints()
            {
                throw new NotImplementedException();
            }

            public override void Move(ISet<BaseShape> selected, double dx, double dy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
