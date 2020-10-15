using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Common.UnitTests
{
    public abstract class TestBaseShape : BaseShape
    {
        public abstract Type TargetType { get; }
        public ObservableObject Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ShapeState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Context Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ShapeStyle Style { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsStroked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public virtual bool IsDirty()
        {
            throw new NotImplementedException();
        }

        public virtual void Invalidate()
        {
            throw new NotImplementedException();
        }

        public object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void Deselect(ISelection selection)
        {
            throw new NotImplementedException();
        }

        public void DrawShape(object dc, IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public void DrawPoints(object dc, IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public void GetPoints(IList<PointShape> points)
        {
            throw new NotImplementedException();
        }

        public bool Invalidate(IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public void Bind(DataFlow dataFlow, object db, object r)
        {
            throw new NotImplementedException();
        }

        public void SetProperty(string name, object value)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(string name)
        {
            throw new NotImplementedException();
        }

        public void Move(ISelection selection, decimal dx, decimal dy)
        {
            throw new NotImplementedException();
        }

        public void Notify([CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }

        public void Select(ISelection selection)
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }
    }
}
