// Random Lines

#r "System.Security.Cryptography.Csp"
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

double ToDouble(byte[] data, int index, double size)
{
    var ul = BitConverter.ToUInt64(data, index) / (1 << 11);
    return size * (ul / (double)(1UL << 53));
}

void RandomLines()
{
    double width = Editor.Project.CurrentContainer.Width;
    double height = Editor.Project.CurrentContainer.Height;

    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] data = new byte[32];
        for (int i = 0; i < 1000; i++)
        {
            rng.GetBytes(data);
            double x0 = ToDouble(data, 0, width);
            double y0 = ToDouble(data, 8, height);
            double x1 = ToDouble(data, 16, width);
            double y1 = ToDouble(data, 24, height);
            Editor.ShapeFactory.Line(x0, y0, x1, y1, true);
            //Console.WriteLine($"[{x0};{y0}] [{x1};{y1}]");
        }
    }
}

Task.Factory.StartNew(() => RandomLines());
