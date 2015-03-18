using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class Demo
    {
        public static void Create(IContainer container, double width, double height, int shapes)
        {
            var style1 = container.Styles[1];
            var style2 = container.Styles[2];
            var style3 = container.Styles[3];
            var style4 = container.Styles[4];

            var layer1 = container.Layers[0];
            var layer2 = container.Layers[1];
            var layer3 = container.Layers[2];
            var layer4 = container.Layers[3];

            var rand = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var l = XLine.Create(x1, y1, x2, y2, style1);
                layer1.Shapes.Add(l);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var r = XRectangle.Create(x1, y1, x2, y2, style2);
                layer2.Shapes.Add(r);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var e = XEllipse.Create(x1, y1, x2, y2, style3);
                layer3.Shapes.Add(e);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                double x3 = rand.NextDouble() * width;
                double y3 = rand.NextDouble() * height;
                double x4 = rand.NextDouble() * width;
                double y4 = rand.NextDouble() * height;
                var b = XBezier.Create(x1, y1, x2, y2, x3, y3, x4, y4, style4);
                layer4.Shapes.Add(b);
            }

            foreach (var layer in container.Layers)
                layer.Invalidate();
            container.WorkingLayer.Invalidate();
        }
    }
}
