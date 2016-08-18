// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Generic input command.
    /// </summary>
    /// <typeparam name="T">The command parameter type.</typeparam>
    public class Command<T> : CoreCommand<T> where T : class
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        public Command()
        {
        }

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
        public override bool CanRun(T parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc/>
        public override void Run(T parameter)
        {
            _execute?.Invoke(parameter);
        }

        /// <summary>
        /// Creates a new <see cref="Command{T}"/> instance.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute function.</param>
        /// <returns>The new instance of the <see cref="Command{T}"/> class.</returns>
        public static Command<T> Create(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new Command<T>(execute, canExecute);
        }
    }
}
