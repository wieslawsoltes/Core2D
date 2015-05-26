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
            set { Update(ref _name, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ArrowStyle StartArrowStyle
        {
            get { return _startArrowStyle; }
            set { Update(ref _startArrowStyle, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ArrowStyle EndArrowStyle
        {
            get { return _endArrowStyle; }
            set { Update(ref _endArrowStyle, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LineCap LineCap
        {
            get { return _lineCap; }
            set { Update(ref _lineCap, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double[] Dashes
        {
            get { return _dashes; }
            set { Update(ref _dashes, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double DashOffset
        {
            get { return _dashOffset; }
            set { Update(ref _dashOffset, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public MaxLengthFlags MaxLengthFlags
        {
            get { return _maxLengthFlags; }
            set { Update(ref _maxLengthFlags, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double MaxLength
        {
            get { return _maxLength; }
            set { Update(ref _maxLength, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState MaxLengthStartState
        {
            get { return _maxLengthStartState; }
            set { Update(ref _maxLengthStartState, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState MaxLengthEndState
        {
            get { return _maxLengthEndState; }
            set { Update(ref _maxLengthEndState, value); }
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
