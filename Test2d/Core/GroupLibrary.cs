// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// Named XGroup shapes collection.
    /// </summary>
    public class GroupLibrary : ObservableObject
    {
        private string _name;
        private IList<XGroup> _groups;
        private XGroup _currentGroup;

        /// <summary>
        /// Gets or sets group library name.
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
        /// Gets or sets a colletion XGroup.
        /// </summary>
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

        /// <summary>
        /// Gets or sets currenly selected group fro Groups collection.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of the GroupLibrary class.
        /// </summary>
        /// <param name="name">The group library name.</param>
        /// <returns>The new instance of the GroupLibrary class.</returns>
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
