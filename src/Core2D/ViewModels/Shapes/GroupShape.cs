using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public class GroupShape : ConnectableShape
    {
        private ImmutableArray<Property> _shapesProperties;
        private ImmutableArray<BaseShape> _shapes;

        public override Type TargetType => typeof(GroupShape);

        public ImmutableArray<Property> ShapesProperties => GetShapeProperties();

        public ImmutableArray<BaseShape> Shapes
        {
            get => _shapes;
            set
            {
                if (RaiseAndSetIfChanged(ref _shapes, value))
                {
                    _shapesProperties = default;
                }
            }
        }

        private ImmutableArray<Property> GetShapeProperties()
        {
            if (_shapesProperties == null)
            {
                if (_shapes != null)
                {
                    var builder = ImmutableArray.CreateBuilder<Property>();

                    foreach (var shape in _shapes)
                    {
                        foreach (var property in shape.Data.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    foreach (var connector in base.Connectors)
                    {
                        foreach (var property in connector.Data.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    _shapesProperties = builder.ToImmutable();
                }
            }
            return _shapesProperties;
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.DrawShape(dc, renderer);
                }
            }

            base.DrawShape(dc, renderer);
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in Shapes)
                {
                    shape.DrawPoints(dc, renderer);
                }
            }

            base.DrawPoints(dc, renderer);
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            foreach (var shape in Shapes)
            {
                shape.Bind(dataFlow, db, record);
            }

            base.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            foreach (var shape in Shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Connector))
                {
                    shape.Move(selection, dx, dy);
                }
            }

            base.Move(selection, dx, dy);
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var shape in Shapes)
            {
                shape.GetPoints(points);
            }

            base.GetPoints(points);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var shape in Shapes)
            {
                isDirty |= shape.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var shape in Shapes)
            {
                shape.Invalidate();
            }
        }

        public virtual bool ShouldSerializeShapes() => true;
    }
}
