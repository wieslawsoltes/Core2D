// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseStyle : ObservableObject
    {
        private string _name;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private double[] _dashes;
        private double _dashOffset;

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
        public ArgbColor Stroke
        {
            get { return _stroke; }
            set { Update(ref _stroke, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArgbColor Fill
        {
            get { return _fill; }
            set { Update(ref _fill, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Thickness
        {
            get { return _thickness; }
            set { Update(ref _thickness, value); }
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
    }
}
