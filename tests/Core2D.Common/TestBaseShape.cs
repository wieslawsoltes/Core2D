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

namespace Core2D.Common
{
    public abstract class TestBaseShape : IBaseShape
    {
        public abstract Type TargetType { get; }
        public IBaseShape Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IShapeState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IContext Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IShapeStyle Style { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsStroked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMatrixObject Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        public object BeginTransform(object dc, IShapeRenderer renderer)
        {
            throw new NotImplementedException();
        }

        public object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void Deselect(ISet<IBaseShape> selected)
        {
            throw new NotImplementedException();
        }

        public void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            throw new NotImplementedException();
        }

        public void EndTransform(object dc, IShapeRenderer renderer, object state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPointShape> GetPoints()
        {
            throw new NotImplementedException();
        }

        public bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            throw new NotImplementedException();
        }

        public void MarkAsDirty(bool value)
        {
            throw new NotImplementedException();
        }

        public void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            throw new NotImplementedException();
        }

        public void Notify([CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }

        public void Select(ISet<IBaseShape> selected)
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }
    }
}
