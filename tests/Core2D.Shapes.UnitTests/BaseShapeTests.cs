// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;
using Core2D.Shapes.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class BaseShapeTests
    {
        [Fact]
       [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Class1();
            Assert.True(target is ObservableObject);
        }

        [Fact]
       [Trait("Core2D.Shapes", "Shapes")]
        public void State_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.State);
        }

        [Fact]
       [Trait("Core2D.Shapes", "Shapes")]
        public void Default_ShapeStateFlags_Value()
        {
            var target = new Class1();
            Assert.Equal(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone, target.State.Flags);
        }

        [Fact]
       [Trait("Core2D.Shapes", "Shapes")]
        public void Data_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Data);
        }

        [Fact]
       [Trait("Core2D.Shapes", "Shapes")]
        public void Transform_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Transform);
        }

        private class Class1 : BaseShape
        {
            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

            public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<IPointShape> GetPoints()
            {
                throw new NotImplementedException();
            }

            public override void Move(ISet<IBaseShape> selected, double dx, double dy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
