using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface IRenderer
    {
        void ClearCache();
        void Render(object dc, ILayer layer);
        void Draw(object dc, XLine line, double dx, double dy);
        void Draw(object dc, XRectangle rectangle, double dx, double dy);
        void Draw(object dc, XEllipse ellipse, double dx, double dy);
        void Draw(object dc, XBezier bezier, double dx, double dy);
        void Draw(object dc, XQBezier qbezier, double dx, double dy);
    }
}
