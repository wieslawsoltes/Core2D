// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D
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
        private bool _isVisible = true;
        private ImmutableArray<BaseShape> _shapes;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Update(ref _isVisible, value); Invalidate(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<BaseShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        public Layer()
            : base()
        {
            _shapes = ImmutableArray.Create<BaseShape>();
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
        /// Creates a new <see cref="Layer"/> instance.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static Layer Create(string name = "Layer", Container owner = null, bool isVisible = true)
        {
            return new Layer()
            {
                Name = name,
                Owner = owner,
                IsVisible = isVisible
            };
        }
    }
}
