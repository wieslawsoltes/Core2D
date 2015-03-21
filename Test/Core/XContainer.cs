using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XContainer : IContainer
    {
        public IList<ILayer> Layers { get; set; }
        public ILayer CurrentLayer { get; set; }
        public ILayer WorkingLayer { get; set; }
        public IList<XStyle> Styles { get; set; }
        public XStyle CurrentStyle { get; set; }
        public XShape CurrentShape { get; set; }
    }
}
