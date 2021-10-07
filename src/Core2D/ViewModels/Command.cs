#nullable enable
using System;
using System.Windows.Input;

namespace Core2D.ViewModels;

public class Command : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public event EventHandler? CanExecuteChanged;

    public Command(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (() => true);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke();
    }
}