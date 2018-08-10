// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class GroupShapeTests
    {
        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_ConnectableShape()
        {
            var target = new GroupShape();
            Assert.True(target is ConnectableShape);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Shapes_Not_Null()
        {
            var target = new GroupShape();
            Assert.False(target.Shapes.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void ShapesProperties_Gets_Shapes_And_Connectors_Properties()
        {
            var target = new GroupShape();

            var shape = new Class1();
            shape.Data.Properties = shape.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape);

            var point = new PointShape();
            point.Data.Properties = point.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point);

            var length = target.ShapesProperties.Length;
            Assert.Equal(2, length);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void ShapesProperties_Cache_Should_Reset_After_Shapes_Updated()
        {
            var target = new GroupShape();

            var shape1 = new Class1();
            shape1.Data.Properties = shape1.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape1);

            var length1 = target.ShapesProperties.Length;
            Assert.Equal(1, length1);

            var shape2 = new Class1();
            shape2.Data.Properties = shape2.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape2);

            var length2 = target.ShapesProperties.Length;
            Assert.Equal(2, length2);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void GetPoints_Returns_Shapes_And_Connector_Points()
        {
            var target = new GroupShape();

            var text = new TextShape();
            text.Data.Properties = text.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(text);

            var point = new PointShape();
            point.Data.Properties = point.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point);

            var count = target.GetPoints().Count();
            Assert.Equal(3, count);
        }

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void AddShape_Add_Shape_To_Shapes()
        {
            var target = new GroupShape();
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
            var point1 = new PointShape();
            var point2 = new PointShape();

            var shapes = new BaseShape[] { shape1, shape2, point1, point2 };
            var source = shapes.ToList();

            var target = shapes.Group("g", source);

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
            var point1 = new PointShape();
            var point2 = new PointShape();

            var shapes = new BaseShape[] { shape1, shape2, point1, point2 };
            var source = shapes.ToList();

            shapes.Group("g", null);

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
            var point1 = new PointShape();
            var point2 = new PointShape();
            var point3 = new PointShape();

            var target = new GroupShape();

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
            public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<PointShape> GetPoints()
            {
                throw new NotImplementedException();
            }

            public override void Move(ISet<IShape> selected, double dx, double dy)
            {
                throw new NotImplementedException();
            }
        }
    }
}
