// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class InvalidateLayerEventArgs : EventArgs { }
    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    public class Layer : ObservableObject
    {
        public event InvalidateLayerEventHandler InvalidateLayer;

        private string _name;
        private bool _isVisible;
        private IList<BaseShape> _shapes;

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

        public IList<BaseShape> Shapes
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

        public void Invalidate()
        {
            var handler = InvalidateLayer;
            if (handler != null)
            {
                handler(this, new InvalidateLayerEventArgs());
            }
        }

        public static Layer Create(string name, bool isVisible = true)
        {
            return new Layer()
            {
                Name = name,
                IsVisible = isVisible,
                Shapes = new ObservableCollection<BaseShape>()
            };
        }
    }
}
