using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XGroup : XShape
    {
        public string _name;
        public IList<XShape> _shapes;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public IList<XShape> Shapes
        {
            get { return _shapes; }
            set
            {
                if (value != _shapes)
                {
                    _shapes = value;
                    Notify("Shapes");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer);
            }
        }

        public static XGroup Create(string name)
        {
            return new XGroup()
            {
                Name = name,
                Shapes = new ObservableCollection<XShape>()
            };
        }
    }
}
