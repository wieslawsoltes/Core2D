// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class InvalidateLayerEventArgs : EventArgs { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class Layer : ObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        public event InvalidateLayerEventHandler InvalidateLayer;

        private string _name;
        private Container _owner;
        private bool _isVisible;
        private IList<BaseShape> _shapes;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public Container Owner
        {
            get { return _owner; }
            set
            {
                if (value != _owner)
                {
                    _owner = value;
                    Notify("Owner");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void Invalidate()
        {
            var handler = InvalidateLayer;
            if (handler != null)
            {
                handler(this, new InvalidateLayerEventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static Layer Create(string name, Container owner = null, bool isVisible = true)
        {
            return new Layer()
            {
                Name = name,
                Owner = owner,
                IsVisible = isVisible,
                Shapes = new ObservableCollection<BaseShape>()
            };
        }
    }
}
