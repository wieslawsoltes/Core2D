#nullable enable
using System;
using System.Windows.Input;

namespace Core2D.ViewModels;

public class Command<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool> _canExecute;

    public event EventHandler? CanExecuteChanged;

    public Command(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke((T?)parameter);
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke((T?)parameter);
    }
}
