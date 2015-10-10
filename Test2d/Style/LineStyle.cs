// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class LineStyle : ObservableObject
    {
        private string _name;
        private LineFixedLength _fixedLength;

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
        public LineFixedLength FixedLength
        {
            get { return _fixedLength; }
            set { Update(ref _fixedLength, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fixedLength"></param>
        /// <returns></returns>
        public static LineStyle Create(
            string name = "",
            LineFixedLength fixedLength = null)
        {
            return new LineStyle() 
            { 
                Name = name,
                FixedLength = fixedLength ?? LineFixedLength.Create()
            };
        }
    }
}
