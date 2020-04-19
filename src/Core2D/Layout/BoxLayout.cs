using System;
using System.Collections.Generic;
using Core2D.History;
using Core2D.Shapes;
using System.Linq;

namespace Core2D.Layout
{
    public static class BoxLayout
    {
        public static void Stack(IEnumerable<IBaseShape> shapes, StackMode mode, IHistory history)
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
                        double offset = boxes[0].Bounds.Left + boxes[0].Bounds.Width;
                        for (int i = 1; i <= boxes.Count - 1; i++)
                        {
                            var box = boxes[i];
                            double dx = offset - box.Bounds.Left;
                            double dy = 0.0;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Width;
                        }
                    }
                    break;
                case StackMode.Vertical:
                    {
                        boxes.Sort(ShapeBox.CompareTop);
                        double offset = boxes[0].Bounds.Top + boxes[0].Bounds.Height;
                        for (int i = 1; i <= boxes.Count - 1; i++)
                        {
                            var box = boxes[i];
                            double dx = 0.0;
                            double dy = offset - box.Bounds.Top;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Height;
                        }
                    }
                    break;
            }
        }

        public static void Distribute(IEnumerable<IBaseShape> shapes, DistributeMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 2)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            double sw = 0.0;
            double sh = 0.0;

            foreach (var box in boxes)
            {
                sw += box.Bounds.Width;
                sh += box.Bounds.Height;
            }

            double gaph = (groupBox.Bounds.Width - sw) / (groupBox.Boxes.Length - 1);
            double gapv = (groupBox.Bounds.Height - sh) / (groupBox.Boxes.Length - 1);

            switch (mode)
            {
                case DistributeMode.Horizontal:
                    {

                        boxes.Sort(ShapeBox.CompareLeft);
                        double offset = boxes[0].Bounds.Left + boxes[0].Bounds.Width + gaph;
                        for (int i = 1; i <= boxes.Count - 2; i++)
                        {
                            var box = boxes[i];
                            double dx = offset - box.Bounds.Left;
                            double dy = 0.0;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Width + gaph;
                        }
                    }
                    break;
                case DistributeMode.Vertical:
                    {
                        boxes.Sort(ShapeBox.CompareTop);
                        double offset = boxes[0].Bounds.Top + boxes[0].Bounds.Height + gapv;
                        for (int i = 1; i <= boxes.Count - 2; i++)
                        {
                            var box = boxes[i];
                            double dx = 0.0;
                            double dy = offset - box.Bounds.Top;
                            box.MoveByWithHistory(dx, dy, history);
                            offset += box.Bounds.Height + gapv;
                        }
                    }
                    break;
            }
        }

        public static void Align(IEnumerable<IBaseShape> shapes, AlignMode mode, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 1)
            {
                return;
            }

            foreach (var box in groupBox.Boxes)
            {
                double dx = 0.0;
                double dy = 0.0;

                switch (mode)
                {
                    case AlignMode.Left:
                        dx = groupBox.Bounds.Left - box.Bounds.Left;
                        break;
                    case AlignMode.Centered:
                        dx = groupBox.Bounds.CenterX - ((box.Bounds.Left + box.Bounds.Right) / 2.0);
                        break;
                    case AlignMode.Right:
                        dx = groupBox.Bounds.Right - box.Bounds.Right;
                        break;
                    case AlignMode.Top:
                        dy = groupBox.Bounds.Top - box.Bounds.Top;
                        break;
                    case AlignMode.Center:
                        dy = groupBox.Bounds.CenterY - ((box.Bounds.Top + box.Bounds.Bottom) / 2.0);
                        break;
                    case AlignMode.Bottom:
                        dy = groupBox.Bounds.Bottom - box.Bounds.Bottom;
                        break;
                }

                if (dx != 0.0 || dy != 0.0)
                {
                    box.MoveByWithHistory(dx, dy, history);
                }
            }
        }

        public static void Flip(IEnumerable<IBaseShape> shapes, FlipMode mode, IHistory history)
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
                        var previous = new List<(IPointShape point, double x)>();
                        var next = new List<(IPointShape point, double x)>();

                        foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
                        {
                            double x = groupBox.Bounds.Left + (groupBox.Bounds.Width + groupBox.Bounds.Left) - point.X;
                            previous.Add((point, point.X));
                            next.Add((point, x));
                            point.X = x;
                        }

                        history.Snapshot(previous, next, (p) => previous.ForEach(p => p.point.X = p.x));
                    }
                    break;
                case FlipMode.Vertical:
                    {
                        var previous = new List<(IPointShape point, double y)>();
                        var next = new List<(IPointShape point, double y)>();

                        foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
                        {
                            double y = groupBox.Bounds.Top + (groupBox.Bounds.Height + groupBox.Bounds.Top) - point.Y;
                            previous.Add((point, point.Y));
                            next.Add((point, y));
                            point.Y = y;
                        }

                        history.Snapshot(previous, next, (p) => previous.ForEach(p => p.point.Y = p.y));
                    }
                    break;
            }
        }

        public static void Rotate(IEnumerable<IBaseShape> shapes, double angle, IHistory history)
        {
            var groupBox = new GroupBox(shapes.ToList());
            if (groupBox.Boxes.Length <= 0)
            {
                return;
            }

            var boxes = groupBox.Boxes.ToList();

            var previous = new List<(IPointShape point, double x, double y)>();
            var next = new List<(IPointShape point, double x, double y)>();

            var radians = angle * Math.PI / 180.0;
            var centerX = groupBox.Bounds.CenterX;
            var centerY = groupBox.Bounds.CenterY;

            foreach (var point in boxes.SelectMany(box => box.Points).Distinct())
            {
                PointUtil.Rotate(point, radians, centerX, centerY, out var x, out var y);
                previous.Add((point, point.X, point.Y));
                next.Add((point, x, y));
                point.X = x;
                point.Y = y;
            }

            history.Snapshot(previous, next, (p) => previous.ForEach(p =>
            {
                p.point.X = p.x;
                p.point.Y = p.y;
            }));
        }
    }
}
