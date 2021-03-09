﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Shapes
{
    public partial class BaseShapeViewModel : ViewModelBase, IDataObject
    {
        private readonly IDictionary<string, object> _propertyCache = new Dictionary<string, object>();

        [AutoNotify] protected ShapeStateFlags _state;
        [AutoNotify] protected ShapeStyleViewModel _style;
        [AutoNotify] protected bool _isStroked;
        [AutoNotify] protected bool _isFilled;
        [AutoNotify] protected ImmutableArray<PropertyViewModel> _properties;
        [AutoNotify] protected RecordViewModel _record;
        [AutoNotify(SetterModifier = AccessModifier.None)] protected Type _targetType;

        protected BaseShapeViewModel(IServiceProvider serviceProvider, Type targetType) : base(serviceProvider)
        {
            _targetType = targetType;
        }

        public void ToggleDefaultShapeState()
        {
            State ^= ShapeStateFlags.Default;
        }

        public void ToggleVisibleShapeState()
        {
            State ^= ShapeStateFlags.Visible;
        }

        public void TogglePrintableShapeState()
        {
            State ^= ShapeStateFlags.Printable;
        }

        public void ToggleLockedShapeState()
        {
            State ^= ShapeStateFlags.Locked;
        }

        public void ToggleSizeShapeState()
        {
            State ^= ShapeStateFlags.Size;
        }

        public void ToggleThicknessShapeState()
        {
            State ^= ShapeStateFlags.Thickness;
        }

        public void ToggleConnectorShapeState()
        {
            State ^= ShapeStateFlags.Connector;
        }

        public void ToggleNoneShapeState()
        {
            State ^= ShapeStateFlags.None;
        }

        public void ToggleStandaloneShapeState()
        {
            State ^= ShapeStateFlags.Standalone;
        }

        public void ToggleInputShapeState()
        {
            State ^= ShapeStateFlags.Input;
        }

        public void ToggleOutputShapeState()
        {
            State ^= ShapeStateFlags.Output;
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var property in _properties)
            {
                isDirty |= property.IsDirty();
            }

            if (Record is { })
            {
                isDirty |= Record.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var property in _properties)
            {
                property.Invalidate();
            }

            Record?.Invalidate();
        }

        public virtual void DrawShape(object dc, IShapeRenderer renderer, ISelection selection)
        {
            throw new NotImplementedException();
        }

        public virtual void DrawPoints(object dc, IShapeRenderer renderer, ISelection selection)
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
            if (selection?.SelectedShapes is { })
            {
                if (!selection.SelectedShapes.Contains(this))
                {
                    selection.SelectedShapes.Add(this);
                }
            }
        }

        public virtual void Deselect(ISelection selection)
        {
            if (selection?.SelectedShapes is { })
            {
                if (selection.SelectedShapes.Contains(this))
                {
                    selection.SelectedShapes.Remove(this);
                }
            }
        }

        public virtual void GetPoints(IList<PointShapeViewModel> points)
        {
            throw new NotImplementedException();
        }
    }
}