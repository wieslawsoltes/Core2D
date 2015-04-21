// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class LineStyle : ObservableObject
    {
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;

        public ArrowStyle StartArrowStyle
        {
            get { return _startArrowStyle; }
            set
            {
                if (value != _startArrowStyle)
                {
                    _startArrowStyle = value;
                    Notify("StartArrowStyle");
                }
            }
        }
        
        public ArrowStyle EndArrowStyle
        {
            get { return _endArrowStyle; }
            set
            {
                if (value != _endArrowStyle)
                {
                    _endArrowStyle = value;
                    Notify("EndArrowStyle");
                }
            }
        }

        public static LineStyle Create(
            ArrowStyle startArrowStyle = null,
            ArrowStyle endArrowStyle = null)
        {
            return new LineStyle() 
            { 
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle
            };
        }
    }
}
