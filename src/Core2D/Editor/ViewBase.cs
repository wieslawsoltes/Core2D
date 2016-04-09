// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor
{
    /// <summary>
    /// View base class.
    /// </summary>
    public abstract class ViewBase : ObservableObject
    {
        private object _dataContext;

        /// <summary>
        /// Gets or sets data context.
        /// </summary>
        public object DataContext
        {
            get { return _dataContext; }
            set { Update(ref _dataContext, value); }
        }
    }
}
