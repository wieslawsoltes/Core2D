using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    public partial class BaseShape : ViewModelBase, IDataObject
    {
        private IDictionary<string, object> _propertyCache = new Dictionary<string, object>();

        [AutoNotify] private ShapeStateFlags _state;
        [AutoNotify] private ShapeStyle _style;
        [AutoNotify] private bool _isStroked;
        [AutoNotify] private bool _isFilled;
        [AutoNotify] private ImmutableArray<Property> _properties;
        [AutoNotify] private Record _record;
        [AutoNotify(SetterModifier = AccessModifier.None)] private Type _targetType;

        protected BaseShape(Type targetType)
        {
            _targetType = targetType;
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var property in Properties)
            {
                isDirty |= property.IsDirty();
            }

            if (Record != null)
            {
                isDirty |= Record.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var property in Properties)
            {
                property.Invalidate();
            }

            Record?.Invalidate();
        }

        public virtual void DrawShape(object dc, IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public virtual void DrawPoints(object dc, IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public virtual bool Invalidate(IShapeRenderer renderer)
        {
            return false;
        }

        public virtual void Bind(DataFlow dataFlow, object db, object r)
        {
            throw new NotImplementedException();
        }

        public virtual void SetProperty(string name, object value)
        {
            _propertyCache[name] = value;
        }

        public virtual object GetProperty(string name)
        {
            if (_propertyCache.ContainsKey(name))
            {
                return _propertyCache[name];
            }
            return null;
        }

        public virtual void Move(ISelection selection, decimal dx, decimal dy)
        {
            throw new NotImplementedException();
        }

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

        public virtual void GetPoints(IList<PointShape> points)
        {
            throw new NotImplementedException();
        }
    }
}
