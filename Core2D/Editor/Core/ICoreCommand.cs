// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// The core command interface.
    /// </summary>
    public interface ICoreCommand : ICommand
    {
        /// <summary>
        /// Raise <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void NotifyCanExecuteChanged();
    }
}
