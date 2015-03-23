using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XPoint : XShape
    {
        private XShape _shape;
        private double _x;
        private double _y;

        public XShape Shape
        {
            get { return _shape; }
            set
            {
                if (value != _shape)
                {
                    _shape = value;
                    Notify("Shape");
                }
            }
        }

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

        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            if (_shape != null)
            {
                _shape.Draw(dc, renderer, dx, dy);
            }
        }

        public static XPoint Create(double x, double y, XShape shape)
        {
            return new XPoint() { X = x, Y = y, Shape = shape };
        }
    }
}
