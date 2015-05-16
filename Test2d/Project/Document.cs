// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Document : ObservableObject
    {
        private string _name;
        private IList<Container> _containers;

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
        public IList<Container> Containers
        {
            get { return _containers; }
            set
            {
                if (value != _containers)
                {
                    _containers = value;
                    Notify("Containers");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Document Create(string name = "Document")
        {
            return new Document()
            {
                Name = name,
                Containers = new ObservableCollection<Container>()
            };
        }
    }
}
