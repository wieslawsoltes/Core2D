// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Shape;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public abstract class ConnectableShape : BaseShape, IConnectable
    {
        private ObservableCollection<PointShape> _points;

        public ObservableCollection<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ConnectableShape()
        {
            _points = new ObservableCollection<PointShape>();
        }

        public ConnectableShape(ObservableCollection<PointShape> points)
        {
            this.Points = points;
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            return base.Invalidate(renderer, dx, dy);
        }

        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            foreach (var point in Points)
            {
                if (renderer.Selected.Contains(point))
                {
                    point.Draw(dc, renderer, dx, dy, db , r);
                }
            }
        }

        public override void Move(ISelection selection, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (!selection.Selected.Contains(point))
                {
                    point.Move(selection, dx, dy);
                }
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);

            foreach (var point in Points)
            {
                point.Select(selection);
            }
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            foreach (var point in Points)
            {
                point.Deselect(selection);
            }
        }

        private bool CanConnect(PointShape point)
        {
            return _points.Contains(point) == false;
        }

        public virtual bool Connect(PointShape point, PointShape target)
        {
            if (CanConnect(point))
            {
                int index = _points.IndexOf(target);
                if (index >= 0)
                {
                    Debug.WriteLine($"ConnectableShape Connected to Points");
                    _points[index] = point;
                    return true;
                }
            }
            return false;
        }

        public virtual bool Disconnect(PointShape point, out PointShape result)
        {
            result = null;
            return false;
        }

        public virtual bool Disconnect()
        {
            bool result = false;

            for (int i = 0; i < _points.Count; i++)
            {
                Debug.WriteLine($"{nameof(ConnectableShape)}: Disconnected from {nameof(Points)} #{i}");
                _points[i] = (PointShape)_points[i].Copy(null);
                result = true;
            }

            return result;
        }
    }
}
