using System;

namespace Core2D.Input
{
    public interface IInputSource
    {
        IObservable<InputArgs> LeftDown { get; set; }

        IObservable<InputArgs> LeftUp { get; set; }

        IObservable<InputArgs> RightDown { get; set; }

        IObservable<InputArgs> RightUp { get; set; }

        IObservable<InputArgs> Move { get; set; }
    }
}
