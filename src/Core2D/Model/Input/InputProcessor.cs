using System;

namespace Core2D.Input
{
    internal class InputArgsObserver : IObserver<InputArgs>
    {
        private readonly IInputTarget _target;
        private readonly Action<IInputTarget, InputArgs> _onNext;

        public InputArgsObserver(IInputTarget target, Action<IInputTarget, InputArgs> onNext)
        {
            _target = target;
            _onNext = onNext;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(InputArgs value)
        {
            _onNext(_target, value);
        }
    }

    public class InputProcessor : IDisposable
    {
        private IDisposable _beginDownDisposable = null;
        private IDisposable _beginUpDisposable = null;
        private IDisposable _endDownDisposable = null;
        private IDisposable _endUpDisposable = null;
        private IDisposable _moveDisposable = null;

        private static IDisposable ConnectBeginDown(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextBeginDown);
            return source.BeginDown.Subscribe(observer);

            static void OnNextBeginDown(IInputTarget target, InputArgs args)
            {
                if (target.IsBeginDownAvailable())
                {
                    target.BeginDown(args);
                }
            }
        }

        private static IDisposable ConnectBeginUp(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextBeginUp);
            return source.BeginUp.Subscribe(observer);

            static void OnNextBeginUp(IInputTarget target, InputArgs args)
            {
                if (target.IsBeginUpAvailable())
                {
                    target.BeginUp(args);
                }
            }
        }

        private static IDisposable ConnectEndDown(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextEndDown);
            return source.EndDown.Subscribe(observer);

            static void OnNextEndDown(IInputTarget target, InputArgs args)
            {
                if (target.IsEndDownAvailable())
                {
                    target.EndDown(args);
                }
            }
        }

        private static IDisposable ConnectEndUp(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextEndUp);
            return source.EndUp.Subscribe(observer);

            static void OnNextEndUp(IInputTarget target, InputArgs args)
            {
                if (target.IsEndUpAvailable())
                {
                    target.EndUp(args);
                }
            }
        }

        private static IDisposable ConnectMove(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextMove);
            return source.Move.Subscribe(observer);

            static void OnNextMove(IInputTarget target, InputArgs args)
            {
                if (target.IsMoveAvailable())
                {
                    target.Move(args);
                }
            }
        }

        private static void DisconnectBeginDown(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectBeginUp(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectEndDown(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectEndUp(IDisposable disposable)
        {
            disposable.Dispose();
        }

        private static void DisconnectMove(IDisposable disposable)
        {
            disposable?.Dispose();
        }

        public void Connect(IInputSource source, IInputTarget target)
        {
            _beginDownDisposable = ConnectBeginDown(source, target);
            _beginUpDisposable = ConnectBeginUp(source, target);
            _endDownDisposable = ConnectEndDown(source, target);
            _endUpDisposable = ConnectEndUp(source, target);
            _moveDisposable = ConnectMove(source, target);
        }

        public void Disconnect()
        {
            DisconnectBeginDown(_beginDownDisposable);
            DisconnectBeginUp(_beginUpDisposable);
            DisconnectEndDown(_endDownDisposable);
            DisconnectEndUp(_endUpDisposable);
            DisconnectMove(_moveDisposable);
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
