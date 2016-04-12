// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor
{
    /// <summary>
    /// View base class.
    /// </summary>
    public abstract class ViewBase : ObservableObject
    {
        private string _name;
        private object _context;

        /// <summary>
        /// Gets or sets view name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets view context.
        /// </summary>
        public object Context
        {
            get { return _context; }
            set { Update(ref _context, value); }
        }
    }
}
