// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input command.
    /// </summary>
    public class Command : Command<object>
    {
        private Action _execute;
        private Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute function.</param>
        public Command(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc/>
        public override bool CanRun(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <inheritdoc/>
        public override void Run(object parameter)
        {
            _execute?.Invoke();
        }

        /// <summary>
        /// Creates a new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute function.</param>
        /// <returns>The new instance of the <see cref="Command"/> class.</returns>
        public static Command Create(Action execute, Func<bool> canExecute = null)
        {
            return new Command(execute, canExecute);
        }
    }
}
