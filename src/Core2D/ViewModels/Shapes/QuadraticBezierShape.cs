using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierShape : BaseShape
    {
        private PointShape _point1;
        private PointShape _point2;
        private PointShape _point3;

        [IgnoreDataMember]
        public override Type TargetType => typeof(QuadraticBezierShape);

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point1
        {
            get => _point1;
            set => RaiseAndSetIfChanged(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point2
        {
            get => _point2;
            set => RaiseAndSetIfChanged(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point3
        {
            get => _point3;
            set => RaiseAndSetIfChanged(ref _point3, value);
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawQuadraticBezier(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.DrawShape(dc, renderer);
                    _point2.DrawShape(dc, renderer);
                    _point3.DrawShape(dc, renderer);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_point1))
                    {
                        _point1.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_point2))
                    {
                        _point2.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_point3))
                    {
                        _point3.DrawShape(dc, renderer);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            _point1.Bind(dataFlow, db, record);
            _point2.Bind(dataFlow, db, record);
            _point3.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            if (!Point1.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point1.Move(selection, dx, dy);
            }

            if (!Point2.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point2.Move(selection, dx, dy);
            }

            if (!Point3.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point3.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            Point1.Select(selection);
            Point2.Select(selection);
            Point3.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            Point1.Deselect(selection);
            Point2.Deselect(selection);
            Point3.Deselect(selection);
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(Point1);
            points.Add(Point2);
            points.Add(Point3);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point1.IsDirty();
            isDirty |= Point2.IsDirty();
            isDirty |= Point3.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Point1.Invalidate();
            Point2.Invalidate();
            Point3.Invalidate();
        }
    }
}
