using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XLine : XShape
    {
        private XStyle _style;
        private XPoint _start;
        private XPoint _end;

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

        public XPoint Start
        {
            get { return _start; }
            set
            {
                if (value != _start)
                {
                    _start = value;
                    Notify("Start");
                }
            }
        }

        public XPoint End
        {
            get { return _end; }
            set
            {
                if (value != _end)
                {
                    _end = value;
                    Notify("End");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this, 0, 0);
        }

        public static XLine Create(
            double x1, double y1,
            double x2, double y2,
            XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x1, y1),
                End = XPoint.Create(x2, y2)
            };
        }

        public static XLine Create(
            double x, double y,
            XStyle style)
        {
            return Create(x, y, x, y, style);
        }
    }
}
