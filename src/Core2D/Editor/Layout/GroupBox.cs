using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Editor.Layout
{
    public struct GroupBox
    {
        public readonly List<ShapeBox> Boxes;
        public Box Bounds;

        public GroupBox(List<IBaseShape> shapes)
        {
            Boxes = new List<ShapeBox>(shapes.Count);

            for (int i = 0; i < shapes.Count; i++)
            {
                Boxes.Add(new ShapeBox(shapes[i]));
            }

            Bounds = new Box();

            Update(ref Bounds);
        }

        public void Update(ref Box bounds)
        {
            for (int i = 0; i < Boxes.Count; i++)
            {
                var box = Boxes[i];
                box.Update(ref box.Bounds);
            }

            bounds.Left = double.MaxValue;
            bounds.Top = double.MaxValue;
            bounds.Right = double.MinValue;
            bounds.Bottom = double.MinValue;

            foreach (var box in Boxes)
            {
                bounds.Left = Math.Min(bounds.Left, box.Bounds.Left);
                bounds.Top = Math.Min(bounds.Top, box.Bounds.Top);
                bounds.Right = Math.Max(bounds.Right, box.Bounds.Right);
                bounds.Bottom = Math.Max(bounds.Bottom, box.Bounds.Bottom);
            }

            bounds.CenterX = (bounds.Left + bounds.Right) / 2.0;
            bounds.CenterY = (bounds.Top + bounds.Bottom) / 2.0;
            bounds.Width = Math.Abs(bounds.Right - bounds.Left);
            bounds.Height = Math.Abs(bounds.Bottom - bounds.Top);
        }
    }
}
