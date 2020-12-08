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
    public class GroupShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_ConnectableShape()
        {
            var target = _factory.CreateGroupShape();
            Assert.True(target is ConnectableShape);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Shapes_Not_Null()
        {
            var target = _factory.CreateGroupShape();
            Assert.False(target.Shapes.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void ShapesProperties_Gets_Shapes_And_Connectors_Properties()
        {
            var target = _factory.CreateGroupShape();

            var shape = new Class1()
            {
                State = ShapeStateFlags.Default,
                Properties = ImmutableArray.Create<Property>()
            };
            shape.Properties = shape.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape);

            var point = _factory.CreatePointShape();
            point.Properties = point.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Connectors = target.Connectors.Add(point);

            var length = target.ShapesProperties.Length;
            Assert.Equal(2, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void ShapesProperties_Cache_Should_Reset_After_Shapes_Updated()
        {
            var target = _factory.CreateGroupShape();

            var shape1 = new Class1()
            {
                State = ShapeStateFlags.Default,
                Properties = ImmutableArray.Create<Property>()
            };
            shape1.Properties = shape1.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape1);

            var length1 = target.ShapesProperties.Length;
            Assert.Equal(1, length1);

            var shape2 = new Class1()
            {
                State = ShapeStateFlags.Default,
                Properties = ImmutableArray.Create<Property>()
            };
            shape2.Properties = shape2.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape2);

            var length2 = target.ShapesProperties.Length;
            Assert.Equal(2, length2);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void GetPoints_Returns_Shapes_And_Connector_Points()
        {
            var style = _factory.CreateShapeStyle();
            var target = _factory.CreateGroupShape();

            var text = _factory.CreateTextShape(0, 0, style, "Text");
            text.Properties = text.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(text);

            var point = _factory.CreatePointShape();
            point.Properties = point.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Connectors = target.Connectors.Add(point);

            var points = new List<PointShape>();
            target.GetPoints(points);
            var count = points.Count();
            Assert.Equal(3, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddShape_Add_Shape_To_Shapes()
        {
            var target = _factory.CreateGroupShape();
            var shape = new Class1()
            {
                State = ShapeStateFlags.Default
            };

            target.AddShape(shape);

            Assert.Equal(shape.Owner, target);
            Assert.False(shape.State.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(shape, target.Shapes);

            var length = target.Shapes.Length;
            Assert.Equal(1, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Group_Shapes_Remove_And_Add_To_Source()
        {
            var shape1 = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            var shape2 = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();

            var shapes = new List<BaseShape> { shape1, shape2, point1, point2 };
            var source = shapes.ToList();

            var target = _factory.CreateGroupShape("g");
            target.Group(shapes, source);

            Assert.Equal("g", target.Name);

            Assert.Contains(shape1, target.Shapes);
            Assert.Contains(shape2, target.Shapes);

            var lengthShapes = target.Shapes.Length;
            Assert.Equal(2, lengthShapes);

            Assert.Contains(point1, target.Connectors);
            Assert.Contains(point2, target.Connectors);

            var lengthConnectors = target.Connectors.Length;
            Assert.Equal(2, lengthConnectors);

            Assert.Contains(target, source);

            var count = source.Count;
            Assert.Equal(1, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Group_Shapes_Do_Not_Update_Source()
        {
            var shape1 = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            var shape2 = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();

            var shapes = new BaseShape[] { shape1, shape2, point1, point2 };
            var source = shapes.ToList();

            var target = _factory.CreateGroupShape("g");
            target.Group(shapes, null);

            Assert.Contains(shape1, source);
            Assert.Contains(shape2, source);
            Assert.Contains(point1, source);
            Assert.Contains(point2, source);

            var count = source.Count;
            Assert.Equal(4, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Ungroup_Shape_Remove_And_Add_To_Source()
        {
            var shape = new Class1()
            {
                State = ShapeStateFlags.Default
            };
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();
            var point3 = _factory.CreatePointShape();

            var target = _factory.CreateGroupShape();

            target.AddShape(shape);
            target.AddConnectorAsNone(point1);
            target.AddConnectorAsInput(point2);
            target.AddConnectorAsOutput(point3);

            var source = new List<BaseShape> { target };

            target.Ungroup(source);

            Assert.Contains(shape, source);
            Assert.Contains(point1, source);
            Assert.Contains(point2, source);
            Assert.Contains(point3, source);

            var count = source.Count;
            Assert.Equal(4, count);

            Assert.False(point1.State.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.None));
            Assert.False(point2.State.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Input));
            Assert.False(point3.State.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Output));

            Assert.True(shape.State.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point1.State.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point2.State.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point3.State.HasFlag(ShapeStateFlags.Standalone));
        }

        public class Class1 : BaseShape
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
