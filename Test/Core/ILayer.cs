using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface ILayer
    {
        string Name { get; set; }
        bool IsVisible { get; set; }
        IList<XShape> Shapes { get; set; }
        void SetInvalidate(Action invalidate);
        void Invalidate();
    }
}
