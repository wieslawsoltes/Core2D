using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public abstract class XShape
    {
        public abstract void Draw(object dc, IRenderer renderer);
    }
}
