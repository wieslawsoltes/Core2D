using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Data;
using Core2D.Path;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    [DataContract(IsReference = true)]
    public class PathShape : BaseShape
    {
        private List<PointShape> _points;
        private PathGeometry _geometry;

        [IgnoreDataMember]
        public override Type TargetType => typeof(PathShape);

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PathGeometry Geometry
        {
            get => _geometry;
            set => RaiseAndSetIfChanged(ref _geometry, value);
        }

        private void UpdatePoints()
        {
            if (_points == null)
            {
                _points = new List<PointShape>();
                GetPoints(_points);
            }
            else
            {
                _points.Clear();
                GetPoints(_points);
            }
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawPath(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    UpdatePoints();

                    foreach (var point in _points)
                    {
                        point.DrawShape(dc, renderer);
                    }
                }
                else
                {
                    UpdatePoints();

                    foreach (var point in _points)
                    {
                        if (renderer.State.SelectedShapes.Contains(point))
                        {
                            point.DrawShape(dc, renderer);
                        }
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Bind(dataFlow, db, record);
            }
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            UpdatePoints();

            foreach (var point in _points)
            {
                point.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Select(selection);
            }
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Deselect(selection);
            }
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var figure in Geometry.Figures)
            {
                figure.GetPoints(points);
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (Geometry != null)
            {
                isDirty |= Geometry.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (Geometry != null)
            {
                Geometry.Invalidate();
            }
        }

        public string ToXamlString()
            => Geometry?.ToXamlString();

        public string ToSvgString()
            => Geometry?.ToSvgString();
    }
}
