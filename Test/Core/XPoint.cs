using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static XPoint Create(double x, double y)
        {
            return new XPoint() { X = x, Y = y };
        }
    }
}
