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
        private object _data;

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
        public object Data
        {
            get { return _data; }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    Notify("Data");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ShapeProperty Create(string name, object data)
        {
            return new ShapeProperty() 
            { 
                Name = name,
                Data = data 
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        { 
            return Data.ToString(); 
        }
    }
}
