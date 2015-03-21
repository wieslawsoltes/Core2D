using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XStyle
    {
        public string Name { get; set; }
        public XColor Stroke { get; set; }
        public XColor Fill { get; set; }
        public double Thickness { get; set; }

        public static XStyle Create(
            string name,
            byte sa, byte sr, byte sg, byte sb,
            byte fa, byte fr, byte fg, byte fb,
            double thickness)
        {
            return new XStyle()
            {
                Name = name,
                Stroke = XColor.Create(sa, sr, sg, sb),
                Fill = XColor.Create(fa, fr, fg, fb),
                Thickness = thickness
            };
        }
    }
}
