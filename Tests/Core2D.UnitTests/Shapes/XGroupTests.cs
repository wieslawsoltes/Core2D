// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class XGroupTests
    {
        [Fact]
        [Trait("Core2D", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var target = new XGroup();
            Assert.True(target is BaseShape);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void Shapes_Not_Null()
        {
            var target = new XGroup();
            Assert.NotNull(target.Shapes);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void Connectors_Not_Null()
        {
            var target = new XGroup();
            Assert.NotNull(target.Connectors);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void ShapesProperties_Gets_Shapes_And_Connectors_Properties()
        {
            var target = new XGroup();

            var shape = new Class1();
            shape.Data.Properties = shape.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape);

            var point = new XPoint();
            point.Data.Properties = point.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point);

            Assert.Equal(2, target.ShapesProperties.Length);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void ShapesProperties_Cache_Should_Reset_After_Shapes_Updated()
        {
            var target = new XGroup();

            var shape1 = new Class1();
            shape1.Data.Properties = shape1.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape1);

            Assert.Equal(1, target.ShapesProperties.Length);

            var shape2 = new Class1();
            shape2.Data.Properties = shape2.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(shape2);

            Assert.Equal(2, target.ShapesProperties.Length);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void ShapesProperties_Cache_Should_Reset_After_Connectors_Updated()
        {
            var target = new XGroup();

            var point1 = new XPoint();
            point1.Data.Properties = point1.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point1);

            Assert.Equal(1, target.ShapesProperties.Length);

            var point2 = new XPoint();
            point2.Data.Properties = point2.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point2);

            Assert.Equal(2, target.ShapesProperties.Length);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void GetPoints_Returns_Shapes_And_Connector_Points()
        {
            var target = new XGroup();

            var text = new XText();
            text.Data.Properties = text.Data.Properties.Add(new Property());
            target.Shapes = target.Shapes.Add(text);

            var point = new XPoint();
            point.Data.Properties = point.Data.Properties.Add(new Property());
            target.Connectors = target.Connectors.Add(point);

            Assert.Equal(3, target.GetPoints().Count());
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void AddShape_Add_Shape_To_Shapes()
        {
            var target = new XGroup();
            var shape = new Class1();

            target.AddShape(shape);

            Assert.Equal(shape.Owner, target);
            Assert.False(shape.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(shape, target.Shapes);
        }

        [Fact]
        [Trait("Core2D", "Shapes")]
        public void AddConnectorAsNone_Add_Point_To_Connectors_As_None()
        {
            var target = new XGroup();
            var point = new XPoint();

            target.AddConnectorAsNone(point);

            Assert.Equal(point.Owner, target);
            Assert.True(point.State.Flags.HasFlag(ShapeStateFlags.Connector | ShapeStateFlags.None));
            Assert.False(point.State.Flags.HasFlag(ShapeStateFlags.Standalone));
            Assert.Contains(point, target.Connectors);
        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void AddConnectorAsInput_Add_Point_To_Connectors_As_Input()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void AddConnectorAsOutput_Add_Point_To_Connectors_As_Output()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void Group_Shapes_Remove_And_Add_To_Source()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void Group_Shapes_Do_Not_Update_Source()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void Ungroup_Group_And_Add_To_Source()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "Shapes")]
        public void Ungroup_Shapes_And_Add_To_Source()
        {

        }

        public class Class1 : BaseShape
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
