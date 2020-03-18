using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input source base class.
    /// </summary>
    public abstract class InputSource : IInputSource
    {
        /// <inheritdoc/>
        public IObservable<InputArgs> LeftDown { get; set; }

        /// <inheritdoc/>
        public IObservable<InputArgs> LeftUp { get; set; }

        /// <inheritdoc/>
        public IObservable<InputArgs> RightDown { get; set; }

        /// <inheritdoc/>
        public IObservable<InputArgs> RightUp { get; set; }

        /// <inheritdoc/>
        public IObservable<InputArgs> Move { get; set; }
    }
}
