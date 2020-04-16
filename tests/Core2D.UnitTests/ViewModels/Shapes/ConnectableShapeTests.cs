using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
using Core2D.Data;
using Core2D.Renderer;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class ConnectableShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };
            Assert.True(target is BaseShape);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Connectors_Not_Null()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };
            Assert.False(target.Connectors.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void GetPoints_Returns_Connector_Points()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };

            var point = _factory.CreatePointShape();
            point.Data.Properties = point.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Connectors = target.Connectors.Add(point);

            var count = target.GetPoints().Count();
            Assert.Equal(1, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddConnectorAsNone_Add_Point_To_Connectors_As_None()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };
            var point = _factory.CreatePointShape();

            target.AddConnectorAsNone(point);

            Assert.Equal(point.Owner, target);
            Assert.True(point.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.None));
            Assert.False(point.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(point, target.Connectors);

            var length = target.Connectors.Length;
            Assert.Equal(1, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddConnectorAsInput_Add_Point_To_Connectors_As_Input()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };
            var point = _factory.CreatePointShape();

            target.AddConnectorAsInput(point);

            Assert.Equal(point.Owner, target);
            Assert.True(point.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Input));
            Assert.False(point.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(point, target.Connectors);

            var length = target.Connectors.Length;
            Assert.Equal(1, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddConnectorAsOutput_Add_Point_To_Connectors_As_Output()
        {
            var target = new Class2()
            {
                State = _factory.CreateShapeState(),
                Connectors = ImmutableArray.Create<IPointShape>()
            };
            var point = _factory.CreatePointShape();

            target.AddConnectorAsOutput(point);

            Assert.Equal(point.Owner, target);
            Assert.True(point.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Output));
            Assert.False(point.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(point, target.Connectors);

            var length = target.Connectors.Length;
            Assert.Equal(1, length);
        }

        public class Class1 : BaseShape
        {
            public override Type TargetType => typeof(Class1);

            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

            public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
            {
                throw new NotImplementedException();
            }

            public override void Bind(IDataFlow dataFlow, object db, object r)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<IPointShape> GetPoints()
            {
                throw new NotImplementedException();
            }

            public override void Move(ISelection selection, double dx, double dy)
            {
                throw new NotImplementedException();
            }
        }

        public class Class2 : ConnectableShape
        {
            public override Type TargetType => typeof(Class2);
        }
    }
}
