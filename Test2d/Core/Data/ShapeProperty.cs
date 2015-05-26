// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeProperty : ObservableObject
    {
        private string _name;
        private object _value;

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
        public object Value
        {
            get { return _value; }
            set { Update(ref _value, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ShapeProperty Create(string name, object value)
        {
            return new ShapeProperty() 
            { 
                Name = name,
                Value = value 
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        { 
            return _value.ToString(); 
        }
    }
}
