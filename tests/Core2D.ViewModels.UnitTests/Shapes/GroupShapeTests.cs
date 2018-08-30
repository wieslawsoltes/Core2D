// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Data;
using Core2D.Interfaces;
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

            var shape = new Class1();
            shape.Data.Properties = shape.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape);

            var point = _factory.CreatePointShape();
            point.Data.Properties = point.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Connectors = target.Connectors.Add(point);

            var length = target.ShapesProperties.Length;
            Assert.Equal(2, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void ShapesProperties_Cache_Should_Reset_After_Shapes_Updated()
        {
            var target = _factory.CreateGroupShape();

            var shape1 = new Class1();
            shape1.Data.Properties = shape1.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape1);

            var length1 = target.ShapesProperties.Length;
            Assert.Equal(1, length1);

            var shape2 = new Class1();
            shape2.Data.Properties = shape2.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(shape2);

            var length2 = target.ShapesProperties.Length;
            Assert.Equal(2, length2);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void GetPoints_Returns_Shapes_And_Connector_Points()
        {
            var target = _factory.CreateGroupShape();

            var text = _factory.CreateTextShape();
            text.Data.Properties = text.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Shapes = target.Shapes.Add(text);

            var point = _factory.CreatePointShape();
            point.Data.Properties = point.Data.Properties.Add(_factory.CreateProperty(null, "", ""));
            target.Connectors = target.Connectors.Add(point);

            var count = target.GetPoints().Count();
            Assert.Equal(3, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddShape_Add_Shape_To_Shapes()
        {
            var target = _factory.CreateGroupShape();
            var shape = new Class1();

            target.AddShape(shape);

            Assert.Equal(shape.Owner, target);
            Assert.False(shape.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(shape, target.Shapes);
            
            var length = target.Shapes.Length;
            Assert.Equal(1, length);
        }
        
        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Group_Shapes_Remove_And_Add_To_Source()
        {
            var shape1 = new Class1();
            var shape2 = new Class1();
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();

            var shapes = new List<IBaseShape>{ shape1, shape2, point1, point2 };
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
            var shape1 = new Class1();
            var shape2 = new Class1();
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();

            var shapes = new IBaseShape[] { shape1, shape2, point1, point2 };
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
            var shape = new Class1();
            var point1 = _factory.CreatePointShape();
            var point2 = _factory.CreatePointShape();
            var point3 = _factory.CreatePointShape();

            var target = _factory.CreateGroupShape();

            target.AddShape(shape);
            target.AddConnectorAsNone(point1);
            target.AddConnectorAsInput(point2);
            target.AddConnectorAsOutput(point3);

            var source = new List<IBaseShape> { target };

            target.Ungroup(source);

            Assert.Contains(shape, source);
            Assert.Contains(point1, source);
            Assert.Contains(point2, source);
            Assert.Contains(point3, source);

            var count = source.Count;
            Assert.Equal(4, count);

            Assert.False(point1.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.None));
            Assert.False(point2.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Input));
            Assert.False(point3.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.Output));

            Assert.True(shape.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point1.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point2.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.True(point3.State.Flags.HasFlag(ShapeStateFlags.Standalone));
        }

        public class Class1 : BaseShape
        {
            public override Type TargetType => typeof(Class1);

            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

            public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r)
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
