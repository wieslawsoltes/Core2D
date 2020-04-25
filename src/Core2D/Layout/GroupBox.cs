using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Layout
{
    public struct GroupBox
    {
        public readonly ShapeBox[] Boxes;
        public Box Bounds;

        public GroupBox(IList<IBaseShape> shapes)
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

            Bounds.CenterX = (Bounds.Left + Bounds.Right) / 2.0;
            Bounds.CenterY = (Bounds.Top + Bounds.Bottom) / 2.0;
            Bounds.Width = Math.Abs(Bounds.Right - Bounds.Left);
            Bounds.Height = Math.Abs(Bounds.Bottom - Bounds.Top);
        }

        public bool IsPointMovable(IPointShape point, IBaseShape parent)
        {
            if (point.State.Flags.HasFlag(ShapeStateFlags.Locked) || (point.Owner is IBaseShape ower && ower.State.Flags.HasFlag(ShapeStateFlags.Locked)))
            {
                return false;
            }

            if (point.State.Flags.HasFlag(ShapeStateFlags.Connector) && point.Owner != parent)
            {
                return false;
            }

            return true;
        }

        public List<IPointShape> GetMovablePoints()
        {
            var points = new HashSet<IPointShape>();

            for (int i = 0; i < Boxes.Length; i++)
            {
                foreach (var point in Boxes[i].Points)
                {
                    if (IsPointMovable(point, Boxes[i].Shape))
                    {
                        points.Add(point);
                    }
                }
            }

            return new List<IPointShape>(points);
        }

        public void TransformPoints(ref Matrix2 matrix, List<IPointShape> points)
        {
            if (points == null || points.Count == 0)
            {
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var transformed = Matrix2.TransformPoint(matrix, new Point2(point.X, point.Y));
                point.X = transformed.X;
                point.Y = transformed.Y;
            }
        }

        public void Rotate(double sx, double sy, List<IPointShape> points, ref double rotateAngle)
        {
            var centerX = Bounds.CenterX;
            var centerY = Bounds.CenterY;
            var p0 = new Point2(centerX, centerY);
            var p1 = new Point2(sx, sy);
            var angle = p0.AngleBetween(p1) - 270.0;
            var delta = angle - rotateAngle;
            var radians = delta * (Math.PI / 180.0);
            var matrix = Matrix2.Rotation(radians, centerX, centerY);
            TransformPoints(ref matrix, points);
            rotateAngle = angle;
            Update();
        }

        public void Translate(double dx, double dy, List<IPointShape> points)
        {
            double offsetX = dx;
            double offsetY = dy;
            var matrix = Matrix2.Translate(offsetX, offsetY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleTop(double dy, List<IPointShape> points)
        {
            var oldSize = Bounds.Height;
            var newSize = oldSize - dy;
            if (newSize <= 0 || oldSize <= 0)
            {
                Translate(0.0, dy, points);
                return;
            }
            var scaleX = 1.0;
            var scaleY = newSize / oldSize;
            var centerX = Bounds.CenterX;
            var centerY = Bounds.Bottom;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleBottom(double dy, List<IPointShape> points)
        {
            var oldSize = Bounds.Height;
            var newSize = oldSize + dy;
            if (newSize <= 0 || oldSize <= 0)
            {
                Translate(0.0, dy, points);
                return;
            }
            var scaleX = 1.0;
            var scaleY = newSize / oldSize;
            var centerX = Bounds.CenterX;
            var centerY = Bounds.Top;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleLeft(double dx, List<IPointShape> points)
        {
            var oldSize = Bounds.Width;
            var newSize = oldSize - dx;
            if (newSize <= 0 || oldSize <= 0)
            {
                Translate(dx, 0.0, points);
                return;
            }
            var scaleX = newSize / oldSize;
            var scaleY = 1.0;
            var centerX = Bounds.Right;
            var centerY = Bounds.CenterY;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }

        public void ScaleRight(double dx, List<IPointShape> points)
        {
            var oldSize = Bounds.Width;
            var newSize = oldSize + dx;
            if (newSize <= 0 || oldSize <= 0)
            {
                Translate(dx, 0.0, points);
                return;
            }
            var scaleX = newSize / oldSize;
            var scaleY = 1.0;
            var centerX = Bounds.Left;
            var centerY = Bounds.CenterY;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix, points);
            Update();
        }
    }
}
