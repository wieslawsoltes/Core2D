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
        void Draw(object dc, XLine line);
        void Draw(object dc, XRectangle rectangle);
        void Draw(object dc, XEllipse ellipse);
        void Draw(object dc, XBezier bezier);
    }
}
