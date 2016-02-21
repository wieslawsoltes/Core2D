// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input command.
    /// </summary>
    public abstract class ICoreCommand : ICommand
    {
        /// <summary>
        /// Gets or sets CanExecuteChanged event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Check if can invoke execute action.
        /// </summary>
        /// <param name="parameter">The can execute parameter.</param>
        /// <returns>True if can invoke execute action.</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invoke execute action.
        /// </summary>
        /// <param name="parameter">The execute parameter.</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raise <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <summary>
        /// Raise <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
