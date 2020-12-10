using System;

namespace Core2D.Model.Input
{
    internal class InputArgsObserver : IObserver<InputArgs>
    {
        private readonly InputTarget _target;
        private readonly Action<InputTarget, InputArgs> _onNext;

        public InputArgsObserver(InputTarget target, Action<InputTarget, InputArgs> onNext)
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
}
