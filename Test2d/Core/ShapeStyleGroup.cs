// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test2d
{
    public class ShapeStyleGroup : ObservableObject
    {
        private string _name;
        private IList<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;

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

        public IList<ShapeStyle> Styles
        {
            get { return _styles; }
            set
            {
                if (value != _styles)
                {
                    _styles = value;
                    Notify("Styles");
                }
            }
        }

        public ShapeStyle CurrentStyle
        {
            get { return _currentStyle; }
            set
            {
                if (value != _currentStyle)
                {
                    _currentStyle = value;
                    Notify("CurrentStyle");
                }
            }
        }

        public static ShapeStyleGroup Create(string name)
        {
            return new ShapeStyleGroup()
            {
                Name = name,
                Styles = new ObservableCollection<ShapeStyle>()
            };
        }
    }
}
