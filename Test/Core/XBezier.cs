using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XBezier : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Point1 { get; set; }
        public XPoint Point2 { get; set; }
        public XPoint Point3 { get; set; }
        public XPoint Point4 { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            XStyle style,
            bool isFilled = false)
        {
            return new XBezier()
            {
                Style = style,
                Point1 = XPoint.Create(x1, y1),
                Point2 = XPoint.Create(x2, y2),
                Point3 = XPoint.Create(x3, y3),
                Point4 = XPoint.Create(x4, y4),
                IsFilled = isFilled
            };
        }

        public static XBezier Create(
            double x, double y,
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, x, y, x, y, style, isFilled);
        }
    }
}
