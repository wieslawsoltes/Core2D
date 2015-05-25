// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Column : ObservableObject
    {
        private Guid _id;
        private string _name;
        private double _width;
        private bool _isVisible;

        /// <summary>
        /// 
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    Notify("Id");
                }
            }
        }
        
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
        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    Notify("Width");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    Notify("IsVisible");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public static Column Create(
            string name,
            double width = double.NaN,
            bool isVisible = true)
        {
            return new Column()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Width = width,
                IsVisible = isVisible
            };
        }
    }
}
