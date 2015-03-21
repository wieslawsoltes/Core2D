using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XRectangle : XShape
    {
        private XStyle _style;
        private XPoint _topLeft;
        private XPoint _bottomRight;
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

        public XPoint TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (value != _topLeft)
                {
                    _topLeft = value;
                    Notify("TopLeft");
                }
            }
        }

        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set
            {
                if (value != _bottomRight)
                {
                    _bottomRight = value;
                    Notify("BottomRight");
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

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XRectangle Create(
            double x1, double y1,
            double x2, double y2,
            XStyle style,
            bool isFilled = false)
        {
            return new XRectangle()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }

        public static XRectangle Create(
            double x, double y,
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }
}
