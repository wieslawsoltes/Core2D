using System;

namespace Core2D.Input
{
    public interface IInputSource
    {
        IObservable<InputArgs> BeginDown { get; set; }

        IObservable<InputArgs> BeginUp { get; set; }

        IObservable<InputArgs> EndDown { get; set; }

        IObservable<InputArgs> EndUp { get; set; }

        IObservable<InputArgs> Move { get; set; }
    }
}
