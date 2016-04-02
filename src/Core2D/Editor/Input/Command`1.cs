// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Generic input command.
    /// </summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    public class Command<T> : ICoreCommand<T> where T : class
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute function.</param>
        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter as T) ?? true;
        }

        /// <inheritdoc/>
        public override void Execute(object parameter)
        {
            _execute?.Invoke(parameter as T);
        }

        /// <summary>
        /// Creates a new <see cref="Command{T}"/> instance.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute function.</param>
        /// <returns>The new instance of the <see cref="Command{T}"/> class.</returns>
        public static ICoreCommand<T> Create(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new Command<T>(execute, canExecute);
        }
    }
}
