// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Document : ObservableObject
    {
        private string _name;
        private ImmutableArray<Container> _containers;

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
        public ImmutableArray<Container> Containers
        {
            get { return _containers; }
            set { Update(ref _containers, value); }
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
                Containers = ImmutableArray.Create<Container>()
            };
        }
    }
}
