// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    public class GroupLibrary : ObservableObject
    {
        private string _name;
        private IList<XGroup> _groups;
        private XGroup _currentGroup;

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

        public IList<XGroup> Groups
        {
            get { return _groups; }
            set
            {
                if (value != _groups)
                {
                    _groups = value;
                    Notify("Groups");
                }
            }
        }

        public XGroup CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                if (value != _currentGroup)
                {
                    _currentGroup = value;
                    Notify("CurrentGroup");
                }
            }
        }

        public static GroupLibrary Create(string name)
        {
            return new GroupLibrary()
            {
                Name = name,
                Groups = new ObservableCollection<XGroup>()
            };
        }
    }
}
