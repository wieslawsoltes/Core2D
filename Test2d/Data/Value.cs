// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Value : ObservableObject
    {
        private string _content;

        /// <summary>
        /// 
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { Update(ref _content, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Value Create(string content)
        {
            return new Value()
            {
                Content = content,
            };
        }
    }
}
