// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    public class Document : ObservableObject
    {
        private string _name;
        private IList<Container> _containers;
        private Container _currentContainer;

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

        public Container CurrentContainer
        {
            get { return _currentContainer; }
            set
            {
                if (value != _currentContainer)
                {
                    _currentContainer = value;
                    Notify("CurrentContainer");
                }
            }
        }

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
