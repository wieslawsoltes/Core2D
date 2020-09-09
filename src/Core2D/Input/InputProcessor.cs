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

    /// <summary>
    /// Provides mouse input for target object.
    /// </summary>
    public class InputProcessor : IDisposable
    {
        private IDisposable? _leftDownDisposable = null;
        private IDisposable? _leftUpDisposable = null;
        private IDisposable? _rightDownDisposable = null;
        private IDisposable? _rightUpDisposable = null;
        private IDisposable? _moveDisposable = null;

        private static IDisposable? ConnectLeftDown(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextLeftDown);
            return source.LeftDown?.Subscribe(observer);

            static void OnNextLeftDown(IInputTarget target, InputArgs args)
            {
                if (target.IsLeftDownAvailable())
                {
                    target.LeftDown(args);
                }
            }
        }

        private static IDisposable? ConnectLeftUp(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextLeftUp);
            return source.LeftUp?.Subscribe(observer);

            static void OnNextLeftUp(IInputTarget target, InputArgs args)
            {
                if (target.IsLeftUpAvailable())
                {
                    target.LeftUp(args);
                }
            }
        }

        private static IDisposable? ConnectRightDown(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextRightDown);
            return source.RightDown?.Subscribe(observer);

            static void OnNextRightDown(IInputTarget target, InputArgs args)
            {
                if (target.IsRightDownAvailable())
                {
                    target.RightDown(args);
                }
            }
        }

        private static IDisposable? ConnectRightUp(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextRightUp);
            return source.RightUp?.Subscribe(observer);

            static void OnNextRightUp(IInputTarget target, InputArgs args)
            {
                if (target.IsRightUpAvailable())
                {
                    target.RightUp(args);
                }
            }
        }

        private static IDisposable? ConnectMove(IInputSource source, IInputTarget target)
        {
            var observer = new InputArgsObserver(target, OnNextMove);
            return source.Move?.Subscribe(observer);

            static void OnNextMove(IInputTarget target, InputArgs args)
            {
                if (target.IsMoveAvailable())
                {
                    target.Move(args);
                }
            }
        }

        private static void DisconnectLeftDown(IDisposable? disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectLeftUp(IDisposable? disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectRightDown(IDisposable? disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectRightUp(IDisposable? disposable)
        {
            disposable?.Dispose();
        }

        private static void DisconnectMove(IDisposable? disposable)
        {
            disposable?.Dispose();
        }

        /// <summary>
        /// Connects source and target inputs.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public void Connect(IInputSource source, IInputTarget target)
        {
            _leftDownDisposable = ConnectLeftDown(source, target);
            _leftUpDisposable = ConnectLeftUp(source, target);
            _rightDownDisposable = ConnectRightDown(source, target);
            _rightUpDisposable = ConnectRightUp(source, target);
            _moveDisposable = ConnectMove(source, target);
        }

        /// <summary>
        /// Disconnects source and target inputs.
        /// </summary>
        public void Disconnect()
        {
            DisconnectLeftDown(_leftDownDisposable);
            DisconnectLeftUp(_leftUpDisposable);
            DisconnectRightDown(_rightDownDisposable);
            DisconnectRightUp(_rightUpDisposable);
            DisconnectMove(_moveDisposable);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
