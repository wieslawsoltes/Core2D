// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
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
        /// Initializes a new instance of the <see cref="Command"/> class.
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
            if (_canExecute == null)
                return true;
            return _canExecute(parameter as T);
        }

        /// <inheritdoc/>
        public override void Execute(object parameter)
        {
            if (_execute == null)
                return;
            _execute(parameter as T);
        }

        /// <summary>
        /// Creates a new <see cref="Command"/> instance.
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
