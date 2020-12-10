using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public partial class GroupShapeViewModel : ConnectableShapeViewModel
    {
        // TODO: AutoNotify
        [AutoNotify] private ImmutableArray<PropertyViewModel> _shapesProperties;
        [AutoNotify] private ImmutableArray<BaseShapeViewModel> _shapes;

        public GroupShapeViewModel() : base(typeof(GroupShapeViewModel))
        {
            _shapesProperties = GetShapeProperties();
        }

        private ImmutableArray<PropertyViewModel> GetShapeProperties()
        {
            if (_shapesProperties == null)
            {
                if (_shapes != null)
                {
                    var builder = ImmutableArray.CreateBuilder<PropertyViewModel>();

                    foreach (var shape in _shapes)
                    {
                        foreach (var property in shape.Properties)
                        {
                            builder.Add(property);
                        }
                    }

                    foreach (var connector in base.Connectors)
                    {
                        foreach (var property in connector.Properties)
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
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in _shapes)
                {
                    shape.DrawShape(dc, renderer);
                }
            }

            base.DrawShape(dc, renderer);
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                foreach (var shape in _shapes)
                {
                    shape.DrawPoints(dc, renderer);
                }
            }

            base.DrawPoints(dc, renderer);
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            foreach (var shape in _shapes)
            {
                shape.Bind(dataFlow, db, record);
            }

            base.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            foreach (var shape in _shapes)
            {
                if (!shape.State.HasFlag(ShapeStateFlags.Connector))
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

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            foreach (var shape in _shapes)
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

            foreach (var shape in _shapes)
            {
                isDirty |= shape.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var shape in _shapes)
            {
                shape.Invalidate();
            }
        }
    }
}
