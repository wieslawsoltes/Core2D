// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Container : ObservableObject
    {
        private string _name;
        private ImmutableArray<Layer> _layers;
        private Layer _currentLayer;
        private Layer _workingLayer;
        private Layer _helperLayer;
        private BaseShape _currentShape;

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
        public ImmutableArray<Layer> Layers
        {
            get { return _layers; }
            set { Update(ref _layers, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Layer CurrentLayer
        {
            get { return _currentLayer; }
            set { Update(ref _currentLayer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Layer WorkingLayer
        {
            get { return _workingLayer; }
            set { Update(ref _workingLayer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Layer HelperLayer
        {
            get { return _helperLayer; }
            set { Update(ref _helperLayer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseShape CurrentShape
        {
            get { return _currentShape; }
            set { Update(ref _currentShape, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Container Template
        {
            get { return this; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            foreach (var layer in Layers)
            {
                layer.Shapes = ImmutableArray.Create<BaseShape>();
            }
            WorkingLayer.Shapes = ImmutableArray.Create<BaseShape>();
            HelperLayer.Shapes = ImmutableArray.Create<BaseShape>();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Invalidate()
        {
            if (Layers != null)
            {
                foreach (var layer in Layers)
                {
                    layer.Invalidate();
                }
            }

            if (WorkingLayer != null)
            {
                WorkingLayer.Invalidate();
            }

            if (HelperLayer != null)
            {
                HelperLayer.Invalidate();
            }
        }
    }
}
