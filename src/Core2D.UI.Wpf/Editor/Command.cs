// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;

namespace Core2D.UI.Wpf.Editor
{
    /// <summary>
    /// Command implementation.
    /// </summary>
    public class Command : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

#pragma warning disable CS0067
        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Initialize new instance of <see cref="Command"/> class.
        /// </summary>
        /// <param name="canExecute">The can execute function.</param>
        /// <param name="execute">The execute action.</param>
        public Command(Func<object, bool> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) == true;
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
