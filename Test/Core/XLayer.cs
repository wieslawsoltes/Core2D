using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XLayer : ILayer
    {
        public string Name { get; set; }
        public IList<XShape> Shapes { get; set; }
        public Action Invalidate { get; set; }
    }
}
