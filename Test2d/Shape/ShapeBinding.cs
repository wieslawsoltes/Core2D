// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeBinding : ObservableObject
    {
        private string _property;
        private string _path;

        /// <summary>
        /// 
        /// </summary>
        public string Property
        {
            get { return _property; }
            set { Update(ref _property, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ShapeBinding Create(string property, string path)
        {
            return new ShapeBinding() 
            { 
                Property = property,
                Path = path 
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        { 
            return _path; 
        }
    }
}
