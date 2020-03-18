using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Defines input source contract.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Left down events.
        /// </summary>
        IObservable<InputArgs> LeftDown { get; set; }

        /// <summary>
        /// Left up events.
        /// </summary>
        IObservable<InputArgs> LeftUp { get; set; }

        /// <summary>
        /// Right down events.
        /// </summary>
        IObservable<InputArgs> RightDown { get; set; }

        /// <summary>
        /// Right up events.
        /// </summary>
        IObservable<InputArgs> RightUp { get; set; }

        /// <summary>
        /// Move events.
        /// </summary>
        IObservable<InputArgs> Move { get; set; }
    }
}
