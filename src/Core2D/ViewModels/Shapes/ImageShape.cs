using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    [DataContract(IsReference = true)]
    public class ImageShape : BaseShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;
        private string _key;

        [IgnoreDataMember]
        public override Type TargetType => typeof(ImageShape);

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape TopLeft
        {
            get => _topLeft;
            set => RaiseAndSetIfChanged(ref _topLeft, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape BottomRight
        {
            get => _bottomRight;
            set => RaiseAndSetIfChanged(ref _bottomRight, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string Key
        {
            get => _key;
            set => RaiseAndSetIfChanged(ref _key, value);
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawImage(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _topLeft.DrawShape(dc, renderer);
                    _bottomRight.DrawShape(dc, renderer);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_topLeft))
                    {
                        _topLeft.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_bottomRight))
                    {
                        _bottomRight.DrawShape(dc, renderer);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _topLeft.Bind(dataFlow, db, record);
            _bottomRight.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            if (!TopLeft.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                TopLeft.Move(selection, dx, dy);
            }

            if (!BottomRight.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                BottomRight.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            TopLeft.Select(selection);
            BottomRight.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            TopLeft.Deselect(selection);
            BottomRight.Deselect(selection);
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(TopLeft);
            points.Add(BottomRight);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= TopLeft.IsDirty();
            isDirty |= BottomRight.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            TopLeft.Invalidate();
            BottomRight.Invalidate();
        }
    }
}
