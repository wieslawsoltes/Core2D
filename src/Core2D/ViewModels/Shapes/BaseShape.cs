using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    [DataContract(IsReference = true)]
    public abstract class BaseShape : ObservableObject
    {
        private IDictionary<string, object> _properties = new Dictionary<string, object>();
        private ShapeState _state;
        private ShapeStyle _style;
        private bool _isStroked;
        private bool _isFilled;
        private Context _data;

        [IgnoreDataMember]
        public abstract Type TargetType { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public virtual ShapeState State
        {
            get => _state;
            set => RaiseAndSetIfChanged(ref _state, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public virtual ShapeStyle Style
        {
            get => _style;
            set => RaiseAndSetIfChanged(ref _style, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public virtual bool IsStroked
        {
            get => _isStroked;
            set => RaiseAndSetIfChanged(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public virtual bool IsFilled
        {
            get => _isFilled;
            set => RaiseAndSetIfChanged(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public virtual Context Data
        {
            get => _data;
            set => RaiseAndSetIfChanged(ref _data, value);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= State.IsDirty();
            isDirty |= Data.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            State.Invalidate();
            Data.Invalidate();
        }

        public abstract void DrawShape(object dc, IShapeRenderer renderer);

        public abstract void DrawPoints(object dc, IShapeRenderer renderer);

        public virtual bool Invalidate(IShapeRenderer renderer)
        {
            return false;
        }

        public abstract void Bind(DataFlow dataFlow, object db, object r);

        public virtual void SetProperty(string name, object value)
        {
            _properties[name] = value;
        }

        public virtual object GetProperty(string name)
        {
            if (_properties.ContainsKey(name))
            {
                return _properties[name];
            }

            return null;
        }

        public abstract void Move(ISelection selection, decimal dx, decimal dy);

        public virtual void Select(ISelection selection)
        {
            if (!selection.SelectedShapes.Contains(this))
            {
                selection.SelectedShapes.Add(this);
            }
        }

        public virtual void Deselect(ISelection selection)
        {
            if (selection.SelectedShapes.Contains(this))
            {
                selection.SelectedShapes.Remove(this);
            }
        }

        public abstract void GetPoints(IList<PointShape> points);
    }
}
