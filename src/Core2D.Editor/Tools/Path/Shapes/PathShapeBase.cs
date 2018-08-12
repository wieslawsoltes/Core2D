// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Path.Shapes
{
    internal abstract class PathShapeBase : IBaseShape
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }
        public string Name { get; set; }
        public IBaseShape Owner { get; set; }
        public ShapeState State { get; set; }
        public Context Data { get; set; }
        public ShapeStyle Style { get; set; }
        public bool IsStroked { get; set; }
        public bool IsFilled { get; set; }
        public MatrixObject Transform { get; set; }

        public void MarkAsDirty(bool value)
        {
        }

        public void Notify([CallerMemberName] string propertyName = null)
        {
        }

        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return true;
        }

        public object BeginTransform(object dc, ShapeRenderer renderer)
        {
            return null;
        }

        public void EndTransform(object dc, ShapeRenderer renderer, object state)
        {
        }

        public void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
        }

        public bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        public void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
        }

        public void Select(ISet<IBaseShape> selected)
        {
        }

        public void Deselect(ISet<IBaseShape> selected)
        {
        }

        public object Copy(IDictionary<object, object> shared)
        {
            return null;
        }

        public IEnumerable<IPointShape> GetPoints()
        {
            yield return null;
        }
    }
}
