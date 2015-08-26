// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class RecentProject : ObservableObject
    {
        private string _name;
        private string _path;

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
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RecentProject Create(string name, string path)
        {
            return new RecentProject()
            {
                Name = name,
                Path = path
            };
        }
    }
}
