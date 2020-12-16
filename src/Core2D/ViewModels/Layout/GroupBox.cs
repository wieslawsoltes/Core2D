using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.ViewModels.Layout
{
    public struct GroupBox
    {
        public static void TransformPoint(ref MatrixD matrix, PointShapeViewModel point)
        {
            var transformed = MatrixD.TransformPoint(matrix, new PointD((decimal)point.X, (decimal)point.Y));
            point.X = (double)transformed.X;
            point.Y = (double)transformed.Y;
        }

        public static void TransformPoints(ref MatrixD matrix, IList<PointShapeViewModel> points)
        {
            if (points is null || points.Count == 0)
            {
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                TransformPoint(ref matrix, points[i]);
            }
        }

        public static bool IsPointMovable(PointShapeViewModel point, BaseShapeViewModel parent)
        {
            if (point.State.HasFlag(ShapeStateFlags.Locked) || (point.Owner is BaseShapeViewModel ower && ower.State.HasFlag(ShapeStateFlags.Locked)))
            {
                return false;
            }

            if (point.State.HasFlag(ShapeStateFlags.Connector) && point.Owner != parent)
            {
                return false;
            }

            return true;
        }

        public readonly ShapeBox[] Boxes;
        public Box Bounds;

        public GroupBox(IList<BaseShapeViewModel> shapes)
        {
            Boxes = new ShapeBox[shapes.Count];

            for (int i = 0; i < shapes.Count; i++)
            {
                Boxes[i] = new ShapeBox(shapes[i]);
            }

            Bounds = new Box();

            Update();
        }

        public void Update()
        {
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].Update();
            }

            for (int i = 0; i < Boxes.Length; i++)
            {
                var box = Boxes[i];
                if (i == 0)
                {
                    Bounds.Left = box.Bounds.Left;
                    Bounds.Top = box.Bounds.Top;
                    Bounds.Right = box.Bounds.Right;
                    Bounds.Bottom = box.Bounds.Bottom;
                }
                else
                {
                    Bounds.Left = Math.Min(Bounds.Left, box.Bounds.Left);
                    Bounds.Top = Math.Min(Bounds.Top, box.Bounds.Top);
                    Bounds.Right = Math.Max(Bounds.Right, box.Bounds.Right);
                    Bounds.Bottom = Math.Max(Bounds.Bottom, box.Bounds.Bottom);
                }
            }

            Bounds.CenterX = (Bounds.Left + Bounds.Right) / 2m;
            Bounds.CenterY = (Bounds.Top + Bounds.Bottom) / 2m;
            Bounds.Width = Math.Abs(Bounds.Right - Bounds.Left);
            Bounds.Height = Math.Abs(Bounds.Bottom - Bounds.Top);
        }

        public List<PointShapeViewModel> GetMovablePoints()
        {
            var points = new HashSet<PointShapeViewModel>();

            for (int i = 0; i < Boxes.Length; i++)
            {
                foreach (var point in Boxes[i].Points)
                {
                    if (IsPointMovable(point, Boxes[i]._shapeViewModel))
                    {
                        points.Add(point);
                    }
                }
            }

            return new List<PointShapeViewModel>(points);
        }

        public void Rotate(decimal sx, decimal sy, List<PointShapeViewModel> points, ref decimal rotateAngle)
        {
            var centerX = Bounds.CenterX;
            var centerY = Bounds.CenterY;
            var p0 = new PointD(centerX, centerY);
            var p1 = new PointD(sx, sy);
            var angle = p0.AngleBetween(p1) - 270m;
            var delta = angle - rotateAngle;
            var radians = delta * ((decimal)Math.PI / 180m);
            var matrix = MatrixD.Rotation(radians, centerX, centerY);
            TransformPoints(ref matrix, points);
            rotateAngle = angle;
            Update();
        }

        public void Translate(decimal dx, decimal dy, List<PointShapeViewModel> points)
        {
            decimal offsetX = dx;
            decimal offsetY = dy;
            var matrix = MatrixD.Translate(offsetX, offsetY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleTop(decimal dy, List<PointShapeViewModel> points)
        {
            var oldSize = Bounds.Height;
            var newSize = oldSize - dy;
            if (newSize <= 0m || oldSize <= 0m)
            {
                Translate(0m, dy, points);
                return;
            }
            var scaleX = 1m;
            var scaleY = newSize / oldSize;
            var centerX = Bounds.CenterX;
            var centerY = Bounds.Bottom;
            var matrix = MatrixD.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleBottom(decimal dy, List<PointShapeViewModel> points)
        {
            var oldSize = Bounds.Height;
            var newSize = oldSize + dy;
            if (newSize <= 0m || oldSize <= 0m)
            {
                Translate(0m, dy, points);
                return;
            }
            var scaleX = 1m;
            var scaleY = newSize / oldSize;
            var centerX = Bounds.CenterX;
            var centerY = Bounds.Top;
            var matrix = MatrixD.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleLeft(decimal dx, List<PointShapeViewModel> points)
        {
            var oldSize = Bounds.Width;
            var newSize = oldSize - dx;
            if (newSize <= 0m || oldSize <= 0m)
            {
                Translate(dx, 0m, points);
                return;
            }
            var scaleX = newSize / oldSize;
            var scaleY = 1m;
            var centerX = Bounds.Right;
            var centerY = Bounds.CenterY;
            var matrix = MatrixD.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleRight(decimal dx, List<PointShapeViewModel> points)
        {
            var oldSize = Bounds.Width;
            var newSize = oldSize + dx;
            if (newSize <= 0m || oldSize <= 0m)
            {
                Translate(dx, 0m, points);
                return;
            }
            var scaleX = newSize / oldSize;
            var scaleY = 1m;
            var centerX = Bounds.Left;
            var centerY = Bounds.CenterY;
            var matrix = MatrixD.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }
    }
}
