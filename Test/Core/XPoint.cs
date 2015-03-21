using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XPoint : XObject
    {
        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    Notify("X");
                }
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    Notify("Y");
                }
            }
        }

        public static XPoint Create(double x, double y)
        {
            return new XPoint() { X = x, Y = y };
        }
    }
}
