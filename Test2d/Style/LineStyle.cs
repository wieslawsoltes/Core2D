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
        /// <param name="name"></param>
        /// <param name="maxLengthFlags"></param>
        /// <param name="maxLength"></param>
        /// <param name="maxLengthStartState"></param>
        /// <param name="maxLengthEndState"></param>
        /// <returns></returns>
        public static LineStyle Create(
            string name = "",
            MaxLengthFlags maxLengthFlags = MaxLengthFlags.Disabled,
            double maxLength = 15.0,
            ShapeState maxLengthStartState = ShapeState.Connector | ShapeState.Output,
            ShapeState maxLengthEndState = ShapeState.Connector | ShapeState.Input)
        {
            return new LineStyle() 
            { 
                Name = name,
                MaxLengthFlags = maxLengthFlags,
                MaxLength = maxLength,
                MaxLengthStartState = maxLengthStartState,
                MaxLengthEndState = maxLengthEndState
            };
        }
    }
}
