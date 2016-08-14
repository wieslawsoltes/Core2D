// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Provides mouse input for target object.
    /// </summary>
    public class InputProcessor : IDisposable
    {
        private readonly IDisposable _leftDownDisposable;
        private readonly IDisposable _leftUpDisposable;
        private readonly IDisposable _rightDownDisposable;
        private readonly IDisposable _rightUpDisposable;
        private readonly IDisposable _moveDisposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputProcessor"/> class.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <param name="target">The input target.</param>
        public InputProcessor(IInputSource source, IInputTarget target)
        {
            _leftDownDisposable = source.LeftDown.Subscribe(
                (v) =>
                {
                    if (target.IsLeftDownAvailable())
                    {
                        target.LeftDown(v.X, v.Y);
                    }
                });

            _leftUpDisposable = source.LeftUp.Subscribe(
                (v) =>
                {
                    if (target.IsLeftUpAvailable())
                    {
                        target.LeftUp(v.X, v.Y);
                    }
                });

            _rightDownDisposable = source.RightDown.Subscribe(
                (v) =>
                {
                    if (target.IsRightDownAvailable())
                    {
                        target.RightDown(v.X, v.Y);
                    }
                });

            _rightUpDisposable = source.RightUp.Subscribe(
                (v) =>
                {
                    if (target.IsRightUpAvailable())
                    {
                        target.RightUp(v.X, v.Y);
                    }
                });

            _moveDisposable = source.Move.Subscribe(
                (v) =>
                {
                    if (target.IsMoveAvailable())
                    {
                        target.Move(v.X, v.Y);
                    }
                });
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~InputProcessor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftDownDisposable.Dispose();
                _leftUpDisposable.Dispose();
                _rightDownDisposable.Dispose();
                _rightUpDisposable.Dispose();
                _moveDisposable.Dispose();
            }
        }
    }
}
