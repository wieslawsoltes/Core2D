using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static XColor Create(byte a, byte r, byte g, byte b)
        {
            return new XColor() { A = a, R = r, G = g, B = b };
        }
    }
}
