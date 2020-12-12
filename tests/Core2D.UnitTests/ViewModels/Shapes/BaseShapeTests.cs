using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class BaseShapeTests
    {
        private readonly IFactory _factory = new Factory(null);

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

        private class Class1 : BaseShapeViewModel
        {
            public Class1() : base(null, typeof(Class1))
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

            public override void GetPoints(IList<PointShapeViewModel> points)
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
