// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Binding : ObservableObject
    {
        private string _property;
        private string _path;
        private Data _owner;

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
        /// Gets or sets binding owner data object.
        /// </summary>
        public Data Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Binding"/> instance.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="path"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Binding Create(string property, string path, Data owner)
        {
            return new Binding()
            {
                Property = property,
                Path = path,
                Owner = owner
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
