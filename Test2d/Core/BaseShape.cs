// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    public abstract class BaseShape : ObservableObject
    {
        private ShapeState _state = ShapeState.Visible | ShapeState.Printable;

        public ShapeState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    Notify("State");
                }
            }
        }

        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy);
    }
}
