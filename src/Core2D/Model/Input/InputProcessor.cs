using System;

namespace Core2D.Input
{
    public class InputProcessor : IDisposable
    {
        private IDisposable _beginDownDisposable = null;
        private IDisposable _beginUpDisposable = null;
        private IDisposable _endDownDisposable = null;
        private IDisposable _endUpDisposable = null;
        private IDisposable _moveDisposable = null;

        private static IDisposable ConnectBeginDown(InputSource source, InputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextBeginDown);
            return source.BeginDown.Subscribe(observer);

            static void OnNextBeginDown(InputTarget target, InputArgs args)
            {
                if (target.IsBeginDownAvailable())
                {
                    target.BeginDown(args);
                }
            }
        }

        private static IDisposable ConnectBeginUp(InputSource source, InputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextBeginUp);
            return source.BeginUp.Subscribe(observer);

            static void OnNextBeginUp(InputTarget target, InputArgs args)
            {
                if (target.IsBeginUpAvailable())
                {
                    target.BeginUp(args);
                }
            }
        }

        private static IDisposable ConnectEndDown(InputSource source, InputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextEndDown);
            return source.EndDown.Subscribe(observer);

            static void OnNextEndDown(InputTarget target, InputArgs args)
            {
                if (target.IsEndDownAvailable())
                {
                    target.EndDown(args);
                }
            }
        }

        private static IDisposable ConnectEndUp(InputSource source, InputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextEndUp);
            return source.EndUp.Subscribe(observer);

            static void OnNextEndUp(InputTarget target, InputArgs args)
            {
                if (target.IsEndUpAvailable())
                {
                    target.EndUp(args);
                }
            }
        }

        private static IDisposable ConnectMove(InputSource source, InputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextMove);
            return source.Move.Subscribe(observer);

            static void OnNextMove(InputTarget target, InputArgs args)
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
