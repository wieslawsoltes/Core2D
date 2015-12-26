// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICoreCommand<T> : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        void NotifyCanExecuteChanged();
    }
}
