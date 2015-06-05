// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// Named XGroup shapes collection.
    /// </summary>
    public class GroupLibrary : ObservableObject
    {
        private string _name;
        private ImmutableArray<XGroup> _groups;
        private XGroup _currentGroup;

        /// <summary>
        /// Gets or sets group library name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets a colletion XGroup.
        /// </summary>
        public ImmutableArray<XGroup> Groups
        {
            get { return _groups; }
            set { Update(ref _groups, value); }
        }

        /// <summary>
        /// Gets or sets currenly selected group from Groups collection.
        /// </summary>
        public XGroup CurrentGroup
        {
            get { return _currentGroup; }
            set { Update(ref _currentGroup, value); }
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
                Groups = ImmutableArray.Create<XGroup>()
            };
        }
    }
}
