// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Shapes
{
    public abstract class BoxShape : ConnectableShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;

        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public BoxShape()
            : base()
        {
        }

        public BoxShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return TopLeft;
            yield return BottomRight;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            result |= _topLeft?.Invalidate(renderer, dx, dy) ?? false;
            result |= _bottomRight?.Invalidate(renderer, dx, dy) ?? false;

            return result;
        }

        public override void Move(ISelection selection, double dx, double dy)
        {
            if (!selection.Selected.Contains(_topLeft))
            {
                _topLeft.Move(selection, dx, dy);
            }

            if (!selection.Selected.Contains(_bottomRight))
            {
                _bottomRight.Move(selection, dx, dy);
            }

            base.Move(selection, dx, dy);
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

        private bool CanConnect(PointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
                    Debug.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(TopLeft)}");
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
                    Debug.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(BottomRight)}");
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
                result = (PointShape)point.Copy(null);
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
                result = (PointShape)point.Copy(null);
                this.BottomRight = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.TopLeft != null)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
                this.TopLeft = (PointShape)this.TopLeft.Copy(null);
                result = true;
            }

            if (this.BottomRight != null)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
                this.BottomRight = (PointShape)this.BottomRight.Copy(null);
                result = true;
            }

            return result;
        }
    }
}
