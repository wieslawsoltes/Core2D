// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private Action _execute;
        private Func<bool> _canExecute;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public Command(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_execute == null)
                return;
            _execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static ICommand Create(Action execute, Func<bool> canExecute = null)
        {
            return new Command(execute, canExecute);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Command<T> : ICommand where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter as T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_execute == null)
                return;
            _execute(parameter as T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static ICommand Create(Action<T> execute, Func<T, bool> canExecute = null)
        {
            return new Command<T>(execute, canExecute);
        }
    }
}
