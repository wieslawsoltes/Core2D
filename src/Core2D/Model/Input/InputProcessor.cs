#nullable enable
using System;

namespace Core2D.Model.Input;

public class InputProcessor : IDisposable
{
    private IDisposable? _beginDownDisposable;
    private IDisposable? _beginUpDisposable;
    private IDisposable? _endDownDisposable;
    private IDisposable? _endUpDisposable;
    private IDisposable? _moveDisposable;

    private static IDisposable? ConnectBeginDown(InputSource source, InputTarget target)
    {
        var observer = new InputArgsObserver(target, OnNextBeginDown);
        return source.BeginDown?.Subscribe(observer);

        static void OnNextBeginDown(InputTarget target, InputArgs args)
        {
            if (target.IsBeginDownAvailable())
            {
                target.BeginDown(args);
            }
        }
    }

    private static IDisposable? ConnectBeginUp(InputSource source, InputTarget target)
    {
        var observer = new InputArgsObserver(target, OnNextBeginUp);
        return source.BeginUp?.Subscribe(observer);

        static void OnNextBeginUp(InputTarget target, InputArgs args)
        {
            if (target.IsBeginUpAvailable())
            {
                target.BeginUp(args);
            }
        }
    }

    private static IDisposable? ConnectEndDown(InputSource source, InputTarget target)
    {
        var observer = new InputArgsObserver(target, OnNextEndDown);
        return source.EndDown?.Subscribe(observer);

        static void OnNextEndDown(InputTarget target, InputArgs args)
        {
            if (target.IsEndDownAvailable())
            {
                target.EndDown(args);
            }
        }
    }

    private static IDisposable? ConnectEndUp(InputSource source, InputTarget target)
    {
        var observer = new InputArgsObserver(target, OnNextEndUp);
        return source.EndUp?.Subscribe(observer);

        static void OnNextEndUp(InputTarget target, InputArgs args)
        {
            if (target.IsEndUpAvailable())
            {
                target.EndUp(args);
            }
        }
    }

    private static IDisposable? ConnectMove(InputSource source, InputTarget target)
    {
        var observer = new InputArgsObserver(target, OnNextMove);
        return source.Move?.Subscribe(observer);

        static void OnNextMove(InputTarget target, InputArgs args)
        {
            if (target.IsMoveAvailable())
            {
                target.Move(args);
            }
        }
    }

    public void Connect(InputSource source, InputTarget target)
    {
        _beginDownDisposable = ConnectBeginDown(source, target);
        _beginUpDisposable = ConnectBeginUp(source, target);
        _endDownDisposable = ConnectEndDown(source, target);
        _endUpDisposable = ConnectEndUp(source, target);
        _moveDisposable = ConnectMove(source, target);
    }

    public void Disconnect()
    {
        _beginDownDisposable?.Dispose();
        _beginUpDisposable?.Dispose();
        _endDownDisposable?.Dispose();
        _endUpDisposable?.Dispose();
        _moveDisposable?.Dispose();
    }

    public void Dispose()
    {
        Disconnect();
    }
}