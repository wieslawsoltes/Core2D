using System;
using System.Collections.Generic;
using Core2D.Shapes;

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

            Bounds.Left = double.MaxValue;
            Bounds.Top = double.MaxValue;
            Bounds.Right = double.MinValue;
            Bounds.Bottom = double.MinValue;

            for (int i = 0; i < Boxes.Length; i++)
            {
                var box = Boxes[i];
                Bounds.Left = Math.Min(Bounds.Left, box.Bounds.Left);
                Bounds.Top = Math.Min(Bounds.Top, box.Bounds.Top);
                Bounds.Right = Math.Max(Bounds.Right, box.Bounds.Right);
                Bounds.Bottom = Math.Max(Bounds.Bottom, box.Bounds.Bottom);
            }

            Bounds.CenterX = (Bounds.Left + Bounds.Right) / 2.0;
            Bounds.CenterY = (Bounds.Top + Bounds.Bottom) / 2.0;
            Bounds.Width = Math.Abs(Bounds.Right - Bounds.Left);
            Bounds.Height = Math.Abs(Bounds.Bottom - Bounds.Top);
        }
    }
}
