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
        private string _name;
        private bool _isVisible;
        private IList<XShape> _shapes;
        private Action _invalidate;

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

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    Invalidate();
                    Notify("IsVisible");
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

        public void SetInvalidate(Action invalidate)
        {
            _invalidate = invalidate;
        }

        public void Invalidate()
        {
            if (_invalidate != null)
            {
                _invalidate();
            }
        }

        public static ILayer Create(string name, bool isVisible = true)
        {
            return new XLayer()
            {
                Name = name,
                IsVisible = isVisible,
                Shapes = new ObservableCollection<XShape>()
            };
        }
    }
}
