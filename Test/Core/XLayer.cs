using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XLayer : XObject, ILayer
    {
        public string _name;
        public IList<XShape> _shapes;
        public Action _invalidate;

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

        public Action Invalidate
        {
            get { return _invalidate; }
            set
            {
                if (value != _invalidate)
                {
                    _invalidate = value;
                    Notify("Invalidate");
                }
            }
        }

        public static ILayer Create(string name)
        {
            return new XLayer()
            {
                Name = name,
                Shapes = new ObservableCollection<XShape>()
            };
        }
    }
}
