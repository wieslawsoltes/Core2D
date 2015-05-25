// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class LineStyle : ObservableObject
    {
        private string _name;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private LineCap _lineCap;
        private double[] _dashes;
        private double _dashOffset;
        private MaxLengthFlags _maxLengthFlags;
        private double _maxLength;
        private ShapeState _maxLengthStartState;
        private ShapeState _maxLengthEndState;

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
        
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public MaxLengthFlags MaxLengthFlags
        {
            get { return _maxLengthFlags; }
            set
            {
                if (value != _maxLengthFlags)
                {
                    _maxLengthFlags = value;
                    Notify("MaxLengthFlags");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double MaxLength
        {
            get { return _maxLength; }
            set
            {
                if (value != _maxLength)
                {
                    _maxLength = value;
                    Notify("MaxLength");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState MaxLengthStartState
        {
            get { return _maxLengthStartState; }
            set
            {
                if (value != _maxLengthStartState)
                {
                    _maxLengthStartState = value;
                    Notify("MaxLengthStartState");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState MaxLengthEndState
        {
            get { return _maxLengthEndState; }
            set
            {
                if (value != _maxLengthEndState)
                {
                    _maxLengthEndState = value;
                    Notify("MaxLengthEndState");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startArrowStyle"></param>
        /// <param name="endArrowStyle"></param>
        /// <param name="lineCap"></param>
        /// <param name="dashes"></param>
        /// <param name="dashOffset"></param>
        /// <param name="verticalLength"></param>
        /// <param name="maxLengthFlags"></param>
        /// <param name="maxLength"></param>
        /// <param name="maxLengthStartState"></param>
        /// <param name="maxLengthEndState"></param>
        /// <returns></returns>
        public static LineStyle Create(
            ArrowStyle startArrowStyle = default(ArrowStyle),
            ArrowStyle endArrowStyle = default(ArrowStyle),
            LineCap lineCap = LineCap.Round,
            double[] dashes = default(double[]),
            double dashOffset = 0.0,
            double verticalLength = 22.5,
            MaxLengthFlags maxLengthFlags = MaxLengthFlags.Disabled,
            double maxLength = 0.0,
            ShapeState maxLengthStartState = ShapeState.Default,
            ShapeState maxLengthEndState = ShapeState.Default)
        {
            return new LineStyle() 
            { 
                StartArrowStyle = startArrowStyle,
                EndArrowStyle = endArrowStyle,
                LineCap = lineCap,
                Dashes = dashes,
                DashOffset = dashOffset,
                MaxLengthFlags = maxLengthFlags,
                MaxLength = maxLength,
                MaxLengthStartState = maxLengthStartState,
                MaxLengthEndState = maxLengthEndState
            };
        }
    }
}
