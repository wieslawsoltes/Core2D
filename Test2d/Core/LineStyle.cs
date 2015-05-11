// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class LineStyle : ObservableObject
    {
        private string _name;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private LineCap _lineCap;
        private double[] _dashes;
        private double _dashOffset;

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

        public LineCap LineCap
        {
            get { return _lineCap; }
            set
            {
                if (value != _lineCap)
                {
                    _lineCap = value;
                    Notify("LineCap");
                }
            }
        }

        public double[] Dashes
        {
            get { return _dashes; }
            set
            {
                if (value != _dashes)
                {
                    _dashes = value;
                    Notify("Dashes");
                }
            }
        }

        public double DashOffset
        {
            get { return _dashOffset; }
            set
            {
                if (value != _dashOffset)
                {
                    _dashOffset = value;
                    Notify("DashOffset");
                }
            }
        }

        public static LineStyle Create(
            ArrowStyle startArrowStyle = default(ArrowStyle),
            ArrowStyle endArrowStyle = default(ArrowStyle),
            LineCap lineCap = LineCap.Round,
            double[] dashes = default(double[]),
            double dashOffset = 0.0)
        {
            return new LineStyle() 
            { 
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle,
                LineCap = lineCap,
                Dashes = dashes,
                DashOffset = dashOffset
            };
        }
    }
}
