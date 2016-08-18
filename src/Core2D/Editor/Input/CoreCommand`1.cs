// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Generic input command.
    /// </summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    public abstract class CoreCommand<T> : ICommand where T : class
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
        public bool CanExecute(object parameter)
        {
            return this.CanRun(parameter as T);
        }

        /// <summary>
        /// Invoke execute action.
        /// </summary>
        /// <param name="parameter">The execute parameter.</param>
        public void Execute(object parameter)
        {
            this.Run(parameter as T);
        }

        /// <summary>
        /// Raise <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <summary>
        /// Raise <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Check if can invoke execute action.
        /// </summary>
        /// <param name="parameter">The can execute parameter.</param>
        /// <returns>True if can invoke execute action.</returns>
        public abstract bool CanRun(T parameter);

        /// <summary>
        /// Invoke execute action.
        /// </summary>
        /// <param name="parameter">The execute parameter.</param>
        public abstract void Run(T parameter);
    }
}
