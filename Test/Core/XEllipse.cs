using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XEllipse : XShape
    {
        public XStyle Style { get; set; }
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XEllipse Create(
            double x1, double y1,
            double x2, double y2,
            XStyle style,
            bool isFilled = false)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }

        public static XEllipse Create(
            double x, double y,
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }
}
