using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Data;
using Core2D.Renderer;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class BaseShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_ViewModelBase()
        {
            var target = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            Assert.True(target is ViewModelBase);
        }

        private class Class1 : BaseShape
        {
            public Class1() : base(typeof(Class1))
            {
            }

            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

            public override void DrawShape(object dc, IShapeRenderer renderer)
            {
                throw new NotImplementedException();
            }

            public override void DrawPoints(object dc, IShapeRenderer renderer)
            {
                throw new NotImplementedException();
            }

            public override void Bind(DataFlow dataFlow, object db, object r)
            {
                throw new NotImplementedException();
            }

            public override void GetPoints(IList<PointShape> points)
            {
                throw new NotImplementedException();
            }

            public override void Move(ISelection selection, decimal dx, decimal dy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
