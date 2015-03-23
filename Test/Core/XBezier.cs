using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XBezier : XShape
    {
        private XStyle _style;
        private XPoint _point1;
        private XPoint _point2;
        private XPoint _point3;
        private XPoint _point4;
        private bool _isFilled;

        public XStyle Style
        {
            get { return _style; }
            set
            {
                if (value != _style)
                {
                    _style = value;
                    Notify("Style");
                }
            }
        }

        public XPoint Point1
        {
            get { return _point1; }
            set
            {
                if (value != _point1)
                {
                    _point1 = value;
                    Notify("Point1");
                }
            }
        }

        public XPoint Point2
        {
            get { return _point2; }
            set
            {
                if (value != _point2)
                {
                    _point2 = value;
                    Notify("Point2");
                }
            }
        }

        public XPoint Point3
        {
            get { return _point3; }
            set
            {
                if (value != _point3)
                {
                    _point3 = value;
                    Notify("Point3");
                }
            }
        }

        public XPoint Point4
        {
            get { return _point4; }
            set
            {
                if (value != _point4)
                {
                    _point4 = value;
                    Notify("Point4");
                }
            }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            renderer.Draw(dc, this, dx, dy);
            if (renderer.DrawPoints)
            {
                _point1.Draw(dc, renderer, _point1.X, _point1.Y);
                _point2.Draw(dc, renderer, _point2.X, _point2.Y);
                _point3.Draw(dc, renderer, _point3.X, _point3.Y);
                _point4.Draw(dc, renderer, _point4.X, _point4.Y);
            }
        }

        public static XBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            XStyle style,
            XShape point,
            bool isFilled = false)
        {
            return new XBezier()
            {
                Style = style,
                Point1 = XPoint.Create(x1, y1, point),
                Point2 = XPoint.Create(x2, y2, point),
                Point3 = XPoint.Create(x3, y3, point),
                Point4 = XPoint.Create(x4, y4, point),
                IsFilled = isFilled
            };
        }

        public static XBezier Create(
            double x, double y,
            XStyle style,
            XShape point,
            bool isFilled = false)
        {
            return Create(x, y, x, y, x, y, x, y, style, point, isFilled);
        }
    }
}
