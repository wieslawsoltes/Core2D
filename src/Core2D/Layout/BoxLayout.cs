using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model.History;
using Core2D.ViewModels.Shapes;

namespace Core2D.Layout
{
    public static class BoxLayout
    {
        public static void Stack(IEnumerable<BaseShapeViewModel> shapes, StackMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length < 2)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            switch (mode)
            {
                case StackMode.Horizontal:
                    {
                        boxes.Sort(ShapeBox.CompareLeft);
                        decimal offset = boxes[0].Bounds.Left + boxes[0].Bounds.Width;
                        for (int i = 1; i <= boxes.Count - 1; i++)
                        {
                            var box = boxes[i];
                            decimal dx = offset - box.Bounds.Left;
                            decimal dy = 0m;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Width;
                        }
                    }
                    break;
                case StackMode.Vertical:
                    {
                        boxes.Sort(ShapeBox.CompareTop);
                        decimal offset = boxes[0].Bounds.Top + boxes[0].Bounds.Height;
                        for (int i = 1; i <= boxes.Count - 1; i++)
                        {
                            var box = boxes[i];
                            decimal dx = 0m;
                            decimal dy = offset - box.Bounds.Top;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Height;
                        }
                    }
                    break;
            }
        }

        public static void Distribute(IEnumerable<BaseShapeViewModel> shapes, DistributeMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 2)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            decimal sw = 0m;
            decimal sh = 0m;

            foreach (var box in boxes)
            {
                sw += box.Bounds.Width;
                sh += box.Bounds.Height;
            }

            decimal gaph = (groupBox.Bounds.Width - sw) / (groupBox.Boxes.Length - 1);
            decimal gapv = (groupBox.Bounds.Height - sh) / (groupBox.Boxes.Length - 1);

            switch (mode)
            {
                case DistributeMode.Horizontal:
                    {
                        boxes.Sort(ShapeBox.CompareLeft);
                        decimal offset = boxes[0].Bounds.Left + boxes[0].Bounds.Width + gaph;
                        for (int i = 1; i <= boxes.Count - 2; i++)
                        {
                            var box = boxes[i];
                            decimal dx = offset - box.Bounds.Left;
                            decimal dy = 0m;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Width + gaph;
                        }
                    }
                    break;
                case DistributeMode.Vertical:
                    {
                        boxes.Sort(ShapeBox.CompareTop);
                        decimal offset = boxes[0].Bounds.Top + boxes[0].Bounds.Height + gapv;
                        for (int i = 1; i <= boxes.Count - 2; i++)
                        {
                            var box = boxes[i];
                            decimal dx = 0m;
                            decimal dy = offset - box.Bounds.Top;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Height + gapv;
                        }
                    }
                    break;
            }
        }

        public static void Align(IEnumerable<BaseShapeViewModel> shapes, AlignMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 1)
            {
                return;
            }

            foreach (var box in groupBox.Boxes)
            {
                decimal dx = 0m;
                decimal dy = 0m;

                switch (mode)
                {
                    case AlignMode.Left:
                        dx = groupBox.Bounds.Left - box.Bounds.Left;
                        break;
                    case AlignMode.Centered:
                        dx = groupBox.Bounds.CenterX - ((box.Bounds.Left + box.Bounds.Right) / 2m);
                        break;
                    case AlignMode.Right:
                        dx = groupBox.Bounds.Right - box.Bounds.Right;
                        break;
                    case AlignMode.Top:
                        dy = groupBox.Bounds.Top - box.Bounds.Top;
                        break;
                    case AlignMode.Center:
                        dy = groupBox.Bounds.CenterY - ((box.Bounds.Top + box.Bounds.Bottom) / 2m);
                        break;
                    case AlignMode.Bottom:
                        dy = groupBox.Bounds.Bottom - box.Bounds.Bottom;
                        break;
                }

                if (dx != 0m || dy != 0m)
                {
                    box.MoveByWithHistory(dx, dy, history);
                }
            }
        }

        public static void Flip(IEnumerable<BaseShapeViewModel> shapes, FlipMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 0)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            switch (mode)
            {
                case FlipMode.Horizontal:
                    {
                        var previous = new List<(PointShapeViewModel point, decimal x)>();
                        var next = new List<(PointShapeViewModel point, decimal x)>();

                        foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
                        {
                            decimal x = groupBox.Bounds.Left + (groupBox.Bounds.Width + groupBox.Bounds.Left) - (decimal)point.X;
                            previous.Add((point, (decimal)point.X));
                            next.Add((point, x));
                            point.X = (double)x;
                        }

                        history.Snapshot(previous, next, (p) => previous.ForEach(p => p.point.X = (double)p.x));
                    }
                    break;
                case FlipMode.Vertical:
                    {
                        var previous = new List<(PointShapeViewModel point, decimal y)>();
                        var next = new List<(PointShapeViewModel point, decimal y)>();

                        foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
                        {
                            decimal y = groupBox.Bounds.Top + (groupBox.Bounds.Height + groupBox.Bounds.Top) - (decimal)point.Y;
                            previous.Add((point, (decimal)point.Y));
                            next.Add((point, y));
                            point.Y = (double)y;
                        }

                        history.Snapshot(previous, next, (p) => previous.ForEach(p => p.point.Y = (double)p.y));
                    }
                    break;
            }
        }

        public static void Rotate(IEnumerable<BaseShapeViewModel> shapes, decimal angle, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 0)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            var previous = new List<(PointShapeViewModel point, decimal x, decimal y)>();
            var next = new List<(PointShapeViewModel point, decimal x, decimal y)>();

            var radians = angle * (decimal)Math.PI / 180m;
            var centerX = groupBox.Bounds.CenterX;
            var centerY = groupBox.Bounds.CenterY;

            foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
            {
                PointUtil.Rotate(point, radians, centerX, centerY, out var x, out var y);
                previous.Add((point, (decimal)point.X, (decimal)point.Y));
                next.Add((point, x, y));
                point.X = (double)x;
                point.Y = (double)y;
            }

            history.Snapshot(previous, next, (p) => previous.ForEach(p =>
            {
                p.point.X = (double)p.x;
                p.point.Y = (double)p.y;
            }));
        }
    }
}
