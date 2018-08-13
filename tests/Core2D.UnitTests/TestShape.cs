// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.UnitTests
{
    public abstract class TestBaseShape : IBaseShape
    {
        public abstract Type TargetType { get; }
        public IBaseShape Owner { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ShapeState State { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Context Data { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Name { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ShapeStyle Style { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsStroked { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsFilled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public MatrixObject Transform { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public object BeginTransform(object dc, ShapeRenderer renderer)
        {
            throw new System.NotImplementedException();
        }

        public object Copy(IDictionary<object, object> shared)
        {
            throw new System.NotImplementedException();
        }

        public void Deselect(ISet<IBaseShape> selected)
        {
            throw new System.NotImplementedException();
        }

        public void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            throw new System.NotImplementedException();
        }

        public void EndTransform(object dc, ShapeRenderer renderer, object state)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPointShape> GetPoints()
        {
            throw new System.NotImplementedException();
        }

        public bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            throw new System.NotImplementedException();
        }

        public void MarkAsDirty(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            throw new System.NotImplementedException();
        }

        public void Notify([CallerMemberName] string propertyName = null)
        {
            throw new System.NotImplementedException();
        }

        public void Select(ISet<IBaseShape> selected)
        {
            throw new System.NotImplementedException();
        }

        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
